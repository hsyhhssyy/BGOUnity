using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.Wcf;

namespace TtaWcfServer.InGameLogic.ActionDefinition
{
    public abstract class ActionHandler
    {
        protected GameManager Manager { get; private set; }

        protected ActionHandler(GameManager manager)
        {
            Manager = manager;
        }

        public abstract List<PlayerAction> CheckAbleToPerform(int playerNo);
        
        //注意Data也可能包含来自action的data（可能经过用户的修改）
        public abstract ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data, WcfContext context);
        
    }
}