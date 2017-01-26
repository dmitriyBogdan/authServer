using AuthServerDemo.Models.Client;
using IdentityServer4.EntityFramework.Entities;
using System.Threading.Tasks;

namespace AuthServerDemo.Services.Interfaces
{
    public interface IClientService
    {
        Task<Client> CreateAsync(RegisterClientModel client);
    }
}