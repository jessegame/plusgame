//using System;
//using System.Collections.Generic;
//using System.Text;
//using PlusServer.Logic.Data;
//using PlusServer.LogicActors;

//namespace PlusServer.Logic
//{
//    public static class RoomHelper
//    {
//        #region 公共方法

//        /// <summary>
//        /// 获取座位信息
//        /// </summary>
//        /// <param name="roomId"></param>
//        /// <param name="seatId"></param>
//        /// <returns></returns>
//        public static SeatData GetSeatData(this RoomActor roomData, int roomId, int seatId)
//        {
//            if (seatId == -1)
//                return null;

//            var roomData = GetRoomData(roomId);
//            if (roomData != null)
//            {
//                return roomData.Seats[seatId];
//            }
//            return null;
//        }

//        public SeatData GetSeatData(RoomData roomData, int seatId)
//        {
//            if (seatId == -1)
//                return null;

//            if (roomData != null)
//            {
//                return roomData.Seats[seatId];
//            }
//            return null;
//        }

//        public SeatData GetSeatDataByUserId(int roomId, int userId)
//        {
//            var roomData = GetRoomData(roomId);
//            if (roomData != null)
//            {
//                var list = roomData.Seats.Where(m => m.UserId == userId);
//                if (list.Count() != 0)
//                    return list.First();
//            }
//            return null;
//        }

//        public SeatData GetSeatDataByUserId(RoomData roomData, int userId)
//        {
//            if (roomData != null)
//            {
//                var list = roomData.Seats.Where(m => m.UserId == userId);
//                if (list.Count() != 0)
//                    return list.First();
//            }
//            return null;
//        }

//        public List<SeatData> GetInGameSeatDatas(RoomData roomData, int exceptUserId = -1, int exceptSeatId = -1)
//        {
//            return roomData.Seats.Where(m => m.InGame && m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
//        }

//        public List<SeatData> GetUsedSeatDatas(RoomData roomData, int exceptUserId = -1, int exceptSeatId = -1)
//        {
//            return roomData.Seats.Where(m => m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
//        }

//        public List<SeatData> GetReadySeatDatas(RoomData roomData, int exceptUserId = -1, int exceptSeatId = -1)
//        {
//            return roomData.Seats.Where(m => m.IsReady && m.UserId != -1 && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
//        }

//        public List<SeatData> GetGrabSeatDatas(RoomData roomData)
//        {
//            return roomData.Seats.Where(m => m.InGame && m.Grab > 0).ToList();
//        }

//        public List<SeatData> GetBetSeatDatas(RoomData roomData)
//        {
//            return roomData.Seats.Where(m => m.Bet >= 0).ToList();
//        }

//        public List<SeatData> GetCompeteSeatDatas(RoomData roomData, int exceptUserId = -1, int exceptSeatId = -1)
//        {
//            return roomData.Seats.Where(m => m.IsComplete && m.InGame && m.UserId != exceptUserId && m.SeatId != exceptSeatId).ToList();
//        }

//        public SeatData GetRandomUnusePositionData(RoomData roomData)
//        {
//            List<SeatData> unUsePositions = Array.FindAll(roomData.Seats, m => m.UserId == -1).ToList();

//            if (unUsePositions.Count == 0)
//                return null;

//            int random = RandomUtils.GetRandom(0, unUsePositions.Count);

//            return unUsePositions[random];
//        }

//        /// <summary>
//        /// 只适合旁观押注
//        /// </summary>
//        /// <param name="roomData"></param>
//        /// <returns></returns>
//        public int GetPlayingCount(RoomData roomData)
//        {
//            var seats = GetUsedSeatDatas(roomData);
//            var bets = roomData.Users.Values.Where(m => m.IsInGame);
//            return seats.Count() + bets.Count();
//        }

//        /// <summary>
//        /// 下注位置
//        /// </summary>
//        /// <param name="roomData"></param>
//        /// <param name="userId"></param>
//        /// <returns></returns>
//        public BetInfo GetBetInfo(RoomData roomData, int userId)
//        {
//            var bets = roomData.Users.Values.Where(m => m.UserId == userId);
//            if (bets.Count() != 0)
//            {
//                return bets.First();
//            }
//            return null;
//        }

//        public List<SeatData> GetOffLineSeatDatas(RoomData roomData)
//        {
//            var inGameSeats = GetInGameSeatDatas(roomData);

//            List<SeatData> seats = new List<SeatData>();
//            foreach (var seat in inGameSeats)
//            {
//                var user = GetUser(seat.UserId);
//                if (!user.IsOnline)
//                {
//                    seats.Add(seat);
//                }
//            }
//            return seats;
//        }

//        #endregion
//    }
//}
