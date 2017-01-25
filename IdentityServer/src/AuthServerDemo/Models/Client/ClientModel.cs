﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthServerDemo.Models.Client
{
    public class ClientModel
    {
        public string ClientId { get; set; }

        [Required]
        public string ClientName { get; set; }

        public string Secret { get; set; }

        [Required]
        public string RedirectUri { get; set; }

        [Required]
        public string LogOutRedirectUri { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }
    }
}
