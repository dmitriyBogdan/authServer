//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using IdentityServer4.Models;
//using System.Collections.Concurrent;

//namespace AuthServerDemo.Data.Repository.InAppMemoryRepository
//{
//    public class InAppMemoryGrantRepository : IGrantRepository
//    {
//        private ConcurrentDictionary<string, PersistedGrant> InMemoryPersistedGrant { get; set; }

//        public InAppMemoryGrantRepository()
//        {
//            this.InMemoryPersistedGrant = new ConcurrentDictionary<string, PersistedGrant>();
//        }

//        public Task AddAsync(PersistedGrant token)
//        {
//            this.InMemoryPersistedGrant.TryAdd(token.Key, token);
//            return Task.FromResult(0);
//        }

//        public async Task AddRangeAsync(IEnumerable<PersistedGrant> tokens)
//        {
//            foreach (PersistedGrant token in tokens)
//            {
//                await this.AddAsync(token);
//            }
//        }

//        public Task<PersistedGrant> GetByKeyAsync(string key)
//        {
//            return Task.FromResult(this.InMemoryPersistedGrant[key]);
//        }

//        public Task<IEnumerable<PersistedGrant>> GetBySubjectAsync(string subject)
//        {
//            var result = from s in this.InMemoryPersistedGrant.Where(q => q.Value.SubjectId == subject)
//                         select s.Value;

//            return Task.FromResult(result);
//        }

//        public Task RemoveAsync(string key)
//        {
//            PersistedGrant grantResult;
//            this.InMemoryPersistedGrant.TryRemove(key, out grantResult);

//            return Task.FromResult(0);
//        }

//        public Task RemoveAsync(string subjectId, string clientId, string type)
//        {
//            var grant = this.InMemoryPersistedGrant.First(q => q.Value.SubjectId == subjectId && q.Value.ClientId == clientId && q.Value.Type == type);
//            PersistedGrant grantResult;
//            this.InMemoryPersistedGrant.TryRemove(grant.Key, out grantResult);

//            return Task.FromResult(0);
//        }
//    }
//}