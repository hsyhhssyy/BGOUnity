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
        
    }

    public static class CommonRoomRule
    {
        public const string RandomSeats = "CommonRoomRule-RandomSeats";
    }
}