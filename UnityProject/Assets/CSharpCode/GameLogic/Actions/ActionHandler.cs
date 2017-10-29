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

        public abstract List<PlayerAction> GenerateAction(int playerNo);

        /// <summary>
        /// 检查确认一个Action能否执行，返回false表示不执行
        /// </summary>
        /// <param name="playerNo"></param>
        /// <param name="action"></param>
        /// <param name="boardManagerStateData"></param>
        /// <returns></returns>
        public virtual bool CheckAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            return true;
        }

        public abstract ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData);

        public virtual ActionResponse PerfromInternalAction(int playerNo, PlayerAction action,
            Dictionary<string, object> boardManagerStateData)
        {
            return null;
        }

    }
}