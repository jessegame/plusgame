using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Messages;
using Newtonsoft.Json;
using PlusCommon;
using PlusCommon.Log;
using PlusGame.Config;
using PlusGame.Session;

namespace PlusGame.Contract
{
    public class SessionManager
    {
        private static ConcurrentDictionary<string, GameSession> _globalSession;
        /// <summary>
        /// UserId与session映射关系
        /// </summary>
        private static ConcurrentDictionary<int, string> _userHash;

        private static Timer clearTime;
        private static int isClearWorking;

        /// <summary>
        /// Heartbeat timeout(sec), default 60s
        /// </summary>
        public static int HeartbeatTimeout { get; set; }

        //超时事件
        public static event Action<GameSession> HeartbeatTimeoutHandle;
        private static void DoHeartbeatTimeout(GameSession session)
        {
            try
            {
                Action<GameSession> handler = HeartbeatTimeoutHandle;
                if (handler != null) handler(session);
            }
            catch (Exception)
            {
            }
        }

        static SessionManager()
        {
            HeartbeatTimeout = 60;//60s
            //clearTime = new Timer(OnClearSession, null, 6000, 60000);

            _globalSession = new ConcurrentDictionary<string, GameSession>();
            _userHash = new ConcurrentDictionary<int, string>();
        }

        /// <summary>
        /// 清除过期Session
        /// </summary>
        /// <param name="state"></param>
        private static void OnClearSession(object state)
        {
            if (Interlocked.CompareExchange(ref isClearWorking, 1, 0) == 0)
            {
                try
                {
                    var sessions = new List<GameSession>();
                    foreach (var pair in _globalSession)
                    {
                        var session = pair.Value;
                        if (session == null) continue;

                        if (HeartbeatTimeout > 0 &&
                            session.LastActivityTime < MathUtils.Now.AddSeconds(-HeartbeatTimeout))
                        {
                            DoHeartbeatTimeout(session);
                            _globalSession.TryRemove(session.SessionId, out session);
                        }
                    }
                }
                catch (Exception er)
                {
                    TraceLog.WriteError("ClearSession error:{0}", er);
                }
                finally
                {
                    Interlocked.Exchange(ref isClearWorking, 0);
                }
            }
        }

        #region 绑定Session 与 session校验

        public static bool Bind(string sessionId, int userId)
        {
            _userHash[userId] = sessionId;
            return true;
        }

        ///// <summary>
        ///// 检查是否登陆
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public static bool CheckLogin(IChannelHandlerContext context, int userId)
        //{
        //    string newSessionId = GetSessionId(context);
        //    string oldSessionId;
        //    if (_userHash.TryGetValue(userId, out oldSessionId) && string.Equals(oldSessionId, newSessionId, StringComparison.OrdinalIgnoreCase))
        //        return true;
        //    return false;
        //}

        public static bool CheckLogin(string sessionId, int userId)
        {
            string oldSessionId;
            if (_userHash.TryGetValue(userId, out oldSessionId) && string.Equals(oldSessionId, sessionId, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        #endregion

        public static string GetSessionId(IChannelHandlerContext context)
        {
            string keyCode = context.GetHashCode().ToNotNullString();
            return string.Format("s_{0}_{1}_{2}", keyCode, GameEnvironment.AppConfig.GameId, GameEnvironment.AppConfig.ServerId);
        }

        #region 创建Session

        /// <summary>
        /// Add session to cache
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="socket"></param>
        /// <param name="appServer"></param>
        public static GameSession CreateNew(IChannelHandlerContext context)
        {
            string sessionId = GetSessionId(context);
            GameSession session = new GameSession(sessionId, context);
            _globalSession[sessionId] = session;
            return session;
        }

        #endregion

        #region 获取Session

        public static int GetCount()
        {
            return _globalSession.Values.Count;
        }

        /// <summary>
        /// Get session by context.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GameSession Get(IChannelHandlerContext context)
        {
            GameSession session;
            string key = GetSessionId(context);
            return _globalSession.TryGetValue(key, out session) ? session : null;
        }

        public static GameSession Get(int userId)
        {
            GameSession session;
            string sessionId;
            if (_userHash.TryGetValue(userId, out sessionId) && _globalSession.TryGetValue(sessionId, out session))
            {
                return session;
            }
            return null;
        }

        /// <summary>
        /// Get session by sessionid.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static GameSession Get(string sessionId)
        {
            GameSession session;
            if (_globalSession.TryGetValue(sessionId, out session))
            {
                return session;
            }
            return null;
        }

        #endregion

        #region 删除Session
        public static bool TryRemove(IChannelHandlerContext context, out GameSession session)
        {
            var key = GetSessionId(context);
            return _globalSession.TryRemove(key, out session);
        }
        #endregion

        public static void PostSend(int userId, IByteBuffer data)
        {
            var session = Get(userId);
            if (session != null)
            {
                try
                {
                    session.PostSend(data);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("PostSend Error {0},{1},{2},{3}", userId, JsonConvert.SerializeObject(data), ex.Message, ex.StackTrace);
                }
            }
        }

        public static void PostSend(string sessionId, IByteBuffer data)
        {
            var session = Get(sessionId);
            if (session != null)
            {
                try
                {
                    session.PostSend(data);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("PostSend Error {0},{1},{2},{3}", sessionId, JsonConvert.SerializeObject(data), ex.Message, ex.StackTrace);
                }
            }
        }
    }
}
