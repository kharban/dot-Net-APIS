using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kharban_WebAPI.Helper;
using StackExchange.Redis;

namespace Kharban_WebAPI.Common
{
    public static class RedisConnection 
    {
        //static string connectionString = "localhost:6379,allowAdmin=true";
        static string connectionString = SiteKey.IsLive == "0"? "localhost:6379,allowAdmin=true" : "radis-server-001.3zmmdv.0001.aps1.cache.amazonaws.com:6379,radis-server-002.3zmmdv.0001.aps1.cache.amazonaws.com:6379, abortConnect =false, ssl=false";
        //static string connectionString = System.Configuration.ConfigurationManager.AppSettings["IsLive"].ToString() == "0" ? "13.233.232.38:6379,allowAdmin=true" : "13.233.232.38:6379, abortConnect =false, ssl=false";
        private static void CreateConnection()
        {
            var config = ConfigurationOptions.Parse(connectionString);
            config.AllowAdmin = true;

            RedisConnection.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                //return ConnectionMultiplexer.Connect(connectionString);
                return ConnectionMultiplexer.Connect(config);
            });
            
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static IDatabase Connection
        {
            get
            {
                if (lazyConnection==null)
                {
                    CreateConnection();
                }
                return lazyConnection.Value.GetDatabase();
            }
        }

        public static IServer Server
        {
            get
            {
                if (lazyConnection == null)
                {
                    CreateConnection();
                }
                return lazyConnection.Value.GetServer(lazyConnection.Value.GetEndPoints(true)[0]);
            }
        }
        
        public static void Clear()
        {
            var endpoints = lazyConnection.Value.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = lazyConnection.Value.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }
    }
}