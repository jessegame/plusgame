using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages;
using PlusServer.Logic.Data;
using Proto;
using PlusServer.Logic.Enum;
using Common.PlayType;
using System.Collections.Concurrent;
using System.Linq;
using PlusCommon;
using PlusServer.Manager;
using PlusServerCommon.Enum;
using PlusServerCommon.Base;

namespace PlusServer.LogicActors
{
    public class RoomActor : IPersonalActor
    {
        public void Dispatcher(RequestPackage package)
        {
            switch (package.ActionId)
            {
                case 1002:
                    break;
                case 1004:
                    break;
                case 1005:
                    break;
            }
        }

        public void CheckTimeout()
        {

        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case RequestPackage package:
                    Dispatcher(package);
                    break;
                case CheckTimeout timeout:
                    CheckTimeout();
                    break;
            }
            return Actor.Done;
        }

        #region 公共方法

        public UserData GetUser(int userId)
        {
            UserData user;
            _users.TryGetValue(userId, out user);
            return user;
        }

        /// <summary>
        /// 获取座位信息
        /// </summary>
        /// <param name="seatId"></param>
        /// <returns></returns>
        public SeatData GetSeatData(int seatId)
        {
            if (seatId == -1)
                return null;
            return Seats[seatId];
        }

        public SeatData GetSeatDataByUserId(int userId)
        {
            var list = Seats.Where(m => m.UserId == userId);
            if (list.Count() != 0)
                return list.First();
            return null;
        }

        public List<SeatData> GetInGameSeatDatas(int exceptUserId = -1, int exceptSeatId = -1)
        {
            return Seats.Where(m => m.InGame && m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
        }

        public List<SeatData> GetUsedSeatDatas(int exceptUserId = -1, int exceptSeatId = -1)
        {
            return Seats.Where(m => m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
        }

        public List<SeatData> GetReadySeatDatas(int exceptUserId = -1, int exceptSeatId = -1)
        {
            return Seats.Where(m => m.IsReady && m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
        }

        public List<SeatData> GetGrabSeatDatas()
        {
            return Seats.Where(m => m.InGame && m.Grab > 0).ToList();
        }

        public List<SeatData> GetBetSeatDatas()
        {
            return Seats.Where(m => m.Bet >= 0).ToList();
        }

        public List<SeatData> GetCompeteSeatDatas(int exceptUserId = -1, int exceptSeatId = -1)
        {
            return Seats.Where(m => m.IsComplete && m.InGame && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
        }

        public SeatData GetRandomUnusePositionData()
        {
            List<SeatData> unUsePositions = Array.FindAll(Seats, m => m.UserId == -1).ToList();

            if (unUsePositions.Count == 0)
                return null;

            int random = RandomUtils.GetRandom(0, unUsePositions.Count);

            return unUsePositions[random];
        }

        /// <summary>
        /// 只适合旁观押注
        /// </summary>
        /// <param name="roomData"></param>
        /// <returns></returns>
        public int GetPlayingCount()
        {
            var seats = GetUsedSeatDatas();
            var bets = Users.Values.Where(m => m.IsInGame);
            return seats.Count() + bets.Count();
        }

        /// <summary>
        /// 下注位置
        /// </summary>
        /// <param name="roomData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserData GetBetInfo(int userId)
        {
            var bets = Users.Values.Where(m => m.UserId == userId);
            if (bets.Count() != 0)
            {
                return bets.First();
            }
            return null;
        }

        public List<SeatData> GetOffLineSeatDatas()
        {
            var inGameSeats = GetInGameSeatDatas();

            List<SeatData> seats = new List<SeatData>();
            foreach (var seat in inGameSeats)
            {
                var user = GetUser(seat.UserId);
                if (!user.IsOnline)
                {
                    seats.Add(seat);
                }
            }
            return seats;
        }

        #endregion

        #region 游戏中基础逻辑

        //public void LoadUserData(int userId)
        //{
        //    Task.Run(() =>
        //    {
        //        //ActorManagement.GetServerPid<>
        //    });
        //}

        //public void LoadRoomData(int roomId)
        //{
        //    Task.Run(() =>
        //    {

        //    });
        //}

        //public ResultInfo EnterRoom(int userId, int roomId)
        //{
        //    ResultInfo result = new ResultInfo();

        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.UserNotExist;

        //    var roomData = GetRoomData(roomId);
        //    if (roomData == null)
        //        return ResultType.RoomIsNotExsit;

        //    if (roomData.CreateUserId != userId && roomData.PlayType == NiuniuPlayType.WatchHolderBet &&
        //        roomData.Users.Count >= 8)
        //        return ResultType.DeskIsFull;

        //    user.ModifyLocked(() =>
        //    {
        //        user.RoomId = roomId;
        //    });

        //    if (!roomData.Users.Keys.Contains(userId))
        //        roomData.Users.Add(userId, new BetInfo(userId));

        //    GameNotify.BroadEnterRoom(roomData, user);

        //    GRPCClientManager.Current.NiuniuGameEnterRoom(roomId, userId);

        //    return result;
        //}

        //public ResultType ExitRoom(int userId)
        //{
        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.Success;

        //    var roomData = GetRoomData(user.RoomId);
        //    if (roomData == null)
        //        return ResultType.Success;

        //    if (roomData.GameCount > roomData.GameNum) //修改bug 玩家最后一局直接退出
        //    {
        //        ExitRoom(roomData, user);
        //        return ResultType.Success;
        //    }

        //    var seatData = GetSeatData(roomData, user.SeatId);
        //    if (seatData != null && roomData.GameCount > 0 && roomData.GameCount <= roomData.GameNum)
        //    {
        //        return ResultType.GameAlreadyStarted;
        //    }

        //    var betInfo = GetBetInfo(roomData, user.UserId);
        //    if (betInfo != null && betInfo.IsInGame)
        //    {
        //        return ResultType.GameAlreadyStarted;
        //    }

        //    return ExitRoom(roomData, user, seatData);
        //}

        //private ResultType ExitRoom(RoomData roomData, GameUser user, SeatData seatData = null, bool isNotify = true)
        //{
        //    if (isNotify)
        //        GameNotify.BroadExitRoom(roomData, user.UserId);

        //    if (seatData == null)
        //    {
        //        seatData = GetSeatData(roomData, user.SeatId);
        //    }

        //    if (seatData != null)
        //    {
        //        SitUp(user, seatData);
        //    }

        //    user.ModifyLocked(() =>
        //    {
        //        user.RoomId = -1;
        //    });

        //    GRPCClientManager.Current.NiuniuGameExitRoom(roomData.RoomId, user.UserId);

        //    roomData.Users.Remove(user.UserId);

        //    if (NiuniuPlayType.WatchHolderBet == roomData.PlayType && roomData.GameCount > 0 && roomData.GameState == GameStatus.Beting)
        //    {
        //        AfterBet(roomData);
        //    }

        //    //自动开始

        //    if (!roomData.IsGameStarted && roomData.IsOpenAutoStart && CheckAllReadyAndStart(roomData, roomData.PlayerNum - 2))//4人自动开始 !roomData.Users.Keys.Contains(roomData.CreateUserId) &&
        //    {
        //        GameRoomStart(roomData);
        //    }

        //    return ResultType.Success;
        //}

        //public ResultType SitDown(int userId, out int seatId, out int score)
        //{
        //    seatId = -1;
        //    score = 0;

        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.UserNotExist;

        //    var roomData = GetRoomData(user.RoomId);
        //    if (roomData == null)
        //        return ResultType.RoomIsNotExsit;

        //    if (user.SeatId != -1)
        //        return ResultType.UserAlreadySit;

        //    if (roomData.GameCount >= roomData.GameNum)
        //        return ResultType.RoomIsNotExsit;

        //    SeatData seatData = null;
        //    var result = ResultType.Success;

        //    lock (roomData)
        //    {
        //        seatData = GetRandomUnusePositionData(roomData);
        //        if (seatData == null)
        //        {
        //            return ResultType.DeskIsFull;
        //        }

        //        if (roomData.PlayType == NiuniuPlayType.WatchHolderBet)
        //        {
        //            BetInfo betInfo;
        //            if (GetPlayingCount(roomData) >= roomData.WatchNum &&
        //                roomData.Users.TryGetValue(userId, out betInfo) &&
        //                !betInfo.IsInGame)
        //            {
        //                return ResultType.DeskIsFull;
        //            }
        //        }

        //        seatData.Init(user);
        //        user.ModifyLocked(() =>
        //        {
        //            user.SeatId = seatData.SeatId;
        //        });
        //        seatId = seatData.SeatId;

        //        if (roomData.PlayType == NiuniuPlayType.StaticHolder &&
        //            roomData.CreateUserId == userId)
        //        {
        //            score = roomData.MinBet * roomData.HolderScore;
        //            seatData.GameScore = score;
        //        }

        //        if (roomData.PlayType == NiuniuPlayType.WatchHolderBet)
        //        {
        //            BetInfo betInfo;
        //            if (roomData.Users.TryGetValue(userId, out betInfo) && betInfo.IsInGame)
        //            {
        //                score = betInfo.GameScore;
        //                seatData.GameScore = score;
        //                if (betInfo.Bet == -1)
        //                {
        //                    PlayerBet(userId, 0, userId);
        //                    betInfo.Init();
        //                }
        //            }
        //        }

        //        //TraceLog.WriteInfo("用户坐下广播,{0}_{1}_{2}_{3}", roomData.RoomId, userId, seatId, score);
        //        GameNotify.BroadSitDown(roomData, userId, seatId, score);

        //        var notifyEvent = new TimeoutNotifyEventArgs(1);
        //        notifyEvent.Callback += (p) =>
        //        {
        //            //玩家直接准备
        //            PlayerReady(userId);
        //        };
        //        EventNotifier.Put(notifyEvent);

        //        GRPCClientManager.Current.NiuniuSitDownRoom(roomData.RoomId, userId);
        //    }
        //    return result;
        //}

        //public ResultType SitUp(int userId)
        //{
        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.UserNotExist;

        //    var roomData = GetRoomData(user.RoomId);
        //    if (roomData == null)
        //        return ResultType.RoomIsNotExsit;

        //    var seatData = GetSeatData(user.RoomId, user.SeatId);
        //    if (seatData == null)
        //        return ResultType.SeatNotExist;

        //    if (roomData.GameCount > 0)
        //    {
        //        return ResultType.GameAlreadyStarted;
        //    }

        //    return SitUp(user, seatData);
        //}

        //public ResultType SitUp(GameUser user, SeatData seatData)
        //{
        //    user.ModifyLocked(() =>
        //    {
        //        user.SeatId = -1;
        //    });
        //    seatData.Init();
        //    return ResultType.Success;
        //}

        //public ResultType RequestReleaseRoom(int userId)
        //{
        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.UserNotExist;

        //    var roomData = GetRoomData(user.RoomId);
        //    if (roomData == null)
        //        return ResultType.RoomIsNotExsit;

        //    if (roomData.GameCount == 0 && roomData.CreateUserId != userId)
        //        return ResultType.Error;

        //    if (roomData.GameCount == 0 &&
        //        roomData.CreateUserId == userId)
        //    {
        //        //通知玩家，房间解散
        //        GameNotify.NotifyReleaseRoom(roomData);
        //        GameRelease(roomData, false);

        //        return ResultType.Success;
        //    }

        //    var seatData = GetSeatData(user.RoomId, user.SeatId);
        //    if (seatData == null)
        //        return ResultType.SeatNotExist;

        //    if (!seatData.InGame)
        //        return ResultType.Error;

        //    if (roomData.IsInRelease)//已经在解散了
        //        return ResultType.Error;

        //    roomData.IsInRelease = true;

        //    GameNotify.BroadReqReleaseRoom(roomData, userId);

        //    roomData.ReleaseRecord.Clear();
        //    var seats = GetInGameSeatDatas(roomData);
        //    foreach (var seat in seats)
        //    {
        //        if (seat.UserId == user.UserId)
        //            roomData.ReleaseRecord.Add(new ReleaseInfo() { UserId = userId, IsAgree = true });
        //        else
        //            roomData.ReleaseRecord.Add(new ReleaseInfo() { UserId = seat.UserId, IsAgree = false });
        //    }

        //    roomData.ApplayUserId = userId;
        //    roomData.ApplyTime = (int)MathUtils.GetUnixEpochTimeSpan(DateTime.Now).TotalSeconds;
        //    roomData.StopTimer();

        //    if (CheckReleaseRoom(roomData))
        //    {
        //        //通知玩家，房间解散
        //        GameNotify.NotifyReleaseRoom(roomData);
        //        GameRelease(roomData, false);
        //        return ResultType.Success;
        //    }

        //    var releaseEvent = new TimeoutNotifyEventArgs(roomData.ReleaseTimeOut);
        //    releaseEvent.Target = roomData.RoomId;
        //    releaseEvent.Callback += (p) =>
        //    {
        //        //这里处理延迟逻辑
        //        var room = GetRoomData((int)p.Target);
        //        if (room != null)
        //        {
        //            if (room.ReleaseRecord.Count != 0)
        //            {
        //                GameNotify.NotifyReleaseRoom(roomData);
        //                GameRelease(roomData, false);
        //            }
        //        }
        //    };
        //    EventNotifier.Put(releaseEvent);

        //    return ResultType.Success;
        //}

        //public ResultType AllowReleaseRoom(int userId, bool isAgree)
        //{
        //    var user = GetUser(userId);
        //    if (user == null)
        //        return ResultType.UserNotExist;

        //    var roomData = GetRoomData(user.RoomId);
        //    if (roomData == null)
        //        return ResultType.RoomIsNotExsit;

        //    if (roomData.ReleaseRecord.Count == 0)
        //        return ResultType.Error;

        //    var seatData = GetSeatData(roomData, user.SeatId);
        //    if (seatData == null)
        //        return ResultType.SeatNotExist;

        //    if (!seatData.InGame)
        //        return ResultType.Error;

        //    GameNotify.BroadIsReleaseRoom(roomData, userId, isAgree);
        //    if (!isAgree)
        //    {
        //        roomData.ReleaseRecord.Clear();
        //        roomData.ApplayUserId = 0;
        //        roomData.ApplyTime = 0;
        //        roomData.IsInRelease = false;

        //        GameContinue(roomData);
        //        return ResultType.Success;
        //    }

        //    var releaseItem = roomData.ReleaseRecord.Find(m => m.UserId == userId);
        //    if (releaseItem != null)
        //        releaseItem.IsAgree = isAgree;
        //    //roomData.ReleaseRecord.Add(new ReleaseInfo() { UserId = userId, IsAgree = isAgree });

        //    if (CheckReleaseRoom(roomData))
        //    {
        //        //通知玩家，房间解散
        //        GameNotify.NotifyReleaseRoom(roomData);

        //        GameRelease(roomData, false);
        //    }

        //    return ResultType.Success;
        //}

        //public bool CheckReleaseRoom(RoomData roomData)
        //{
        //    var seats = GetInGameSeatDatas(roomData);

        //    //int offlineCount = 0;

        //    //foreach (var seat in seats)
        //    //{
        //    //    var user = GetUser(seat.UserId);
        //    //    if (user != null && !user.IsOnline)
        //    //    {
        //    //        offlineCount++;
        //    //    }
        //    //}

        //    if (roomData.ReleaseRecord.Where(m => m.IsAgree).Count() == seats.Count())
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public void GameRelease(RoomData roomData, bool isNotify = true)
        //{
        //    //释放数据中房间信息
        //    GRPCClientManager.Current.NiuniuReleaseRoomRequest(roomData.RoomId, roomData.CreateTime);

        //    foreach (var userId in roomData.Users.Keys)
        //    {
        //        var user = GetUser(userId);
        //        ExitRoom(roomData, user, isNotify: isNotify);
        //    }

        //    //清除用户内存
        //    foreach (var userId in roomData.Users.Keys)
        //    {
        //        RemoveUser(userId);
        //    }

        //    //释放定时器
        //    roomData.Close();

        //    //删除房间内存
        //    RemoveRoomData(roomData.RoomId);
        //}

        //public void GameContinue(RoomData roomData)
        //{
        //    roomData.ReStartTimer();
        //}

        //public bool GameRoomStart(RoomData roomData)
        //{
        //    lock (roomData)
        //    {
        //        var seats = GetUsedSeatDatas(roomData);
        //        GRPCClientManager.Current.NiuniuGameStart(roomData.CreateUserId, roomData.RoomId, seats
        //            .Where(m => m.UserId != roomData.CreateUserId)
        //            .Select(m => m.UserId).ToList());

        //        //if (!roomData.IsTimerStarted)
        //        roomData.Start();

        //        if (!CheckAllReadyAndStart(roomData, roomData.MinPlayerNum))
        //            return false;

        //        if (NiuniuPlayType.NiuniuHolder == roomData.PlayType)
        //        {
        //            if (seats.Count == 0)
        //                return false;

        //            roomData.HolderId = seats[RandomUtils.GetRandom(0, seats.Count)].UserId;
        //        }

        //        roomData.StartTimer();

        //        GameStart(roomData);
        //    }
        //    //DoAction(roomData);
        //    return true;
        //}

        //public ResultType GameHolderDown(RoomData roomData)
        //{
        //    if (NiuniuPlayType.StaticHolder == roomData.PlayType)
        //        return ResultType.Error;

        //    if (roomData.GameCount < 3)
        //        return ResultType.Error;

        //    GameNotify.NotifyRoomOver(roomData, null);

        //    GameRelease(roomData, false);

        //    return ResultType.Success;
        //}

        //public void BroadOffline(int userId)
        //{
        //    var user = MainLogic.Current.GetUser(userId);
        //    if (user != null && user.IsOnline)
        //    {
        //        user.ModifyLocked(() =>
        //        {
        //            user.OfflineTime = DateTime.Now;
        //            user.IsOnline = false;
        //        });

        //        var roomData = MainLogic.Current.GetRoomData(user.RoomId);
        //        if (roomData != null)
        //        {
        //            GameNotify.BroadOffline(roomData, userId);
        //        }

        //        //var seatData = GetSeatDataByUserId(roomData, userId);
        //        //if (seatData != null && seatData.InGame)
        //        //{
        //        //    switch (roomData.GameState)
        //        //    {
        //        //        case GameStatus.None:
        //        //            {
        //        //                if (!seatData.IsReady)
        //        //                {
        //        //                    var notifyEvent = new TimeoutNotifyEventArgs(AutoTimeout);
        //        //                    notifyEvent.Callback += (p) =>
        //        //                    {
        //        //                        PlayerReady(seatData.UserId);
        //        //                    };
        //        //                    EventNotifier.Put(notifyEvent);
        //        //                }
        //        //            }
        //        //            break;
        //        //        case GameStatus.Grabing:
        //        //            {
        //        //                if (seatData.Grab == -1)
        //        //                {
        //        //                    var notifyEvent = new TimeoutNotifyEventArgs(AutoTimeout);
        //        //                    notifyEvent.Callback += (p) =>
        //        //                    {
        //        //                        PlayerGrab(roomData, seatData, 0);
        //        //                    };
        //        //                    EventNotifier.Put(notifyEvent);
        //        //                }
        //        //            }
        //        //            break;
        //        //        case GameStatus.Beting:
        //        //            {
        //        //                if (seatData.Bet != 0 && seatData.UserId != roomData.HolderId)
        //        //                {
        //        //                    var notifyEvent = new TimeoutNotifyEventArgs(AutoTimeout);
        //        //                    notifyEvent.Callback += (p) =>
        //        //                    {
        //        //                        PlayerBet(roomData, seatData, roomData.MinBet);
        //        //                    };
        //        //                    EventNotifier.Put(notifyEvent);
        //        //                }
        //        //            }
        //        //            break;
        //        //        case GameStatus.Completing:
        //        //            {
        //        //                if (!seatData.IsComplete)
        //        //                {
        //        //                    var notifyEvent = new TimeoutNotifyEventArgs(AutoTimeout);
        //        //                    notifyEvent.Callback += (p) =>
        //        //                    {
        //        //                        if (seatData.CardData.Count == 5)
        //        //                            PlayerComplete(userId, PokerManager.SortCards(seatData.CardData, roomData));
        //        //                    };
        //        //                    EventNotifier.Put(notifyEvent);
        //        //                }
        //        //            }
        //        //            break;
        //        //    }
        //        //}
        //    }
        //}

        //public void BroadMessage(RoomData room, int fromUserId, int toUserId, int messageType, string content)
        //{
        //    GameNotify.BroadMessage(room, fromUserId, toUserId, (MessageType)messageType, content);
        //}

        //public void BroadAuto(RoomData room, int userId, bool isAuto)
        //{
        //    var user = MainLogic.Current.GetUser(userId);
        //    if (user != null)
        //    {
        //        var seatData = GetSeatData(room, user.SeatId);
        //        if (seatData != null)
        //        {
        //            seatData.IsAuto = isAuto;
        //        }
        //        GameNotify.BroadAuto(room, userId, isAuto);
        //    }
        //}

        #endregion

        #region RoomData

        /// <summary>
        /// 参加过游戏玩家
        /// </summary>
        public HashSet<int> JoinUserIds { get; set; }

        private ConcurrentDictionary<int, UserData> _users;
        private int _roomId;
        private string _createTime;
        private int _createUserId;
        private NiuniuPlayType _playType;
        private int _minBet;
        private int _gameNum;
        private CostType _costType;
        private int _rule;
        private int _goldNiu;
        private int _silverNiu;
        private int _quad;
        private int _fiveNiu;

        private int _straightNiu;
        private int _flushNiu;
        private int _gourdNiu;
        private int _straightFlush;

        private bool _feeIncrease;
        private bool _startedNoComeIn;
        private int _holderScore;
        private int _grabMultiple;
        private int _playerNum;
        private int _watchNum;
        private bool _isOpenWhiteName;
        private bool _isOpenShuffling;
        private bool _isOpenAutoStart;
        private bool _isBetLimit;
        private int _pushBet;

        private int _timerPeriod = 0;

        private const int PackMaxNum = 52;
        private SeatData[] _seats;
        private List<int> _cardList;

        public RoomActor(int roomId, string createTime, int createUserId,
            NiuniuPlayType playType, int minBet, int gameNum, CostType costType, int rule,
            int goldNiu, int silverNiu, int quad, int fiveNiu, int straightNiu, int flushNiu, int gourdNiu, int straightFlush,
            bool isBetLimit, int pushBet, bool feeIncrease, bool startedNoComeIn,
            bool isOpenWhiteName, bool isOpenShuffling, bool isOpenAutoStart, int houseId, int positionId, string houseName, IGameRouteMap gameMap,
            int holderScore = -1, int grabMultiple = -1, int playerNum = 6,
            int holderId = -1, int watchNum = 4, int gameCount = 0)
        {
            _users = new ConcurrentDictionary<int, UserData>();
            _roomId = roomId;
            _createTime = createTime;
            _createUserId = createUserId;
            _playType = playType;
            _minBet = minBet;
            _gameNum = gameNum;
            _costType = costType;
            _rule = rule;
            _goldNiu = goldNiu;
            _silverNiu = silverNiu;
            _quad = quad;
            _fiveNiu = fiveNiu;
            _feeIncrease = feeIncrease;
            _startedNoComeIn = startedNoComeIn;
            _holderScore = holderScore;
            _grabMultiple = grabMultiple;
            _playerNum = playerNum;
            _watchNum = watchNum;
            _isOpenWhiteName = isOpenWhiteName;
            _isOpenShuffling = isOpenShuffling;
            _isOpenAutoStart = isOpenAutoStart;
            _isBetLimit = isBetLimit;
            _pushBet = pushBet;

            _straightNiu = straightNiu;
            _flushNiu = flushNiu;
            _gourdNiu = gourdNiu;
            _straightFlush = straightFlush;

            HouseId = houseId;
            PositionId = positionId;
            HouseName = houseName;

            GameRouteMap = gameMap;

            _seats = new SeatData[_playerNum];
            for (int i = 0; i < _playerNum; i++)
            {
                _seats[i] = new SeatData((short)i);
            }
            _cardList = new List<int>(PackMaxNum);
            for (int i = 0; i < PackMaxNum; i++)
            {
                _cardList.Add(i);
            }

            if (playerNum == 12)
            {
                for (int i = 0; i < PackMaxNum; i++)
                {
                    _cardList.Add(i);
                }
            }

            GameState = GameStatus.None;
            GameCount = gameCount;

            IsGameStarted = false;

            ReleaseRecord = new List<ReleaseInfo>();
            Settlement = string.Empty;
            HolderId = holderId;

            JoinUserIds = new HashSet<int>();
        }

        #region 属性值

        /// <summary>
        /// 玩家列表
        /// </summary>
        public ConcurrentDictionary<int, UserData> Users
        {
            get { return _users; }
        }

        public SeatData[] Seats { get { return _seats; } }

        public List<int> CardData { get { return _cardList; } }

        /// <summary>
        /// 邀请码
        /// </summary>
        public int RoomId { get { return _roomId; } }

        public string CreateTime { get { return _createTime; } }

        /// <summary>
        /// 
        /// </summary>
        public int CreateUserId { get { return _createUserId; } }

        public NiuniuPlayType PlayType { get { return _playType; } }

        public int MinBet { get { return _minBet; } }

        public int GameNum { get { return _gameNum; } }

        public CostType CostType { get { return _costType; } }

        public int Rule { get { return _rule; } }

        /// <summary>
        /// 五花牛
        /// </summary>
        public int GoldNiu { get { return _goldNiu; } }

        /// <summary>
        /// 四花牛
        /// </summary>
        public int SilverNiu { get { return _silverNiu; } }

        /// <summary>
        /// 炸弹
        /// </summary>
        public int Quad { get { return _quad; } }

        /// <summary>
        /// 五小牛
        /// </summary>
        public int FiveNiu { get { return _fiveNiu; } }

        /// <summary>
        /// 顺子牛
        /// </summary>
        public int StraightNiu { get { return _straightNiu; } }

        /// <summary>
        /// 同花牛
        /// </summary>
        public int FlushNiu { get { return _flushNiu; } }

        /// <summary>
        /// 葫芦牛
        /// </summary>
        public int GourdNiu { get { return _gourdNiu; } }

        /// <summary>
        /// 同花顺
        /// </summary>
        public int StraightFlush { get { return _straightFlush; } }

        public bool FeeIncrease { get { return _feeIncrease; } }

        public bool StartedNoComeIn { get { return _startedNoComeIn; } }

        public int HolderScore { get { return _holderScore; } }

        public int GrabMultiple { get { return _grabMultiple; } }

        public int PlayerNum { get { return _playerNum; } }

        public bool IsOpenWhiteName { get { return _isOpenWhiteName; } }

        public bool IsOpenShuffling { get { return _isOpenShuffling; } }

        public bool IsOpenAutoStart { get { return _isOpenAutoStart; } }

        public bool IsBetLimit { get { return _isBetLimit; } }

        public int PushBet { get { return _pushBet; } }

        public int HouseId { get; private set; }

        public int PositionId { get; private set; }

        public string HouseName { get; private set; }

        /// <summary>
        /// 参加游戏总人数
        /// </summary>
        public int WatchNum { get { return _watchNum; } }

        public int MinPlayerNum { get { return 2; } }

        public int MaxPlayerNum { get { return 6; } }

        /// <summary>
        /// 60秒
        /// </summary>
        public int ReleaseTimeOut { get { return 60; } }

        /// <summary>
        /// 是否释放桌子记录
        /// </summary>
        public List<ReleaseInfo> ReleaseRecord { get; set; }

        public IGameRouteMap GameRouteMap { get; private set; }

        #region 可扩展

        //public int ReadyWaitTime { get { return _isOpenShuffling ? 5 : 2; } }//准备时间

        //public int GrabWaitTime { get { return _isOpenShuffling ? 10 : 3; } }//抢庄时间

        //public int BetWaitTime { get { return _isOpenShuffling ? 10 : 5; } }//下注时间

        //public int CompleteWaitTime { get { return _isOpenShuffling ? 15 : 8; } }//亮牌时间

        public int ReadyWaitTime { get { return _isOpenShuffling ? 5 : 0; } }//准备时间

        public int GrabWaitTime { get { return _isOpenShuffling ? 7 : 5; } }//抢庄时间

        public int BetWaitTime { get { return _isOpenShuffling ? 8 : 5; } }//下注时间

        public int CompleteWaitTime { get { return _isOpenShuffling ? 15 : 8; } }//亮牌时间

        #endregion

        #endregion

        #region 游戏相关
        /// <summary>
        /// 房间是否关闭
        /// </summary>
        public bool IsGameStarted { get; private set; }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public GameStatus GameState { get; set; }

        /// <summary>
        /// 游戏状态改变时间
        /// </summary>
        public DateTime StateChangeTime { get; set; }

        /// <summary>
        /// 庄家UserId
        /// </summary>
        public int HolderId { get; set; }

        /// <summary>
        /// 游戏局数
        /// </summary>
        public int GameCount { get; private set; }

        /// <summary>
        /// 是否正在解散
        /// </summary>
        public bool IsInRelease { get; set; }

        /// <summary>
        /// 游戏结算数据
        /// </summary>
        public string Settlement { get; set; }

        public void ResetGameData()
        {
            //HolderId = -1;
            //GameState = GameStatus.None;
            GameCount++;
            Settlement = string.Empty;

            foreach (var seat in _seats)
            {
                seat.ResetPlayingData();
            }

            foreach (var item in Users.Values)
            {
                item.BetUserId = -1;
                item.Bet = -1;
                //item.IsInGame = false;
            }
        }

        public int ApplyTime { get; set; }

        public int ApplayUserId { get; set; }

        #endregion

        private void Start()
        {
            GameCount++;
            IsGameStarted = true;
        }

        private void Close()
        {
            IsGameStarted = false;
        }

        #endregion
    }

    public class ReleaseInfo
    {
        public int UserId { get; set; }

        public bool IsAgree { get; set; }
    }
}
