using AuthServerDemo.Attributes;
using AuthServerDemo.Configuration;
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
    [Authorize(Roles.Admin)]
    public class ClientController : Controller
    {
        private readonly IClientService clientService;

        public ClientController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clients = await clientService.GetAllAsync();
            var model = clients.Select(x => new ShortClientInfoModel { Id = x.Id, Name = x.ClientName }).ToArray();

            return View(model);
        }

        [HttpGet]        
        public IActionResult Register()
        {
            return View(new RegisterClientModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterClientModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await clientService.CreateAsync(model);

                return RedirectToAction(nameof(ProfileInfo), new { id = client.Id });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileInfo(int id)
        {
            ClientModel model;

            var client = await clientService.GetByIdAsync(id);
            if(client != null)
            {
                model = new ClientModel
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    Secret = client.ClientSecrets?.FirstOrDefault()?.Value,
                    RedirectUri = client.RedirectUris?.FirstOrDefault()?.RedirectUri,
                    LogOutRedirectUri = client.LogoutUri,
                    GrantType = client.AllowedGrantTypes?.Select(x => x.GrantType).Aggregate((i, j) => i + "; " + j)
                };
            }
            else
            {
                model = new ClientModel();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await clientService.DeleteByIdAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
