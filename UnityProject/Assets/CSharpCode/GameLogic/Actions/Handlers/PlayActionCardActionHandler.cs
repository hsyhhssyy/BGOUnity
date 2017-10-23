using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    public class PlayActionCardActionHandler : ActionHandler
    {
        public PlayActionCardActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> CheckAbleToPerform(int playerNo)
        {
            return null;        
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data)
        {
            return null;
        }
    }
}
