using AuthServerDemo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServerDemo.Data.Repository
{
    public interface IApplicationUserRepository
    {
        Task AddAsync(ApplicationUser user);

        Task AddRangeAsync(IEnumerable<ApplicationUser> users);

        Task UpdateAsync(ApplicationUser user);

        Task DeleteAsync(ApplicationUser user);

        Task<ApplicationUser> GetByIdAsync(int id);

        Task<ApplicationUser> GetByUserNameAsync(string userName);
    }
}