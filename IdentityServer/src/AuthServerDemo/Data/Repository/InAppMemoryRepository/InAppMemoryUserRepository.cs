using System.Linq;
using System.Threading.Tasks;
using AuthServerDemo.Data.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AuthServerDemo.Data.Repository.InAppMemoryRepository
{
    public class InAppMemoryUserRepository : IApplicationUserRepository
    {
        private ConcurrentDictionary<string, ApplicationUser> InMemoryUsers { get; set; }

        public InAppMemoryUserRepository()
        {
            this.InMemoryUsers = new ConcurrentDictionary<string, ApplicationUser>();
        }

        public Task AddAsync(ApplicationUser user)
        {
            this.InMemoryUsers.TryAdd(user.Id.ToString(), user);
            return Task.FromResult(0);
        }

        public async Task AddRangeAsync(IEnumerable<ApplicationUser> users)
        {
            foreach (ApplicationUser item in users)
            {
                await AddAsync(item); 
            }
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            ApplicationUser resultUser;
            this.InMemoryUsers.TryRemove(user.Id.ToString(), out resultUser);

            return Task.FromResult(0);
        }

        public Task<ApplicationUser> GetByIdAsync(int id)
        {
            return Task.FromResult(this.InMemoryUsers[id.ToString()]);
        }

        public Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            var appUser = this.InMemoryUsers.FirstOrDefault(q => q.Value.UserName == userName).Value;

            return Task.FromResult(appUser);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            this.InMemoryUsers[user.Id.ToString()] = user;
            return Task.FromResult(0);
        }
    }
}