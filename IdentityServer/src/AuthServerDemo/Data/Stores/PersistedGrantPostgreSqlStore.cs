using System.Collections.Generic;
using System.Threading.Tasks;
using AuthServerDemo.Data.Repository;
using IdentityServer4.Stores;
using PersistedGrantEntity = IdentityServer4.EntityFramework.Entities.PersistedGrant;
using PersistedGrantModel = IdentityServer4.Models.PersistedGrant;

namespace AuthServerDemo.Data.Stores
{
    public class PersistedGrantPostgreSqlStore : IPersistedGrantStore
    {
        private readonly IGrantRepository repository;

        public PersistedGrantPostgreSqlStore(IGrantRepository repository)
        {
            this.repository = repository;
        }

        public async Task StoreAsync(PersistedGrantModel grant)
        {
            await this.repository.AddAsync(this.ModelToEntity(grant));
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await this.repository.RemoveAllAsync(subjectId, clientId, type);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await this.repository.RemoveAllAsync(subjectId, clientId, null);
        }

        public async Task RemoveAsync(string key)
        {
            await this.repository.RemoveAsync(key);
        }

        public async Task<IEnumerable<PersistedGrantModel>> GetAllAsync(string subjectId)
        {
            var persistedGrantsList = new List<PersistedGrantModel>();
            var entities = await this.repository.GetBySubjectAsync(subjectId);
            foreach (var entity in entities)
            {
                persistedGrantsList.Add(this.EntityToModel(entity));
            }

           return persistedGrantsList;
        }

        public async Task<PersistedGrantModel> GetAsync(string key)
        {
            return this.EntityToModel(await this.repository.GetByKeyAsync(key));
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