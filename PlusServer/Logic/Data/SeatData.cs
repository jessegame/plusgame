using System;
using System.Collections.Generic;
using PlusServerCommon.Data;
using ProtoBuf;

namespace PlusServer.Logic.Data
{
    /// <summary>
    /// 桌子的座位对象
    /// </summary>
    public class SeatData
    {
        private int _id;
        
        public SeatData(int id)
        {
            _id = id;
            _cardData = new List<CardData>();
            _cardDataBefore = new List<CardData>();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(UserData user)
        {
            User = user;
            ResetPlayingData();
        }

        public void Init()
        {
            User = null;
            ResetPlayingData();
        }

        /// <summary>
        /// 重新发牌
        /// </summary>
        public void ResetPlayingData(bool resetBeforeCards = true)
        {
            IsReady = false;
            Grab = -1;
            Bet = -1;
            IsComplete = false;
            LastTime = DateTime.Now;
            if (resetBeforeCards)
            {
                _cardDataBefore.Clear();
                _cardDataBefore.AddRange(_cardData);
            }
            _cardData.Clear();
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId
        {
            get
            {
                if (User == null)
                    return -1;
                else
                    return User.UserId;
            }
        }

        public UserData User { get; private set; }

        /// <summary>
        /// 座位编号
        /// </summary>
        public int SeatId { get { return _id; } }

        /// <summary>
        /// 是否在游戏中
        /// </summary>
        public bool InGame { get; set; }

        /// <summary>
        /// 是否亮牌
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 是否准备
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// 是否抢庄(-1:初始化，0:不抢，1-4:抢庄倍数)
        /// </summary>
        public int Grab { get; set; }

        /// <summary>
        /// 叫倍数
        /// </summary>
        public int Bet { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime? LastTime { get; set; }

        /// <summary>
        /// 玩家筹码
        /// </summary>
        public int GameScore { get; set; }

        /// <summary>
        /// 是否可以推注
        /// </summary>
        public bool CanIncrease { get; set; }

        /// <summary>
        /// 推注筹码
        /// </summary>
        public int IncreaseScore { get; set; }

        private List<CardData> _cardData;
        /// <summary>
        /// 本轮手牌数据
        /// </summary>
        public List<CardData> CardData
        {
            get { return _cardData; }
        }

        /// <summary>
        /// 用户状态
        /// </summary>
        public byte State { get; set; }

        private List<CardData> _cardDataBefore;
        /// <summary>
        /// 上轮手牌数据
        /// </summary>
        public List<CardData> CardDataBefore
        {
            get { return _cardDataBefore; }
        }

        /// <summary>
        /// 是否托管
        /// </summary>
        public bool IsAuto { get; set; }

    }
}
