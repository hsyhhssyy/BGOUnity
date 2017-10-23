using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    public class IncreasePopulationActionHandler:ActionHandler
    {
        public IncreasePopulationActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> CheckAbleToPerform(int playerNo)
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

            int yellowMarker = board.Resource[ResourceType.YellowMarker];
            if (yellowMarker <= 0)
            {
                return null;
            }
            int baseFood=Manager.Civilopedia.GetRuleBook().FoodToIncreasePopulation(yellowMarker);
            int calcuatedFood = baseFood;
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

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data)
        {
            if (action.ActionType == PlayerActionType.IncreasePopulation)
            {
                var board = Manager.CurrentGame.Boards[playerNo];
                var response=new ActionResponse();
                response.Type = ActionResponseType.ChangeList;

                int foodCost = (int)action.Data[0];
                Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
                Manager.SimSpendResource(playerNo, BuildingType.Farm, ResourceType.FoodIncrement, foodCost, markers);

                response.Changes.Add(GameMove.Production(ResourceType.Food,0-foodCost,markers));
                Manager.PerformMarkerChange(playerNo,markers);

                int originalWorkerPool = board.Resource[ResourceType.WorkerPool];
                board.Resource[ResourceType.WorkerPool] = originalWorkerPool + 1;

                response.Changes.Add(GameMove.Resource(ResourceType.WorkerPool, originalWorkerPool, originalWorkerPool+1));

                //别忘了掉1白
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));
                
                return response;
            }
            return null;
        }
    }
}
