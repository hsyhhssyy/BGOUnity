using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class UpgradeBuildingActionHandler:ActionHandler
    {
        public UpgradeBuildingActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
        {

        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            throw new NotImplementedException();
        }
    }
}
