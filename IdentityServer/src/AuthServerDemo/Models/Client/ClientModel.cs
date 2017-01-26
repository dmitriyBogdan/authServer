using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class ClientModel
    {
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public string Secret { get; set; }

        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }

        [Display(Name = "LogOut Redirect Uri")]
        public string LogOutRedirectUri { get; set; }

        [Display(Name = "Grant Type")]
        public string GrantType { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}
