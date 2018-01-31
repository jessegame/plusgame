using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PlusCommon.Log;
using PlusGame.Contract;
using PlusGame.IO;

namespace PlusGame.Config
{
    public static class GameEnvironment
    {
        private readonly static string configPath = "config";

        private static AppConfig appConfig = new AppConfig();
        private static RedisConfig redisConfig = new RedisConfig();
        private static SocketConfig socketConfig = new SocketConfig();
        private static HttpConfig httpConfig = new HttpConfig();
        private static ActorConfig actorConfig = new ActorConfig();

        private static IActionDispatcher actionDispatcher;

        static GameEnvironment()
        {
            try
            {
                string appConfigPath = string.Format("{0}/{1}", configPath, "AppConfig.txt");
                string redisConfigPath = string.Format("{0}/{1}", configPath, "RedisConfig.txt");
                string socketConfigPath = string.Format("{0}/{1}", configPath, "SocketConfig.txt");
                string httpConfigPath = string.Format("{0}/{1}", configPath, "HttpConfig.txt");
                string actorConfigPath = string.Format("{0}/{1}", configPath, "ActorConfig.txt");

                appConfig = ReadConfig<AppConfig>(appConfigPath);
                redisConfig = ReadConfig<RedisConfig>(redisConfigPath);
                socketConfig = ReadConfig<SocketConfig>(socketConfigPath);
                httpConfig = ReadConfig<HttpConfig>(httpConfigPath);
                actorConfig = ReadConfig<ActorConfig>(actorConfigPath);

                MessageStructure.EnableGzip = socketConfig.EnableActionGZip;
                MessageStructure.EnableGzipMinByte = socketConfig.ActionGZipOutLength;

                actionDispatcher = new PlusDispatcher();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("读取配置文件失败,{0},{1}", ex.Message, ex.StackTrace);
            }
        }

        private static T ReadConfig<T>(string path) where T : new()
        {
            if (File.Exists(path))
            {
                var configTxt = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(configTxt);
            }
            return new T();
        }

        public static AppConfig AppConfig { get { return appConfig; } }
        public static RedisConfig RedisConfig { get { return redisConfig; } }
        public static SocketConfig SocketConfig { get { return socketConfig; } }
        public static HttpConfig HttpConfig { get { return httpConfig; } }
        public static ActorConfig ActorConfig { get { return actorConfig; } }

        public static IActionDispatcher ActionDispatcher { get { return actionDispatcher; } set { actionDispatcher = value; } }
    }
}
