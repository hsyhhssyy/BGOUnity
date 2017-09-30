using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.ActionDefinition;

namespace TtaWcfServer.InGameLogic
{
    public class GameManager
    {
        public void SetupNewGame(GameRoom room)
        {
            //目前无视其他的设置仅支持2人    
        }



        public List<PlayerAction> GetCurrentPlayerAction(int id)
        {
            return null;
        }
    }
}