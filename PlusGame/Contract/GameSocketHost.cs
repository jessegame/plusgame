using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Messages;
using PlusCommon.Log;
using PlusCommon.Security;
using PlusGame.Config;
using PlusGame.Session;

namespace PlusGame.Contract
{
    public abstract class GameSocketHost
    {
        private IChannel boundChannel;
        private MultithreadEventLoopGroup bossGroup;
        private MultithreadEventLoopGroup workerGroup;
        private ServerBootstrap bootstrap;

        public GameSocketHost()
        {
            bossGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();
            bootstrap = new ServerBootstrap();

            bootstrap.Group(bossGroup, workerGroup);
            bootstrap.Channel<TcpServerSocketChannel>();
            bootstrap
                   .Option(ChannelOption.SoBacklog, GameEnvironment.SocketConfig.Backlog)
                   .Handler(new LoggingHandler("SRV-LSTN"))
                   .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                   {
                       IChannelPipeline pipeline = channel.Pipeline;
                       pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                       //pipeline.AddLast("timeout", new IdleStateHandler(0, 0, 60));
                       pipeline.AddLast("framing-enc", new LengthFieldPrepender(GameEnvironment.SocketConfig.ByteOrder, 
                           GameEnvironment.SocketConfig.PrePackageLength, 0, false));
                       pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(GameEnvironment.SocketConfig.ByteOrder,
                           Int32.MaxValue, 0, GameEnvironment.SocketConfig.PrePackageLength, 0, GameEnvironment.SocketConfig.PrePackageLength, true));
                       pipeline.AddLast("MainHandler", new MainHandler(this));
                   }));

            //心跳超时
            SessionManager.HeartbeatTimeoutHandle += OnHeartTimeout;
        }

        public async Task RunServerAsync()
        {
            try
            {
                EndPoint ipEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(GameEnvironment.SocketConfig.IPAddress), GameEnvironment.SocketConfig.Port);
                boundChannel = await bootstrap.BindAsync(ipEndPoint);
            }
            catch(Exception ex)
            {
                TraceLog.WriteError("RunServerAsync 异常:{0},{1}", ex.Message, ex.StackTrace);
            }
        }

        public async Task CloseServerAsync()
        {
            try
            {
                await boundChannel.CloseAsync();
                await Task.WhenAll(
                bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
            catch(Exception ex)
            {
                TraceLog.WriteError("CloseServerAsync 异常:{0},{1}", ex.Message, ex.StackTrace);
            }
        }

        protected virtual void OnConnected(GameSession session)
        {

        }

        protected virtual void OnDisconnected(GameSession session)
        {

        }

        protected virtual void OnDataReceived(IChannelHandlerContext context, IByteBuffer message)
        {

        }

        /// <summary>
        /// 心跳包
        /// </summary>
        /// <param name="session"></param>
        protected virtual void OnHeartbeat(GameSession session)
        {

        }

        /// <summary>
        /// 心跳超时
        /// </summary>
        /// <param name="session"></param>
        protected virtual void OnHeartTimeout(GameSession session)
        {

        }

        public class MainHandler : ChannelHandlerAdapter
        {
            private GameSocketHost _host;
            public MainHandler(GameSocketHost host)
            {
                _host = host;
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                var buffer = message as IByteBuffer;
                if (buffer == null)
                {
                    return;
                }

                _host.OnDataReceived(context, buffer);
            }

            public override void ChannelReadComplete(IChannelHandlerContext context)
            {
                context.Flush();
            }

            public override void ChannelRegistered(IChannelHandlerContext context)
            {
                var session = SessionManager.CreateNew(context);
                if (session != null)
                    _host.OnConnected(session);

                base.ChannelRegistered(context);
            }

            public override void ChannelUnregistered(IChannelHandlerContext context)
            {
                GameSession session;
                SessionManager.TryRemove(context, out session);
                _host.OnDisconnected(session);

                base.ChannelUnregistered(context);
            }

            public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
            {
                context.CloseAsync();
                TraceLog.WriteError("ExceptionCaught 异常:{0},{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
