using AuthServerDemo.Models.Client;
using AuthServerDemo.Services.Interfaces;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using System;
using System.Threading.Tasks;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientSecret = IdentityServer4.EntityFramework.Entities.ClientSecret;

namespace AuthServerDemo.Services
{
    public class ClientService : IClientService
    {
        private readonly ConfigurationDbContext context;

        public ClientService(ConfigurationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(RegisterClientModel client)
        {
            var secret = new Secret(client.Secret.Sha256());

            var clientDb = new Client
            {
                ClientId = Guid.NewGuid().ToString().Replace("-", string.Empty),
                ClientSecrets = 
                {
                    new ClientSecret
                    {
                        Description = secret.Description,
                        Expiration = secret.Expiration,
                        Type = secret.Type,
                        Value = secret.Value
                    }
                }
            };

            await context.AddAsync(client);
        }
    }
}