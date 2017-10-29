using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class SetupAdoptTacticActionHandler:ActionHandler
    {
        public SetupAdoptTacticActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
        {
            throw new NotImplementedException();
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            throw new NotImplementedException();
        }
    }
}
