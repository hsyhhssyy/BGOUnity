using System;
using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class IncreasePopulationActionHandler:ActionHandler
    {
        public IncreasePopulationActionHandler(GameLogicManager manager) : base(manager)
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

            if (board.Resource[ResourceType.WhiteMarker] <= 0)
            {
                return null;
            }
            
            int calcuatedFood = CalcuatedFoodCost(playerNo);
            if (board.Resource[ResourceType.Food] >= calcuatedFood)
            {
                var action = new PlayerAction()
                {
                    ActionType = PlayerActionType.IncreasePopulation,
                };
                action.Data[0] = calcuatedFood;
                return new List<PlayerAction>() {action};
            }
            return null;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            if (action.ActionType == PlayerActionType.IncreasePopulation)
            {
                var board = Manager.CurrentGame.Boards[playerNo];
                var response = new ActionResponse {Type = ActionResponseType.ChangeList};

                response.Changes.AddRange(IncreasePopulation(playerNo));

                 //别忘了掉1白
                 var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));
                
                return response;
            }
            return null;
        }

        /// <summary>
        /// 返回当前拉一个1人需要的粮食消耗
        /// </summary>
        /// <returns></returns>
        public static int CalcuatedFoodCost(int playerNo)
        {
            var board = GameLogicManager.CurrentManager.CurrentGame.Boards[playerNo];
            int yellowMarker = board.Resource[ResourceType.YellowMarker];
            if (yellowMarker <= 0)
            {
                return Int32.MaxValue;
            }
            int baseFood = GameLogicManager.CurrentManager.Civilopedia.GetRuleBook().FoodToIncreasePopulation(yellowMarker);
            int calcuatedFood = baseFood;
            foreach (var effect in
                board.EffectPool.FilterEffect(CardEffectType.E400))
            {
                calcuatedFood -= effect.Data[0];
            }
            if (calcuatedFood < 0)
            {
                calcuatedFood = 0;
            }
            //Calc
            return calcuatedFood;
        }

        /// <summary>
        /// 消耗合理的粮食，并增加1人口
        /// </summary>
        /// <returns></returns>
        public static List<GameMove> IncreasePopulation(int playerNo)
        {
            var result = new List<GameMove>();
            var board = GameLogicManager.CurrentManager.CurrentGame.Boards[playerNo];

            int calcuatedFood = CalcuatedFoodCost(playerNo);
            Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
            GameLogicManager.CurrentManager.SimSpendResource(
                GameLogicManager.CurrentManager.CurrentGame.Boards.IndexOf(board), BuildingType.Farm,
                ResourceType.FoodIncrement, calcuatedFood, markers);
            result.Add(GameMove.Production(ResourceType.Food, 0 - calcuatedFood, markers));
            GameLogicManager.CurrentManager.PerformMarkerChange(playerNo, markers);

            int originalWorkerPool = board.Resource[ResourceType.WorkerPool];
            board.Resource[ResourceType.WorkerPool] = originalWorkerPool + 1;
            result.Add(GameMove.Resource(ResourceType.WorkerPool, originalWorkerPool, originalWorkerPool + 1));


            return result;
        }
    }
}
