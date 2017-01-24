using AuthServerDemo.Data.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthServerDemo.Data.Repository
{
    /// <summary>
    /// User data access point for redis database
    /// </summary>
    public class ApplicationUserRedisRepository : IApplicationUserRepository
    {
        private const string KEY_ID_SUFFIX = "USER_ID_";
        private const string KEY_NAME_SUFFIX = "USER_NAME_";

        private RedisConnection connection;

        public ApplicationUserRedisRepository(RedisConnection redisCconnection)
        {
            connection = redisCconnection;
        }

        public async Task AddAsync(ApplicationUser user)
        {
            var serializedUser = JsonConvert.SerializeObject(user);

            await connection.Database.StringSetAsync(KEY_ID_SUFFIX + user.Id.ToString(), serializedUser);
            await connection.Database.StringSetAsync(KEY_NAME_SUFFIX + user.UserName.ToUpper(), serializedUser);
        }

        public async Task AddRangeAsync(IEnumerable<ApplicationUser> users)
        {
            foreach (ApplicationUser user in users)
            {
                await this.AddAsync(user);
            }
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await this.AddAsync(user); // Add will just replace the key with new value, it's the same action as update
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await connection.Database.KeyDeleteAsync(KEY_ID_SUFFIX + user.Id.ToString());
            await connection.Database.KeyDeleteAsync(KEY_NAME_SUFFIX + user.UserName.ToUpper());
        }

        public async Task<ApplicationUser> GetByIdAsync(int id)
        {
            ApplicationUser result = null;

            var value = await connection.Database.StringGetAsync(KEY_ID_SUFFIX + id.ToString());
            if (value.HasValue)
            {
                result = JsonConvert.DeserializeObject<ApplicationUser>(value);
            }

            return result;
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            ApplicationUser result = null;

            var value = await connection.Database.StringGetAsync(KEY_NAME_SUFFIX + userName.ToUpper());
            if (value.HasValue)
            {
                result = JsonConvert.DeserializeObject<ApplicationUser>(value);
            }

            return result;
        }
    }
}