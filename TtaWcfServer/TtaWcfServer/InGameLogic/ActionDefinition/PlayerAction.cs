using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.ActionDefinition
{
    public class PlayerAction
    {
        public PlayerActionType ActionType;

        public Dictionary<Int32, Object> Data = new Dictionary<int, object>();

        /// <summary>
        /// 表明该Action不代表一个具体的真实游戏存在的Action，而是向服务器发送一个指令。
        /// 目前用于选择类卡牌的效果。
        /// 注意internal action不能改变游戏面板，因为也因此，执行internal action不需要在得到结果后refresh board
        /// </summary>
        public bool Internal = false;

        public PlayerAction()
        {

        }
    }
}