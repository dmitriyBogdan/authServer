using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class ClientModel
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public string RedirectUri { get; set; }

        public string LogOutRedirectUri { get; set; }

        public string GrantType { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}
