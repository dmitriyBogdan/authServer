﻿using AuthServerDemo.Models.Client;
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
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            clientDb.LogoutUri = client.LogOutRedirectUri;

            var savedClient = (await context.AddAsync(clientDb)).Entity;
            await context.SaveChangesAsync();

            savedClient.ClientSecrets = new List<ClientSecret>
                                        {
                                            new ClientSecret
                                            {
                                                Description = secret.Description,
                                                Expiration = secret.Expiration,
                                                Type = secret.Type,
                                                Value = secret.Value,
                                                Client = savedClient
                                            }
                                        };
            savedClient.RedirectUris = new List<ClientRedirectUri>()
                                        {
                                            new ClientRedirectUri
                                            {
                                                RedirectUri = client.RedirectUri,
                                                Client = savedClient
                                            }
                                        };
            savedClient.AllowedGrantTypes = new List<ClientGrantType>
                                            {
                                                new ClientGrantType
                                                {
                                                    GrantType = client.GrantType,
                                                    Client = savedClient
                                                }
                                            };

            await context.SaveChangesAsync();

            return savedClient;
        }

        public async Task<Client[]> GetAllAsync()
        {
            return await context.Clients.ToArrayAsync();
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await context.Clients
                                .Include(x => x.ClientSecrets)
                                .Include(x => x.RedirectUris)
                                .Include(x => x.AllowedGrantTypes)
                                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var client = await context.Clients.FirstOrDefaultAsync(x => x.Id == id);
            if(client != null)
            {
                context.Clients.Remove(client);
                await context.SaveChangesAsync();
            }            
        }
    }
}