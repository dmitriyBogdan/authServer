using IdentityServer4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServerDemo.Data.Repository
{
    /// <summary>
    /// Persisted grant data access point for redis database 
    /// </summary>
    public class GrantRedisRepository : IGrantRepository
    {
        private const string TOKEN = "TOKEN";
        private const string SUBJECT = "SUBJECT";

        private RedisConnection connection;

        public GrantRedisRepository(RedisConnection redisCconnection)
        {
            connection = redisCconnection;
        }

        public async Task AddAsync(PersistedGrant token)
        {
            await connection.Database.StringSetAsync(TOKEN + token.Key, JsonConvert.SerializeObject(token));
            await AddSubject(token);
        }

        private async Task AddSubject(PersistedGrant token)
        {
            var tokensWithSubject = (await GetBySubjectAsync(token.SubjectId)).ToList();
            tokensWithSubject.Add(token);

            var serTokens = JsonConvert.SerializeObject(tokensWithSubject);
            await connection.Database.StringSetAsync(SUBJECT + token.SubjectId, serTokens);
        }

        public async Task AddRangeAsync(IEnumerable<PersistedGrant> tokens)
        {
            foreach (PersistedGrant token in tokens)
            {
                await this.AddAsync(token);
            }
        }

        public async Task RemoveAsync(string key)
        {
            await connection.Database.KeyDeleteAsync(TOKEN + key);
        }

        public async Task RemoveAsync(string subjectId, string clientId, string type)
        {
            var allSubjects = await GetBySubjectAsync(subjectId);
            var subjectsToRemove = allSubjects.Where(x => x.SubjectId == subjectId && x.ClientId == clientId);
            if (type != null)
            {
                allSubjects = allSubjects.Where(x => x.Type == type);
            }

            foreach (PersistedGrant grant in subjectsToRemove)
            {
                await connection.Database.KeyDeleteAsync(TOKEN + grant.Key);
            }

            var serTokens = JsonConvert.SerializeObject(allSubjects.Except(subjectsToRemove));
            await connection.Database.StringSetAsync(SUBJECT + subjectId, serTokens);
        }

        public async Task<PersistedGrant> GetByKeyAsync(string key)
        {
            PersistedGrant result = null;

            var value = await connection.Database.StringGetAsync(TOKEN + key);
            if(value.HasValue)
            {
                result = JsonConvert.DeserializeObject<PersistedGrant>(value);
            }

            return result;
        }

        public async Task<IEnumerable<PersistedGrant>> GetBySubjectAsync(string subject)
        {
            List<PersistedGrant> result = new List<PersistedGrant>();

            var value = await connection.Database.StringGetAsync(SUBJECT + subject);
            if (value.HasValue)
            {
                result.AddRange(JsonConvert.DeserializeObject<PersistedGrant[]>(value));
            }

            return result;
        }
    }
}
