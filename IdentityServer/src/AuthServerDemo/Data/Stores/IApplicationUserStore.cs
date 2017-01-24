using AuthServerDemo.Data.Entities;
using AuthServerDemo.Data.Repository;
using System.Threading.Tasks;

namespace AuthServerDemo.Data.Stores
{
    public interface IApplicationUserStore
    {
        IApplicationUserRepository UsersRepository { get; }

        Task<bool> ValidateCredentialsAsync(string username, string password);

        Task<ApplicationUser> FindByUsernameAsync(string username);

        Task<ApplicationUser> FindByIdAsync(int id);
    }
}