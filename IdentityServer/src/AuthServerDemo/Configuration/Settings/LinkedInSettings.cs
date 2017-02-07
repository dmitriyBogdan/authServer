using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AuthServerDemo.Configuration.Settings
{
    public static class LinkedInSettings
    {
        private const string ROOT_SECTION = "LinkedIn";

        private const string CLIENT_ID = "ClientId";
        private const string SECRET = "Secret";
        private const string AUTH_SCHEME = "AuthScheme";
        private const string DISPLAY_NAME = "DisplayName";

        private const string PATTERN = "{0}:{1}";

        public static string GetLinkedInClientId(this IConfiguration config)
        {
            return config.GetSection(string.Format(PATTERN, ROOT_SECTION, CLIENT_ID)).Value;
        }

        public static string GetLinkedInSecret(this IConfiguration config)
        {
            return config.GetSection(string.Format(PATTERN, ROOT_SECTION, SECRET)).Value;
        }

        public static string GetLinkedInAuthScheme(this IConfiguration config)
        {
            return config.GetSection(string.Format(PATTERN, ROOT_SECTION, AUTH_SCHEME)).Value;
        }

        public static string GetLinkedInDisplayName(this IConfiguration config)
        {
            return config.GetSection(string.Format(PATTERN, ROOT_SECTION, DISPLAY_NAME)).Value;
        }
    }
}