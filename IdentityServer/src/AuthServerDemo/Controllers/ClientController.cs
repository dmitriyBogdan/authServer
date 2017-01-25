using AuthServerDemo.Attributes;
using AuthServerDemo.Models.Client;
using AuthServerDemo.Services.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterClientModel client)
        {
            if (ModelState.IsValid)
            {
                await clientService.CreateAsync(client);

                RedirectToAction(nameof(ProfileInfo));
            }

            // IdentityServer4.Models.GrantTypes
            return View(client);
        }

        [HttpGet]
        public IActionResult ProfileInfo()
        {
            return View();
        }
    }
}
