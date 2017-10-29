using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class BuildWonderActionHandler:ActionHandler
    {
        public BuildWonderActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
        {
            if (Manager.CurrentGame.CurrentPhase != TtaPhase.ActionPhase ||
                Manager.CurrentGame.CurrentPlayer != playerNo)
            {
                return null;
            }


            var board = Manager.CurrentGame.Boards[playerNo];
            var whiteMarker = board.Resource[ResourceType.WhiteMarker];
            if (whiteMarker <= 0)
            {
                return null;
            }

            if (board.ConstructingWonder == null)
            {
                return null;
            }

            var result = new List<PlayerAction>();

            var costs = board.ConstructingWonder.BuildCost;
            var resource = board.Resource[ResourceType.Resource];
            var step = board.ConstructingWonderSteps;


            //找到最大的E410值

            var e410Value = board.EffectPool.FilterEffect(CardEffectType.E410).Max(t => t.Data[0]);

                int totalCost =0;
            int whiteCost = 0;
            for (var index = step; index < costs.Count; index++)
            {
                var c = costs[index];
                totalCost += c;
                if (resource < totalCost)
                {
                    break;
                }
                whiteCost++;
                int actualWhiteCost = whiteCost - e410Value;
                if (actualWhiteCost < 0)
                {
                    actualWhiteCost = 0;
                }

                if (actualWhiteCost > whiteMarker)
                {
                    break;
                }


            }

            return result;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            throw new NotImplementedException();
        }
    }
}
