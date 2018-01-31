using System;
using System.Collections.Generic;
using System.Text;

namespace PlusServerCommon.Message
{
    public enum RpcMsgType
    {
        /// <summary>
        /// 加载用户数据
        /// </summary>
        LoadUserInfo = 1,
        /// <summary>
        /// 加载房间数据
        /// </summary>
        LoadRoomInfo = 2,
    }
}
