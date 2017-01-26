using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace AuthServerDemo.Initialization
{
    public class FakeDataConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("userscope", new [] { "role", "admin", "user" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("users")
                {
                    ApiSecrets = {
                        new Secret("secret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "userscope",
                            DisplayName = "Scope for user management"
                        }
                    },
                    UserClaims = { "role", "admin", "user" }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin@mail.com",
                    Password = "qweQWE123!"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "user@mail.com",
                    Password = "qweQWE123!"
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5001/signin-oidc" },

                    PostLogoutRedirectUris = { "http://localhost:5001" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientName = "demo",
                    ClientId = "demo",
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RequireConsent = false,
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "users",
                        "userscope",
                        "role"
                    }
                },
                new Client
                {
                        ClientId = "rfClient",
                        ClientName = "Refresh token test",
                        AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        RedirectUris           = { "http://localhost:5001/signin-oidc" },
                        PostLogoutRedirectUris = { "http://localhost:5001" },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "api1",
                            "users"
                        },
                        AllowOfflineAccess = true
                }
            };
        }
    }
}