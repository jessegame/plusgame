using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Messages;
using PlusCommon.Log;
using PlusGame.Config;
using PlusGame.Contract;
using PlusGame.Redis;
using PlusGame.Session;
using PlusServer.Actors;
using PlusServer.Data;
using PlusServer.DataActors;
using PlusServer.LocationActors;
using PlusServer.Manager;
using Proto;
using Proto.Remote;

namespace PlusServer
{
    class Program
    {
        public static HashSet<int> ThreadIds = new HashSet<int>();
        static void Main(string[] args)
        {
            //ActorConfig config = new ActorConfig();
            //config.LocationIPAddress = "127.0.0.1";
            //config.LocationPort = 8001;
            //config.LocationActorName = "LOCATIONACTORNAME";
            //config.ActorIPAddress = "127.0.0.1";
            //config.ActorPort = 9001;
            ////RedisConfig config = new RedisConfig();
            ////config.PreRedisKey = "PLUSDATA";
            ////config.RedisHost = "127.0.0.1:6379";
            ////config.RedisDB = 0;
            //string txt = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            //System.IO.File.WriteAllText(string.Format("config/{0}.txt", config.GetType().Name), txt);
            //try
            //{
            //    RedisHelper.Init();

            //    RedisHelper.HashSet<GameUser>("GAMEUSER", "10038", new GameUser() { UserId = 10038, NickName = "test" });
            //    var user = RedisHelper.HashGet<GameUser>("GAMEUSER", "10038");

            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        RedisHelper.StringSet(i.ToString(), i.ToString());
            //        RedisHelper.StringGet(i.ToString());
            //    }
            //    watch.Stop();
            //    Console.WriteLine(watch.ElapsedMilliseconds);
            //}
            //catch (Exception ex)
            //{

            //}

            ServerHost host = new ServerHost();
            try
            {
                Serialization.RegisterFileDescriptor(Messages.ProtosReflection.Descriptor);
                Remote.Start(GameEnvironment.ActorConfig.ActorIPAddress, GameEnvironment.ActorConfig.ActorPort);

                #region Location Server

                var serverLocationPid = Actor.SpawnNamed(
                    Actor.FromProducer(() => new LocationActor()),
                    GameEnvironment.ActorConfig.LocationActorName);

                #endregion

                var locationPid = new PID(string.Format("{0}:{1}",
                    GameEnvironment.ActorConfig.LocationIPAddress,
                    GameEnvironment.ActorConfig.LocationPort),
                    GameEnvironment.ActorConfig.LocationActorName);

                ActorManagement.SetLocationPid(locationPid);

                #region Data Server

                var register = Actor.SpawnNamed(
                    Actor.FromProducer(() => new RegisterActor()),
                    ActorManagement.GetActorName<RegisterActor>());

                ActorManagement.RegisterPid<RegisterActor>(register);

                #endregion

                #region Login Server

                var loginPid = Actor.SpawnNamed(
                    Actor.FromProducer(() => new LoginActor()),
                    ActorManagement.GetActorName<LoginActor>());

                ActorManagement.RegisterPid<LoginActor>(loginPid);

                #endregion

                host.RunServerAsync().Wait();

                TraceLog.WriteInfo("服务器{0},{1}启动成功", GameEnvironment.SocketConfig.IPAddress, GameEnvironment.SocketConfig.Port);

                Task.Run(() =>
                {
                    while (true)
                    {
                        Task.Delay(1000).Wait();
                        Console.WriteLine("Session count:{0},{1}:{2}", SessionManager.GetCount(), ActorManagement.GetPidCount(), ActorManagement.GetPidTypeNames());
                        Console.WriteLine("线程个数:{0}", ThreadIds.Count);
                    }
                });

                Console.Read();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("服务器{0},{1}启动失败,{2},{3}", GameEnvironment.SocketConfig.IPAddress, GameEnvironment.SocketConfig.Port, ex.Message, ex.StackTrace);
            }
            finally
            {
                host.CloseServerAsync().Wait();
            }
        }
    }

    public class ServerHost : GameSocketHost
    {
        protected override void OnConnected(GameSession session)
        {

        }

        protected override void OnDisconnected(GameSession session)
        {

        }

        protected override void OnDataReceived(IChannelHandlerContext context, IByteBuffer message)
        {
            Program.ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
            context.WriteAndFlushAsync(message);
            //Task.Run(() =>
            //{
                //var session = SessionManager.Get(context);
                //session.PostSend(message);
            //});
            

            //Task.Run(() =>
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    try
            //    {
            //        string result;
            //        RequestPackage package;
            //        if (TryBuildPackage(context, message, out result, out package))
            //        {
            //            var loginPid = ActorManagement.GetLocalServerPid<LoginActor>();
            //            loginPid.Tell(package);
            //            //ResponsePackage repPackage = new ResponsePackage();
            //            //repPackage.InitPackage(package);
            //            //IByteBuffer data = GameEnvironment.ActionDispatcher.TryEncodePackage(repPackage);
            //            //var session = SessionManager.Get(repPackage.SessionId);
            //            //session.PostSend(data);
            //        }
            //        else
            //        {
            //            throw new Exception(result);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ResponsePackage response = new ResponsePackage();
            //        response.ErrorCode = 10000;
            //        response.ErrorInfo = ex.Message;
            //        var data = GameEnvironment.ActionDispatcher.TryEncodePackage(response);
            //        context.WriteAndFlushAsync(data);
            //        TraceLog.WriteError("接受数据异常:{0},{1},{2}", ex.Message, ex.StackTrace, message.ToString());
            //    }
            //    watch.Stop();
            //    if (watch.ElapsedMilliseconds > 5)
            //        TraceLog.WriteError("解包超时:{0},{1}", message.ToString(), watch.ElapsedMilliseconds);
            //});
        }

        protected override void OnHeartTimeout(GameSession session)
        {

        }
    }
}