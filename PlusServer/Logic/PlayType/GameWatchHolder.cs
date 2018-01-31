using System.Collections.Generic;
using System.Linq;
using PlusCommon;
using PlusServer.Logic.Enum;
using PlusServer.LogicActors;

namespace Common.PlayType
{
    public class GameWatchHolder : IGameRouteMap
    {
        public GameStatus GetNextStatus(GameStatus curStatus)
        {
            GameStatus state = GameStatus.None;
            switch (curStatus)
            {
                case GameStatus.None:
                    state = GameStatus.Grabing;
                    break;
                case GameStatus.Grabing:
                    state = GameStatus.Beting;
                    break;
                case GameStatus.Beting:
                    state = GameStatus.Completing;
                    break;
                case GameStatus.Completing:
                    state = GameStatus.None;
                    break;
            }
            return state;
        }

        public List<ActionInfo> GetActions(GameStatus curStatus)
        {
            List<ActionInfo> actions = new List<ActionInfo>();
            switch (curStatus)
            {
                case GameStatus.None:
                    {
                        var action1 = new ActionInfo();
                        action1.ActionType = ActionType.NotifyGameOver;
                        actions.Add(action1);

                        var action2 = new ActionInfo();
                        action2.ActionType = ActionType.NotifyReady;
                        action2.Parameters = new Dictionary<string, object>();
                        actions.Add(action2);
                    }
                    break;
                case GameStatus.Grabing:
                    {
                        var action1 = new ActionInfo();
                        action1.ActionType = ActionType.NotifySendCards;
                        action1.Parameters = new Dictionary<string, object>();
                        action1.Parameters.Add("CardNum", 4);
                        action1.Parameters.Add("IsShow", true);

                        var action2 = new ActionInfo();
                        action2.ActionType = ActionType.NotifyGrab;
                        action2.Parameters = new Dictionary<string, object>();
                        action2.Parameters.Add("MaxGrab", 0);

                        actions.Add(action1);
                        actions.Add(action2);
                    }
                    break;
                case GameStatus.Beting:
                    {
                        var action1 = new ActionInfo();
                        action1.ActionType = ActionType.NotifyHolder;
                        action1.Parameters = new Dictionary<string, object>();

                        var action2 = new ActionInfo();
                        action2.ActionType = ActionType.NotifyBet;
                        action1.Parameters = new Dictionary<string, object>();

                        actions.Add(action1);
                        actions.Add(action2);
                    }
                    break;
                case GameStatus.Completing:
                    {
                        var action1 = new ActionInfo();
                        action1.ActionType = ActionType.NotifySendCards;
                        action1.Parameters = new Dictionary<string, object>();
                        action1.Parameters.Add("CardNum", 1);
                        action1.Parameters.Add("IsShow", false);

                        var action2 = new ActionInfo();
                        action2.ActionType = ActionType.NotifyComplete;
                        action2.Parameters = new Dictionary<string, object>();

                        actions.Add(action1);
                        actions.Add(action2);
                    }
                    break;
            }
            return actions;
        }

        public int GetHolderId(RoomActor roomData)
        {
            //确定庄家位置
            var grabSeats = roomData.GetGrabSeatDatas();

            var seats = roomData.GetInGameSeatDatas();

            int holderId = 0;
            int random = 0;
            if (grabSeats.Count == 0)
            {
                if (seats.Count == 0)
                    return 10000;

                random = RandomUtils.GetRandom(0, seats.Count);
                holderId = seats[random].UserId;
            }
            else
            {
                var maxGrab = grabSeats.Select(m => m.Grab).Max();
                var grabSeat = grabSeats.Where(m => m.Grab == maxGrab);

                if(grabSeat.Count() == 1)
                {
                    holderId = grabSeat.First().UserId;
                }
                else
                {
                    random = RandomUtils.GetRandom(0, grabSeat.Count());
                    holderId = grabSeat.ElementAt(random).UserId;
                }
            }

            return holderId;
        }
    }
}
