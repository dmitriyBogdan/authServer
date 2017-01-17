﻿using AuthServerDemo.Data.Stores;
using IdentityModel;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace AuthServerDemo.Services
{
    public class InMemoryUsersPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IInMemoryApplicationUserStore users { get; set; }

        public InMemoryUsersPasswordValidator(IInMemoryApplicationUserStore usersStore)
        {
            this.users = usersStore;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (await users.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                var user = users.FindByUsername(context.UserName);
                context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}