using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.LinkedIn;
using AuthServerDemo.Configuration.Settings;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AuthServerDemo.Initialization
{
    public static class LinkedInAuthentificationExtansion
    {
        public static IApplicationBuilder UseLinkedIn(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions()
            {
                AuthenticationScheme = config.GetLinkedInAuthScheme(),
                DisplayName = config.GetLinkedInDisplayName(),
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                ClientId = config.GetLinkedInClientId(),
                ClientSecret = config.GetLinkedInSecret()
            });

            return app;
        }
    }
}
