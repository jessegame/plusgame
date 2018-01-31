using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace PlusGame.Contract
{
    public static class PackageExtension
    {
        public static void InitPackage(this ResponsePackage response, RequestPackage request)
        {
            response.MsgId = request.MsgId;
            response.ActionId = request.ActionId;
            response.SessionId = request.SessionId;
            response.UserId = request.UserId;
            response.St = request.St;
            response.ReceiveTime = request.ReceiveTime;
        }

        public static void IntiPackage(this ResponseData response, RequestData request)
        {
            response.MessageType = request.MessageType;
        }
    }
}
