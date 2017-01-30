using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;
using AuthServerDemo.Data.Entities;
using AuthServerDemo.Configuration;

namespace AuthServerDemo.Services
{
    public class IdentityProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory;

        public IdentityProfileService(UserManager<ApplicationUser> identityUserManager, IUserClaimsPrincipalFactory<ApplicationUser> identityClaimsFactory)
        {
            this.userManager = identityUserManager;
            this.claimsFactory = identityClaimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await this.userManager.FindByIdAsync(subject);
            var principal = await this.claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(JwtClaimTypes.Address, user.Address));
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));

            if (await this.userManager.IsInRoleAsync(user, Roles.Admin))
            {
                claims.Add(new Claim(JwtClaimTypes.Role, Roles.Admin));
            }

            claims.Add(new Claim(JwtClaimTypes.Role, Roles.User));
            claims.Add(new Claim(JwtClaimTypes.Scope, InternalScope.Users));
            
            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await this.userManager.FindByIdAsync(subject);

            context.IsActive = user != null;
        }
    }
}