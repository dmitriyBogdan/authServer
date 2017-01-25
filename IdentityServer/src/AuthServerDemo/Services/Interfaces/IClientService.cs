using AuthServerDemo.Models.Client;
using System.Threading.Tasks;

namespace AuthServerDemo.Services.Interfaces
{
    public interface IClientService
    {
        Task CreateAsync(RegisterClientModel client);
    }
}