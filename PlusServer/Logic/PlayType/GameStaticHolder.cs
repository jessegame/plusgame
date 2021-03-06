﻿using System;
using System.Collections.Generic;
using PlusServer.Logic.Enum;
using PlusServer.LogicActors;

namespace Common.PlayType
{
    public class GameStaticHolder : IGameRouteMap
    {
        public GameStatus GetNextStatus(GameStatus curStatus)
        {
            GameStatus state = GameStatus.None;
            switch (curStatus)
            {
                case GameStatus.None:
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
                        action1.Parameters.Add("CardNum", 5);
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
            var user = roomData.GetUser(roomData.CreateUserId);
            if (user == null)
                throw new Exception("GameStaticHolder-GetHolderId用户信息为空");

            var holderSeat = roomData.GetSeatData(user.UserId);
            if(holderSeat == null)
                throw new Exception("GameStaticHolder-GetHolderId房主没有玩游戏");

            return roomData.CreateUserId;
        }
    }
}
