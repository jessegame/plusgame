using System;
using System.Collections.Generic;
using System.Text;

namespace PlusServer.Logic.Data
{
    public class UserData
    {
        public UserData(UserInfo userInfo)
        {
            if (userInfo == null)
                throw new Exception("数据未加载成功");
            Info = userInfo;
        }

        public void Init()
        {
            BetUserId = -1;
            Bet = -1;
            GameScore = 0;
            IsInGame = false;
        }

        public UserInfo Info { get; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 离线时间
        /// </summary>
        public DateTime? OfflineTime { get; set; }

        /// <summary>
        /// 押注者
        /// </summary>
        public int UserId { get { return Info.UserId; } }

        /// <summary>
        /// 被押者
        /// </summary>
        public int BetUserId { get; set; }

        /// <summary>
        /// 押注额
        /// </summary>
        public int Bet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GameScore { get; set; }

        /// <summary>
        /// 是否在游戏中
        /// </summary>
        public bool IsInGame { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastTime { get; set; }
    }
}
