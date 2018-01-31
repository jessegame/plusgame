using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Messages;
using PlusGame.Config;
using PlusGame.Contract;
using PlusServer.Manager;
using PlusServerCommon.Base;
using Proto;

namespace PlusServer.LocationActors
{
    public class LocationActor : IShareActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RegisterPid registerPid:
                    ActorManagement.TryAddPid(registerPid.Sender, registerPid.Name);
                    context.Respond(new Done());
                    break;
                case RequestPid requestPid:
                    var pid = ActorManagement.TryGetPid(requestPid.Name);
                    context.Respond(new ResponsePid() { Sender = pid });
                    break;
                case RequestPackage package:
                    ResponsePackage repPackage = new ResponsePackage();
                    repPackage.InitPackage(package);
                    IByteBuffer data = GameEnvironment.ActionDispatcher.TryEncodePackage(repPackage);
                    SessionManager.PostSend(repPackage.UserId, data);
                    break;
            }
            return Actor.Done;
        }
    }
}
