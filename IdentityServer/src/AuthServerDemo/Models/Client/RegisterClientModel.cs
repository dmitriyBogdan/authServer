using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class RegisterClientModel
    {
        public string ClientName { get; set; }

        public string Secret { get; set; }

        [Required]
        public string RedirectUri { get; set; }

        [Required]
        public string LogOutRedirectUri { get; set; }

        [Required]
        public string GrantType { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}
