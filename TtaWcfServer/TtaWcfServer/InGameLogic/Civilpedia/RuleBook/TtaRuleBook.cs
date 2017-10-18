using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.Civilpedia.RuleBook
{
    public abstract class TtaRuleBook
    {
        public abstract TtaGame SetupNewGame(GameRoom room);

        public abstract int DiscardAmount(GameRoom room);

        public abstract List<CardInfo> GetCivilDeckForAge(GameRoom room,Age age);
        public abstract List<CardInfo> GetMilitaryDeckForAge(GameRoom room,Age age);
    }

    public static class CommonRoomRule
    {
        /// <summary>
        /// 表示游戏开始时会随机调换座位
        /// </summary>
        public const string RandomSeats = "CommonRoomRule-RandomSeats";
        /// <summary>
        /// 表示游戏可以被任何人围观，并显示在主页上
        /// </summary>
        public const string PublicObservable= "CommonRoomRule-PublicObservable";
    }
}