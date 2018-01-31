using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;

namespace PlusGame.Config
{
    public class AppConfig
    {
        /// <summary>
        /// 游戏唯一标识
        /// </summary>
        public int GameId { get; set; }
        /// <summary>
        /// 服务器唯一标识
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// 服务器类型
        /// </summary>
        public int ServerType { get; set; }
        /// <summary>
        /// md5 key
        /// </summary>
        public string SignKey { get; set; }
        /// <summary>
        /// DES key
        /// </summary>
        public string ClientDesDeKey { get; set; }
    }

    public class RedisConfig
    {
        public string PreRedisKey { get; set; }
        public string RedisHost { get; set; }
        public int RedisDB { get; set; }
    }

    /// <summary>
    /// 内网通讯
    /// </summary>
    public class ActorConfig
    {
        /// <summary>
        /// Actor管理服 Location服不用配置
        /// </summary>
        public string LocationIPAddress { get; set; }
        public int LocationPort { get; set; }
        public string LocationActorName { get; set; }

        /// <summary>
        /// 本地Actor
        /// </summary>
        public string ActorIPAddress { get; set; }
        public int ActorPort { get; set; }
    }

    /// <summary>
    /// 外网Socket配置
    /// </summary>
    public class SocketConfig
    {
        public SocketConfig()
        {
            IPAddress = "127.0.0.1";
            Port = 9001;
            Backlog = 1000;
            EnableActionGZip = true;
            ActionGZipOutLength = 1024;//1k
            PrePackageLength = 4;
            ByteOrder = ByteOrder.LittleEndian;
        }

        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int Backlog { get; set; }
        public int PrePackageLength { get; set; }
        public ByteOrder ByteOrder { get; set; }

        public bool EnableActionGZip { get; set; }
        public int ActionGZipOutLength { get; set; }
    }

    /// <summary>
    /// Http配置
    /// </summary>
    public class HttpConfig
    {
        public HttpConfig()
        {
            HttpHost = "http://127.0.0.1";
            HttpPort = 80;
            HttpName = "Service.aspx";
            HttpTimeout = 120000;
        }

        public string HttpHost { get; set; }
        public int HttpPort { get; set; }
        public string HttpName { get; set; }
        public int HttpTimeout { get; set; }
    }

    
}
