﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public abstract PlayerAction PerfromAction(int playerNo, PlayerAction action);
        
    }
}