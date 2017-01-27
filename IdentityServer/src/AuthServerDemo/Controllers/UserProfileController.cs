using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthServerDemo.Models.UserProfile;
using Microsoft.AspNetCore.Identity;
using AuthServerDemo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using AuthServerDemo.Configuration;
using AuthServerDemo.Data;
using System;
using AuthServerDemo.Data.Stores;
using System.Security.Claims;
using IdentityServer4;

namespace AuthServerDemo.Controllers
{
    [Route("api/profile")]
    [Produces("application/json")]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IApplicationUserStore inMemoryUsers;

        public UserProfileController(UserManager<ApplicationUser> identityUserManager, IApplicationUserStore inMemoryStore)
        {
            userManager = identityUserManager;
            inMemoryUsers = inMemoryStore;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(string email)
        {
            try
            {          
                var user = await inMemoryUsers.FindByUsernameAsync(EmailToSearch(User, email));
                return Ok(new
                            {
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Address = user.Address,
                    // The data is from memory or redis. We need to change the source first and admin determination then
                    //IsAdmin = user.IsAdmin
                });
            }
            catch (InvalidOperationException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Address = model.Address,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        // Do we need this method? I guess we have UI for it
                        //IsAdmin = model.IsAdmin
                    };

                var result = await userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    var createdUser = userManager.Users.First(q => q.UserName == user.UserName);
                    await inMemoryUsers.UsersRepository.AddAsync(createdUser);
                    return Ok(model);
                }
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(string email, [FromBody]UserProfileModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(email))
            {
                var user = userManager.Users.AsQueryable().FirstOrDefault(ApplicationUserQueries.GetUserByEmailWithRoleRestrictionsQuery(User, email));

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;

                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        await inMemoryUsers.UsersRepository.UpdateAsync(user);

                        return Ok(new {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Address = user.Address,
                            // Do we need this method? I guess we have UI for it
                            //IsAdmin = user.IsAdmin
                        });
                    }                
                }
            }

            return BadRequest(ModelState);
        }

        [Authorize(Roles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = userManager.Users.FirstOrDefault(ApplicationUserQueries.GetUserByEmailQuery(email));

                if (user != null)
                {
                    var result = await userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        await inMemoryUsers.UsersRepository.DeleteAsync(user);

                        return Ok();
                    }
                }
            }

            return BadRequest();
        }

        private string EmailToSearch(ClaimsPrincipal user, string email)
        {
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == IdentityServerConstants.StandardScopes.Email).Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return userEmail;
            }
            else
            {
                if (User.IsInRole(Roles.Admin) || (userEmail == email))
                {
                    return email;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}