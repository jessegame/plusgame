using System;
using System.Threading.Tasks;
using Messages;
using PlusServer.Manager;
using Proto;
using PlusGame.Contract;
using PlusCommon.Log;
using PlusServerCommon.Message;
using PlusServerCommon.Base;

namespace PlusServer.DataActors
{
    public class LoadActor : IShareActor
    {
        public void DispatchPackage(RequestData package)
        {
            PID srcPid = null;
            ResponseData response = new ResponseData();
            response.IntiPackage(package);
            try
            {
                srcPid = ActorManagement.GetOrLoadPid(package.ActorName);
                if (srcPid == null)
                    throw new Exception("获取Actor失败");

                switch ((RpcMsgType)package.MessageType)
                {
                    case RpcMsgType.LoadRoomInfo:

                        break;
                    case RpcMsgType.LoadUserInfo:

                        break;
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 10000;
                response.ErrorInfo = ex.Message;
                TraceLog.WriteError("{0},{1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (srcPid != null)
                    srcPid.Tell(response);
            }
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RequestData package:
                    Task.Run(() =>
                    {
                        DispatchPackage(package);
                    });
                    break;
            }
            return Actor.Done;
        }
    }
}
