using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AuthServerDemo.Configuration;
using AuthServerDemo.Data;
using AuthServerDemo.Data.Entities;
using AuthServerDemo.Initialization;
using AuthServerDemo.Services;
using IdentityServer4.Services;
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using AuthServerDemo.Configuration.Settings;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Validation;
using AuthServerDemo.Data.Repository;
using IdentityServer4.Stores;
using AuthServerDemo.Data.Stores;
using AuthServerDemo.Data.Repository.InAppMemoryRepository;
using Microsoft.AspNetCore.Http;

namespace AuthServerDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly IHostingEnvironment environment;

        /// <summary>
        /// This constructor going to be execute first when application starts first time. Needed to read configuration files and configure environment. 
        /// </summary>
        /// <param name="env">Represents current execution environment</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            environment = env;

            Configuration = builder.Build();
        }

        /// <summary>
        /// Adding services to the services container makes them available within your application via dependency injection. 
        /// Going to be executed before Configure method.
        /// </summary>
        /// <param name="services">Takes IServiceCollection allows to add services in order to set up how application going to be executed</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString(Configuration.GetDatabaseConnectionStringName());
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Configuration of Identity User and Role models, and passing data store information in to EntityFramework
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AuthorizationServerDbContext, int>()
                .AddDefaultTokenProviders();

            //Registration of database context and adding configuration options to use PostgreSQL databse.
            services.AddDbContext<AuthorizationServerDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Specification of role-based authorization policy using claim
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Roles.User, policyUser =>
                {
                    policyUser.RequireClaim(JwtClaimTypes.Role, Roles.User);
                });
                options.AddPolicy(Roles.Admin, policyAdmin =>
                {
                    policyAdmin.RequireClaim(JwtClaimTypes.Role, Roles.Admin);
                });
            });

            // Adding MVC means we would be able to use build in ASP.NET MVC features like Routing using controllers\Authorization\Views etc.
            services.AddMvc();

            // If Redis:Enable == true we will store data in Redis
            // If Redis:Enable == false we will store data in application memory
            if (bool.Parse(Configuration["Redis:Enable"]))
            {

                // Adds singleton connection for Redis. Needs to be executed once as far as this is heavy operation
                services.AddSingleton(new RedisConnection(Configuration["Redis:Host"]));

                // Registration for Redis store implementation 
                services.AddTransient<IApplicationUserRepository, ApplicationUserRedisRepository>();
                services.AddTransient<IGrantRepository, GrantRedisRepository>();
            }
            else
            {
                // Registration for InMemory store implementation 
                //services.AddSingleton<IApplicationUserRepository, InAppMemoryUserRepository>();
                //services.AddSingleton<IGrantRepository, InAppMemoryGrantRepository>();
            }

            //Registration of custom implementation of interfaces that going to be injected and used during application execution
            //services.AddTransient<IPersistedGrantStore, PersistedGrantRedisStore>();
            services.AddTransient<IProfileService, IdentityProfileService>();
            //services.AddTransient<IResourceOwnerPasswordValidator, ApplicationUserPasswordValidator>();

            //Specification that IdentityServer going to be used with registration of required services
            services.AddIdentityServer()
                // Adding signing keys, this method will generate RSA keys
                .AddTemporarySigningCredential()
                .AddConfigurationStore(builder =>
                    builder.UseNpgsql(connectionString, options =>
                        options.MigrationsAssembly(migrationAssembly)))

                .AddOperationalStore(builder =>
                    builder.UseNpgsql(connectionString, options =>
                        options.MigrationsAssembly(migrationAssembly)))

                 .AddAspNetIdentity<ApplicationUser>()
                 .AddProfileService<IdentityProfileService>();
            // Adding user store
            //.ManageApplicationUsers();
        }

        /// <summary>
        /// The Configure method is used to specify how the ASP.NET application will respond to HTTP requests
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure app pipeline </param>
        /// <param name="env">Represents current execution environment</param>
        /// <param name="loggerFactory">Logging configuration</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Adds console logger
            loggerFactory.AddConsole();

            // Copy user information from PostgreSQL database in to redis on application start
            // app.CopyUsersFromDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Reads DatabaseSettings configuration section and migrates database if needed
            if (Configuration.IsMigrateDatabaseOnStartup())
            {
                app.ApplyMigrations(Configuration.IsMigrateDatabaseOnStatupWithTestingData());
            }

            // Enables Identity
            app.UseIdentity();

            // Enable IdentityServer4 
            // This middleware will manage HTTP request provided by IdentityServer4
            app.UseIdentityServer();

            // Allow CookiesAuthentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
                ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
            });

            app.UseOAuthAuthentication(new OAuthOptions
            {
                AuthenticationScheme = "LinkedIn",
                DisplayName = "LinkedIn",
                ClientId = "75xcbb4icwltx3",
                ClientSecret = "giSng0gfq81DTUzZ",

                CallbackPath = new PathString("/signin-linkedin"),

                AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization",
                TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken",
                UserInformationEndpoint = "https://api.linkedin.com/v1/people/~:(id,formatted-name,email-address,picture-url)",

                Scope = { "r_basicprofile", "r_emailaddress" },
            });

            app.UseFacebook(Configuration);

            // Clears default claim types mapping 
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Adds identity authorization 
            app.UseIdentityAuthentication(Configuration);

            // Enables static file serving
            app.UseStaticFiles();

            // Adds MVC middleware with default routing
            // Will manage requests to controllers
            app.UseMvcWithDefaultRoute();
        }
    }
}