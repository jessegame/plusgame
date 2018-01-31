using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace PlusServerCommon.Models
{
    [Serializable, ProtoContract]
    public class GameUser
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public string NickName { get; set; }

        [ProtoMember(3)]
        public bool Sex { get; set; }

        [ProtoMember(4)]
        public string HeadIcon { get; set; }

        [ProtoMember(5)]
        public int UserLevel { get; set; }

        [ProtoMember(6)]
        public int ExpNum { get; set; }

        [ProtoMember(7)]
        public string FinalIP { get; set; }

        [ProtoMember(8)]
        public int Diamond { get; set; }

        [ProtoMember(9)]
        public string RegisterDate { get; set; }

        [ProtoMember(10)]
        public string Location { get; set; }

        [ProtoMember(11)]
        public string Comment { get; set; }

        /// <summary>
        /// 是否黑名单
        /// </summary>
        [ProtoMember(12)]
        public byte State { get; set; }

        [ProtoMember(13)]
        public int PlayNum { get; set; }
    }
}
