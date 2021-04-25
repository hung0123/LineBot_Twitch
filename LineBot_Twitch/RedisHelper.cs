using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineBot_Twitch
{
    public sealed class RedisHelper
    {

        private static Lazy<RedisHelper> lazy = new Lazy<RedisHelper>(() =>
        {
            if (String.IsNullOrEmpty(_settingOption)) throw new InvalidOperationException("Please call Init() first.");
            return new RedisHelper();
        });

        private static string _settingOption;

        public readonly ConnectionMultiplexer ConnectionMultiplexer;

        public static RedisHelper Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public RedisHelper()
        {
            ConnectionMultiplexer = ConnectionMultiplexer.Connect(_settingOption);
        }

        public static void Init(string settingOption)
        {
            _settingOption = settingOption;
        }

        public string GetData(string key,int db=-1)
        {
            var result = "";
            var redisDB = ConnectionMultiplexer.GetDatabase(db);
            result = redisDB.StringGet(key);

            return result;
        }

        public bool SetData(string key, string value ,TimeSpan? expire = null,int db=-1)
        {
            var result = false;
            var redisDB = ConnectionMultiplexer.GetDatabase(db);
            result = redisDB.StringSet(key,value, expire);

            return result;
        }
    }
}
