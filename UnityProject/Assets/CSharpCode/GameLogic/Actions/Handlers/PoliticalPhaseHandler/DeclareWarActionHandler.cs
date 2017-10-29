using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.PoliticalPhaseHandler
{
    public class DeclareWarActionHandler:ActionHandler
    {
        public DeclareWarActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
        {
            return null;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            return null;
        }
    }
}
