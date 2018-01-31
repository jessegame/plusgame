using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using PlusCommon.Log;

namespace PlusGame.Session
{
    public class GameSession
    {
        private IChannelHandlerContext _context;

        #region property

        /// <summary>
        /// Remote end address
        /// </summary>
        public string RemoteAddress { get; private set; }

        /// <summary>
        /// SessionId
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActivityTime { get; private set; }

        #endregion

        public GameSession(string sessionId, IChannelHandlerContext context)
        {
            _context = context;
            RemoteAddress = context.Channel.RemoteAddress.ToString();
            SessionId = sessionId;
            Refresh();
        }

        /// <summary>
        /// 刷新心跳时间
        /// </summary>
        internal void Refresh()
        {
            LastActivityTime = DateTime.Now;
        }

        public void PostSend(IByteBuffer data)
        {
            _context.WriteAsync(data);
        }
    }
}
