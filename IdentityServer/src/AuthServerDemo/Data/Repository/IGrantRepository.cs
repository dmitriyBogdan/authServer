using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthServerDemo.Data.Repository
{
    public interface IGrantRepository
    {
        Task AddAsync(PersistedGrant token);

        Task AddRangeAsync(IEnumerable<PersistedGrant> tokens);

        Task RemoveAsync(string subjectId, string clientId, string type);

        Task RemoveAsync(string key);

        Task<PersistedGrant> GetByKeyAsync(string key);

        Task<IEnumerable<PersistedGrant>> GetBySubjectAsync(string subject);
    }
}