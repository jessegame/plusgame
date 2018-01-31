using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using PlusCommon.Log;
using PlusServer.Logic.Data;
using PlusServer.Manager;
using PlusServerCommon.Base;
using PlusServerCommon.Message;
using Proto;

namespace PlusServer.LogicActors
{
    public class CheckTimeout
    {

    }

    public class RoomManagerActor : IShareActor
    {
        private ConcurrentDictionary<int, RoomInfo> roomList;
        private ConcurrentDictionary<int, UserInfo> userList;

        private Timer checkTableTimer;
        private Timer checkRoomTimer;

        //乐观锁
        private int isCheckTableWorking = 0;
        private int isCheckRoomWorking = 0;

        #region 单例

        private static RoomManagerActor _instance;

        static RoomManagerActor()
        {
            _instance = new RoomManagerActor();
        }

        /// <summary>
        /// 当前游戏房间
        /// </summary>
        public static RoomManagerActor Current
        {
            get { return _instance; }
        }

        private RoomManagerActor()
        {
            roomList = new ConcurrentDictionary<int, RoomInfo>();
            userList = new ConcurrentDictionary<int, UserInfo>();

            //每秒执行一次
            checkTableTimer = new Timer(OnCheckTableCallback, this, 1000, 1000);
            //没分钟执行一次
            checkRoomTimer = new Timer(OnCheckRoomCallback, this, 60 * 1000, 60 * 1000);
        }

        #endregion

        #region 超时回调

        private void OnCheckTableCallback(object state)
        {
            if (Interlocked.CompareExchange(ref isCheckTableWorking, 1, 0) == 0)
            {
                try
                {
                    foreach (var item in roomList.Keys)
                    {
                        var roomPid = ActorManagement.TryGetPid<RoomActor>(item);
                        if (roomPid != null)
                        {
                            roomPid.Tell(new CheckTimeout());
                        }
                    }
                }
                catch (Exception er)
                {
                    TraceLog.WriteError("OnTimerCallback error:{0}", er);
                }
                finally
                {
                    Interlocked.Exchange(ref isCheckTableWorking, 0);
                }
            }
        }

        private void OnCheckRoomCallback(object state)
        {
            if (Interlocked.CompareExchange(ref isCheckRoomWorking, 1, 0) == 0)
            {
                try
                {
                   
                }
                catch (Exception er)
                {
                    TraceLog.WriteError("OnTimerCallback error:{0}", er);
                }
                finally
                {
                    Interlocked.Exchange(ref isCheckRoomWorking, 0);
                }
            }
        }

        #endregion

        private void CreateRoom(RequestPackage package)
        {

        }

        private void ReleaseRoom(RequestPackage package)
        {

        }

        #region DispatcherMessage

        private void DispatcherRequestPackage(RequestPackage package)
        {
            switch (package.ActionId)
            {
                case 1002:
                     
                    break;
            }
        }

        private void DispatcherResponsePackage(ResponsePackage package)
        {
            switch (package.ActionId)
            {
                case 1002:
                    break;
            }
        }

        private void DispatcherRequestData(RequestData package)
        {

        }

        private void DispatcherResponseData(ResponseData package)
        {
            switch ((RpcMsgType)package.MessageType)
            {
                case RpcMsgType.LoadRoomInfo:
                    break;
                case RpcMsgType.LoadUserInfo:
                    break;
            }
        }

        #endregion

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RequestPackage requestPackage:
                    DispatcherRequestPackage(requestPackage);
                    break;
                case ResponsePackage responsePackage:
                    DispatcherResponsePackage(responsePackage);
                    break;

                //case RequestData requestData:
                //    DispatcherRequestData(requestData);
                //    break;
                case ResponseData responseData:
                    DispatcherResponseData(responseData);
                    break;
            }
            return Actor.Done;
        }
    }
}
