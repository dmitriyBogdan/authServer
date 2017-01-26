using AuthServerDemo.Attributes;
using AuthServerDemo.Models.Client;
using AuthServerDemo.Services.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServerDemo.Controllers
{
    [SecurityHeaders]
    public class ClientController : Controller
    {
        private readonly IClientService clientService;

        public ClientController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterClientModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterClientModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await clientService.CreateAsync(model);

                var info = new ClientModel
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    Secret = client.ClientSecrets.First().Value,
                    RedirectUri = client.RedirectUris.First().RedirectUri,
                    LogOutRedirectUri = client.LogoutUri,
                    GrantType = client.AllowedGrantTypes.First().GrantType
                };
                return RedirectToAction(nameof(ProfileInfo), info);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ProfileInfo(ClientModel model)
        {
            return View(model);
        }
    }
}
