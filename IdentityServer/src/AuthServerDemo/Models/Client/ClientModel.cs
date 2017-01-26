using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class ClientModel
    {
        [Required]
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        [Required]
        public string Secret { get; set; }

        public string RedirectUri { get; set; }

        public string LogOutRedirectUri { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}