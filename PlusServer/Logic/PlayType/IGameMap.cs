using System;
using System.Collections.Generic;
using PlusServer.Logic.Enum;
using PlusServer.LogicActors;

namespace Common.PlayType
{
    public interface IGameRouteMap
    {
        GameStatus GetNextStatus(GameStatus curStatus);
        List<ActionInfo> GetActions(GameStatus curStatus);
        int GetHolderId(RoomActor roomData);
    }

    public struct ActionItem
    {
        public ActionInfo ActionInfo;
        public DateTime ActionTime;
    }

    public struct ActionInfo
    {
        public ActionType ActionType;
        public Dictionary<string, object> Parameters;
    }

    public enum ActionType
    {
        NotifyReady,
        NotifyGrab,
        NotifyBet,
        NotifyComplete,

        NotifyHolder,
        NotifySendCards,
        NotifyGameOver,
    }
}
