using Messages;
using PlusServerCommon.Base;
using Proto;
using System.Threading.Tasks;

namespace PlusServer.Manager
{
    public class ManagerActor : IShareActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case UnRegisterPid unregister:

                    break;
            }
            return Actor.Done;
        }
    }
}
