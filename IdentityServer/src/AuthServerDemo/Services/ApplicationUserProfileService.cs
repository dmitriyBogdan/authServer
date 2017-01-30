using IdentityServer4.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using AuthServerDemo.Data.Stores;
using IdentityServer4.Extensions;
using AuthServerDemo.Data.Entities;
using System.Security.Claims;
using IdentityModel;
using AuthServerDemo.Configuration;
using IdentityServer4;

namespace AuthServerDemo.Services
{
    public class ApplicationUserProfileService : IProfileService
    {
        private IApplicationUserStore users { get; set; }

        public ApplicationUserProfileService(IApplicationUserStore usersStore)
        {
            this.users = usersStore;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.RequestedClaimTypes.Any())
            {
                var user = await GetUserBySubjectAsync(context.Subject);

                var claims = new List<Claim>();

                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                claims.Add(new Claim(JwtClaimTypes.Address, user.Address));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));

                // I guess it is not necessary to fix the class as the entire class is out of use. We should delete it
                /*if (user.IsAdmin)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, Roles.Admin));
                }*/

                claims.Add(new Claim(JwtClaimTypes.Role, Roles.User));
                claims.Add(new Claim(JwtClaimTypes.Scope, InternalScope.Users));

                claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

                context.IssuedClaims = claims;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await GetUserBySubjectAsync(context.Subject);
            context.IsActive = user != null;
        }

        private async Task<ApplicationUser> GetUserBySubjectAsync(ClaimsPrincipal sub)
        {
            return await users.FindByIdAsync(int.Parse(sub.GetSubjectId()));
        }
    }
}
