using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace PlusServer.Data
{
    [ProtoContract]
    public class GameUser
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public string NickName { get; set; }
    }
}
