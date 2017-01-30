using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthServerDemo.Data.Repository
{
    public class GrantPostgreSqlRepository : IGrantRepository
    {
        private PersistedGrantDbContext dbContext;

        public GrantPostgreSqlRepository(PersistedGrantDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(PersistedGrant token)
        {
            await this.dbContext.PersistedGrants.AddAsync(token);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<PersistedGrant> tokens)
        {
            foreach (var token in tokens)
            {
                await this.dbContext.PersistedGrants.AddAsync(token);
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrantsToRemove = this.dbContext.PersistedGrants
                                            .Where(x => x.SubjectId == subjectId && x.ClientId == clientId);
            if (!string.IsNullOrEmpty(type))
            {
                persistedGrantsToRemove = persistedGrantsToRemove.Where(x => x.Type == type);
            }

           this.dbContext.PersistedGrants.RemoveRange(persistedGrantsToRemove);
           await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string key)
        {
            var persistedGratnToDelete = this.dbContext.PersistedGrants.FirstOrDefault(x => x.Key == key);
            if (persistedGratnToDelete != null)
            {
                this.dbContext.PersistedGrants.Remove(persistedGratnToDelete);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<PersistedGrant> GetByKeyAsync(string key)
        {
            return await this.dbContext.PersistedGrants.FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task<IEnumerable<PersistedGrant>> GetBySubjectAsync(string subject)
        {
            return await this.dbContext.PersistedGrants.Where(x => x.SubjectId == subject).ToListAsync();
        }
    }
}
