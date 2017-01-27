using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using AuthServerDemo.Services;
using AuthServerDemo.Models;
using AuthServerDemo.Attributes;
using IdentityServer4;
using AuthServerDemo.Data.Entities;
using Microsoft.Extensions.Logging;
using AuthServerDemo.Data.Stores;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthServerDemo.Configuration;

namespace AuthServerDemo.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly AccountService _account;

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IIdentityServerInteractionService interaction, IClientStore clientStore, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> identityUserManager, SignInManager<ApplicationUser> identitySignInManager)
        {
            _interaction = interaction;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);

            this.userManager = identityUserManager;
            this.signInManager = identitySignInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Address = model.Address,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            return View(model);
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var loginViewModel = await _account.BuildLoginViewModelAsync(returnUrl);

            if (loginViewModel.IsExternalLoginOnly)
            {
                // only one option for logging in
                return await ExternalLogin(loginViewModel.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return View(loginViewModel);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
                }
            }

            var loginViewModel = await _account.BuildLoginViewModelAsync(model);
            return View(loginViewModel);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var logOutModel = await _account.BuildLogoutViewModelAsync(logoutId);

            if (logOutModel.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(logOutModel);
            }

            return View(logOutModel);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var logedOutModel = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (logedOutModel.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = logedOutModel.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(logedOutModel.ExternalAuthenticationScheme, new AuthenticationProperties { RedirectUri = url });
                }
                catch(NotSupportedException) // this is for the external providers that don't have signout
                {
                }
                catch(InvalidOperationException) // this is for Windows/Negotiate
                {
                }
            }

            // delete local authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            return View("LoggedOut", logedOutModel);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            returnUrl = Url.Action("ExternalLoginCallback", new { returnUrl = returnUrl });

            // windows authentication is modeled as external in the asp.net core authentication manager, so we need special handling
            if (AccountOptions.WindowsAuthenticationSchemes.Contains(provider))
            {
                // but they don't support the redirect uri, so this URL is re-triggered when we call challenge
                if (HttpContext.User is WindowsPrincipal)
                {
                    var props = new AuthenticationProperties();
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationProviderName);

                    var id = new ClaimsIdentity(provider);

                    id.AddClaim(new Claim(ClaimTypes.NameIdentifier, HttpContext.User.Identity.Name));
                    id.AddClaim(new Claim(ClaimTypes.Name, HttpContext.User.Identity.Name));

                    await HttpContext.Authentication.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, new ClaimsPrincipal(id), props);
                    return Redirect(returnUrl);
                }
                else
                {
                    return new ChallengeResult(AccountOptions.WindowsAuthenticationSchemes);
                }
            }
            else
            {
                var props = new AuthenticationProperties
                {
                    RedirectUri = returnUrl,
                    Items = { { "scheme", provider } }
                };

                return new ChallengeResult(provider, props);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            var user = this.userManager.Users.FirstOrDefault(q => q.ProviderName == provider && q.ProviderSubjectId == userId);

            if (user == null)
            {
                user = await this.AutoProvisionUserAsync(provider, userId, claims);
            }

            var additionalClaims = new List<Claim>();
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            AuthenticationProperties props = null;
            var id_token = info.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }

            await HttpContext.Authentication.SignInAsync(user.Id.ToString(), user.UserName, provider, props, additionalClaims.ToArray());
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (_interaction.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> BecomeAdmin()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if(user != null)
            {
                var isAdmin = await userManager.IsInRoleAsync(user, Roles.Admin);
                if(!isAdmin)
                {
                    var result = await userManager.AddToRoleAsync(user, Roles.Admin);
                    if(result.Succeeded)
                    {
                        await signInManager.SignOutAsync();
                    }
                }                
            }

            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string userId, List<Claim> claims)
        {
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                else
                {
                    filtered.Add(claim);
                }
            }

            var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
            var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;

            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            var email = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value;

            var newUser = new ApplicationUser()
            {
                UserName = email,
                FirstName = first,
                LastName = last,
                Email = email,
                ProviderName = provider,
                ProviderSubjectId = userId
            };

            foreach (var claim in filtered)
            {
                newUser.Claims.Add(new IdentityUserClaim<int>() {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }

            var user = await this.userManager.CreateAsync(newUser);

            if (user.Succeeded)
            {
                return newUser;
            }

            throw new Exception("Provision user failed");
        }
    }
}