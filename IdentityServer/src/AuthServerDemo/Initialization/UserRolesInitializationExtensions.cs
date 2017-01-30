using AuthServerDemo.Configuration;
using AuthServerDemo.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace AuthServerDemo.Initialization
{
    public static class UserRolesInitializationExtensions
    {
        public static void InitRoles(this IApplicationBuilder app)
        {
            var roleManager = (RoleManager<ApplicationRole>)app.ApplicationServices.GetService(typeof(RoleManager<ApplicationRole>));

            CreateIfDoesNotExist(roleManager, Roles.Admin);
            CreateIfDoesNotExist(roleManager, Roles.User);
        }

        private static void CreateIfDoesNotExist(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            if(!roleManager.RoleExistsAsync(roleName).Result)
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                var result = roleManager.CreateAsync(role).Result;
            }            
        }
    }
}