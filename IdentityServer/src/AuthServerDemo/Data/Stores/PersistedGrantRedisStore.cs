//using IdentityServer4.Stores;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using IdentityServer4.Models;
//using AuthServerDemo.Data.Repository;

//namespace AuthServerDemo.Data.Stores
//{
//    public class PersistedGrantRedisStore : IPersistedGrantStore
//    { 
//        public IGrantRepository Tokens { get; private set; }

//        public PersistedGrantRedisStore(IGrantRepository tokenConnections)
//        {
//            this.Tokens = tokenConnections;
//        }

//        public async Task StoreAsync(PersistedGrant grant)
//        {
//            await Tokens.AddAsync(grant);
//        }

//        public async Task<PersistedGrant> GetAsync(string key)
//        {
//            try
//            {
//                return await Tokens.GetByKeyAsync(key);
//            }
//            catch
//            {
//                return await Task.FromResult<PersistedGrant>(null);
//            }
//        }

//        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
//        {
//            return await Tokens.GetBySubjectAsync(subjectId);
//        }

//        public async Task RemoveAsync(string key)
//        {
//            await this.Tokens.RemoveAsync(key);
//        }

//        public async Task RemoveAllAsync(string subjectId, string clientId)
//        {
//            await this.Tokens.RemoveAsync(subjectId, clientId, null);
//        }

//        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
//        {
//            await this.Tokens.RemoveAsync(subjectId, clientId, type);
//        }
//    }
//}