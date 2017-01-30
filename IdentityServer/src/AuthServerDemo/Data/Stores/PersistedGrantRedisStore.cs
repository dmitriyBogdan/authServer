using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using AuthServerDemo.Data.Repository;
using PersistedGrantEntity = IdentityServer4.EntityFramework.Entities.PersistedGrant;
using PersistedGrantModel = IdentityServer4.Models.PersistedGrant;
using System.Linq;

namespace AuthServerDemo.Data.Stores
{
    public class PersistedGrantRedisStore : IPersistedGrantStore
    {
        public IGrantRepository Tokens { get; private set; }

        public PersistedGrantRedisStore(IGrantRepository tokenConnections)
        {
            this.Tokens = tokenConnections;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await Tokens.AddAsync(this.ModelToEntity(grant));
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            try
            {
                return this.EntityToModel(await Tokens.GetByKeyAsync(key));
            }
            catch
            {
                return await Task.FromResult<PersistedGrant>(null);
            }
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var entities = await Tokens.GetBySubjectAsync(subjectId);
            return entities.Select(this.EntityToModel);
        }

        public async Task RemoveAsync(string key)
        {
            await this.Tokens.RemoveAsync(key);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await this.Tokens.RemoveAllAsync(subjectId, clientId, null);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await this.Tokens.RemoveAllAsync(subjectId, clientId, type);
        }

        private PersistedGrantModel EntityToModel(PersistedGrantEntity entity)
        {
            var model = new PersistedGrantModel
            {
                Key = entity.Key,
                ClientId = entity.ClientId,
                CreationTime = entity.CreationTime,
                Data = entity.Data,
                Expiration = entity.Expiration,
                SubjectId = entity.SubjectId,
                Type = entity.Type
            };

            return model;
        }

        private PersistedGrantEntity ModelToEntity(PersistedGrantModel model)
        {
            var entity = new PersistedGrantEntity
            {
                Key = model.Key,
                ClientId = model.ClientId,
                CreationTime = model.CreationTime,
                Data = model.Data,
                Expiration = model.Expiration,
                SubjectId = model.SubjectId,
                Type = model.Type
            };

            return entity;
        }
    }
}