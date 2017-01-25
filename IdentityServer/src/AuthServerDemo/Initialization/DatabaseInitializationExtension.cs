using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;
using AuthServerDemo.Data;

namespace AuthServerDemo.Initialization
{
    public static class DatabaseInitializationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app, bool createTestData)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                serviceScope.ServiceProvider.GetRequiredService<AuthorizationServerDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (createTestData)
                {
                    context.InitializeTestData();
                }
            }
        }

        private static void InitializeTestData(this ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in FakeDataConfig.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in FakeDataConfig.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in FakeDataConfig.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }
    }
}