using StackExchange.Redis;

namespace AuthServerDemo.Data
{
    /// <summary>
    /// Represents Redis connection
    /// </summary>
    public class RedisConnection
    {
        public IDatabase Database { get; private set; }

        public RedisConnection(string host)
        {
            Database = ConnectionMultiplexer.Connect(host).GetDatabase();
        }
    }
}