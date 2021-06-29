using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Json.Net;
using StackExchange.Redis;
using ServiceStack.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Emalk_Yorumlari_Redis
{
    public class RedisManager
    {

        private static string host = "localhost";
        private static string port = "6379";
        private static string connString = "";
        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = { "localhost:6379" }

        });
        public IDatabase db = redis.GetDatabase();

        public RedisManager(string host, string port)
        {
            connString = host + ":" + port;

        }

        public RedisManager()
        {
            
        }
  


        

        public bool IsSet(RedisKey key)
        {
            return db.KeyExists(key);
        }

        public bool updateKey(RedisKey key, RedisValue data, int cacheTime = 0)
        {
            db.SetAdd(key, data);
            var setTime = TimeSpan.FromMinutes(cacheTime);
            if (cacheTime > 0)
            {
                db.KeyExpire(key, setTime);
            }

            return true;

        }

        public bool setKey(RedisKey key, RedisValue data, int cacheTime = 0)
        {
            if (IsSet(key))
            {
                return false;
            }

            db.SetAdd(key, data);
            
            var setTime = TimeSpan.FromMinutes(cacheTime);
            if (cacheTime > 0)
            {
                db.KeyExpire(key, setTime);
            }

            return true;

        }
        
        public string getKey(RedisKey key)
        {
            var deneme = db.SetMembers(key);
            var result = deneme.ToStringArray();
            string value = result[0];
            return value;
        }

        public bool Remove(RedisKey key)
        {
            return db.KeyDelete(key);
        }

    }
}
