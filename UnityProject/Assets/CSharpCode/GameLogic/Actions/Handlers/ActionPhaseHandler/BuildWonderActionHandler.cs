using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;

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

            int totalCost = 0;
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

                var action = new PlayerAction {ActionType = PlayerActionType.BuildWonder};
                action.Data[0] = board.ConstructingWonder;
                action.Data[1] = index - step + 1;
                action.Data[2] = totalCost;
                action.Data[3] = actualWhiteCost;
                result.Add(action);
            }

            return result;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            if (action.ActionType != PlayerActionType.BuildWonder)
            {
                return null;
            }

            var board = Manager.CurrentGame.Boards[playerNo];
            var response = new ActionResponse {Type = ActionResponseType.ChangeList};
            //var ruleBook = Manager.Civilopedia.GetRuleBook();


            //扣除白点
            var originalWhite = board.Resource[ResourceType.WhiteMarker];
            var destWhite = originalWhite - (int)action.Data[3];
            board.Resource[ResourceType.WhiteMarker] = destWhite;
            response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, destWhite));


            //扣除资源
            var cost = (int)action.Data[2];
            Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
            Manager.SimSpendResource(playerNo, BuildingType.Mine, ResourceType.ResourceIncrement, cost, markers);
            response.Changes.Add(GameMove.Production(ResourceType.Resource, 0 - cost, markers));
            Manager.PerformMarkerChange(playerNo, markers);

            //增加级数
            response.Changes.AddRange(IncreaseConstructionStep((int)action.Data[1], board));
            
            return response;
        }

        public static List<GameMove> IncreaseConstructionStep(int step, TtaBoard board)
        {
            var result = new List<GameMove>();

            board.ConstructingWonderSteps += step;
            result.Add(GameMove.ConstructWonder(step));

            if (board.ConstructingWonderSteps >= board.ConstructingWonder.BuildCost.Count)
            {
                var card = board.ConstructingWonder;
                board.CompletedWonders.Add(card);
                board.ConstructingWonder = null;
                board.ConstructingWonderSteps = 0;
                board.EffectPool.AddCardInfo(card);

                result.Add(GameMove.WonderComplete(card));
            }

            return result;
        }
        /// <summary>
        /// 返回从当前级别开始，连续建造奇迹N级所需的白点消耗
        /// </summary>
        /// <param name="step"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int WhiteMarkerCost(int step, TtaBoard board)
        {
            if (board.ConstructingWonder == null)
            {
                return 0;
            }

            return 0;
        }
        /// <summary>
        /// 返回建造当前奇迹第N级所需的资源
        /// </summary>
        /// <param name="step"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int ResourceCostOnStep(int step, TtaBoard board)
        {
            if (board.ConstructingWonder == null)
            {
                return 0;
            }

            return board.ConstructingWonder.BuildCost[step];
        }
        /// <summary>
        /// 返回从当前级别开始，连续建造奇迹N级所需的cost
        /// </summary>
        /// <param name="step"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int ResourceCost(int step, TtaBoard board)
        {
            var costs = board.ConstructingWonder.BuildCost;
            var totalCost = 0;
            for (var index = 0; index < step; index++)
            {
                var level = board.ConstructingWonderSteps + index;
                if (level >= costs.Count)
                {
                    break;
                }
                var c = ResourceCostOnStep(level, board);
                totalCost += c;
            }
            return totalCost;
        }
    }
}
