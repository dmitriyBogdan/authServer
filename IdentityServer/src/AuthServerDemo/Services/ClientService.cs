using AuthServerDemo.Models.Client;
using AuthServerDemo.Services.Interfaces;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientSecret = IdentityServer4.EntityFramework.Entities.ClientSecret;
using ClientRedirectUri = IdentityServer4.EntityFramework.Entities.ClientRedirectUri;
using ClientGrantType = IdentityServer4.EntityFramework.Entities.ClientGrantType;

namespace AuthServerDemo.Services
{
    public class ClientService : IClientService
    {
        private readonly ConfigurationDbContext context;

        public ClientService(ConfigurationDbContext context)
        {
            this.context = context;
        }

        public async Task<Client> CreateAsync(RegisterClientModel client)
        {
            var secret = new Secret(client.Secret.Sha256());

            var clientDb = new Client();

            clientDb.ClientId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            clientDb.ClientName = client.ClientName;
            clientDb.ClientSecrets = new List<ClientSecret>
                                        {
                                            new ClientSecret
                                            {
                                                Description = secret.Description,
                                                Expiration = secret.Expiration,
                                                Type = secret.Type,
                                                Value = secret.Value
                                            }
                                        };
            clientDb.RedirectUris = new List<ClientRedirectUri>()
                                        {
                                            new ClientRedirectUri { RedirectUri = client.RedirectUri }
                                        };
            clientDb.LogoutUri = client.LogOutRedirectUri;
            clientDb.AllowedGrantTypes = new List<ClientGrantType>
                                            {
                                                new ClientGrantType { GrantType = client.GrantType }
                                            };

            var createdClient = await context.AddAsync(clientDb);
            await context.SaveChangesAsync();

            return createdClient.Entity;
        }
    }
}