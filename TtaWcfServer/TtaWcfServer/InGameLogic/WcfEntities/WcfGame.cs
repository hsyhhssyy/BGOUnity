using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.WcfEntities
{
    public class WcfGame
    {
        public const int PlayerNumberObserver = -1;
        public const int PlayerNumberReferee = -2;


        public WcfGame(TtaGame game,int player)
        {
            //-1是Observer

        }

        public List<WcfBoard> Board;

    }
}