using System.Threading.Tasks;
using Messages;
using PlusServer.Manager;
using PlusServerCommon.Base;
using Proto;

namespace PlusServer.Actors
{
    public class LoginActor : IShareActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RequestPackage package:
                    //ResponsePackage repPackage = new ResponsePackage();
                    //repPackage.InitPackage(package);
                    //IByteBuffer data = GameEnvironment.ActionDispatcher.TryEncodePackage(repPackage);
                    //var session = SessionManager.Get(repPackage.SessionId);
                    //session.PostSend(data);
                    RequestData reqPackage = new RequestData();
                    reqPackage.ActorName = ActorManagement.GetActorName<LoginActor>();
                    var loginPid = ActorManagement.GetOrLoadPid<LoginActor>();
                    loginPid.Tell(reqPackage);
                    break;
            }
            return Actor.Done;
        }
    }
}
