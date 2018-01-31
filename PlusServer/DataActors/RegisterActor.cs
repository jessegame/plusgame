using Messages;
using PlusGame.Contract;
using PlusServer.Manager;
using PlusServerCommon.Base;
using Proto;
using System;
using System.Threading.Tasks;

namespace PlusServer.DataActors
{
    public class RegisterActor : IShareActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RequestPackage package:
                    try
                    {
                        var pid = ActorManagement.GetOrLoadPid(package.ActorName);
                        if (pid != null)
                        {
                            var response = new ResponsePackage();
                            response.InitPackage(package);
                            response.Data = "注册成功";
                            pid.Tell(response);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    break;
            }
            return Actor.Done;
        }

        //private async Task<int> GetUserInfo()
        //{
        //    Task.Run(() =>
        //    {
        //        Task.Delay(10000);
        //    });
        //}
    }
}
