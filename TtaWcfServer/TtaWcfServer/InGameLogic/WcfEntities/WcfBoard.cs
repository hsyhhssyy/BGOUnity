using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.WcfEntities
{
    public class WcfBoard
    {
        public WcfBoard(TtaGame game,int playerId)
        {
            
        }
        //设置裁判能看到的Board
        private void SetupRefereeBoard(TtaGame game)
        {
            
        }

        //在RefereeBoard的基础上隐藏观战者/选手不能看到的内容
        private void HideForPlayer(int playerId)
        {
            
        }
    }
}