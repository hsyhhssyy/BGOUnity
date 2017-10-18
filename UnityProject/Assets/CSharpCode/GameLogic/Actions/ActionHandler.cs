using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Actions
{
    public abstract class ActionHandler
    {
        protected GameLogicManager Manager { get; private set; }

        protected ActionHandler(GameLogicManager manager)
        {
            Manager = manager;
        }

        public abstract List<PlayerAction> CheckAbleToPerform(int playerNo);
        
        public abstract ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data);
        
    }
}