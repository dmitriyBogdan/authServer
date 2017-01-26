using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class RegisterClientModel
    {
        [Required]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }

        [Required]
        [Display(Name = "LogOut Redirect Uri")]
        public string LogOutRedirectUri { get; set; }

        [Required]
        [Display(Name = "Grant Type")]
        public string GrantType { get; set; }

        public List<SelectListItem> GrantTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "ClientCredentials", Text = "ClientCredentials" },
            new SelectListItem { Value = "Code", Text = "Code" },
            new SelectListItem { Value = "CodeAndClientCredentials", Text = "CodeAndClientCredentials"  },
            new SelectListItem { Value = "CodeAndClientCredentials", Text = "CodeAndClientCredentials"  },
            new SelectListItem { Value = "Hybrid", Text = "Hybrid"  },
            new SelectListItem { Value = "HybridAndClientCredentials", Text = "HybridAndClientCredentials"  },
            new SelectListItem { Value = "Implicit", Text = "Implicit"  },
            new SelectListItem { Value = "ImplicitAndClientCredentials", Text = "ImplicitAndClientCredentials"  },
            new SelectListItem { Value = "ResourceOwnerPassword", Text = "ResourceOwnerPassword"  },
            new SelectListItem { Value = "ResourceOwnerPasswordAndClientCredentials", Text = "ResourceOwnerPasswordAndClientCredentials"  }
    };

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}
