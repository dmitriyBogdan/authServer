using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthServerDemo.Data.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
        
        public bool IsAdmin { get; set; }

        //public string ProviderName { get; set; }

        //public string ProviderSubjectId { get; set; }
    }
}