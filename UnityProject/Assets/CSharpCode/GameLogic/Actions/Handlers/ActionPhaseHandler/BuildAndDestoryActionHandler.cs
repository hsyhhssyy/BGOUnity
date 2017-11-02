using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    class BuildAndDestoryActionHandler:ActionHandler
    {
        public BuildAndDestoryActionHandler(GameLogicManager manager) : base(manager)
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

            var workerPool = board.Resource[ResourceType.WorkerPool];
            var resource = board.Resource[ResourceType.Resource];
            var whiteMarker = board.Resource[ResourceType.WhiteMarker];
            var redMarker = board.Resource[ResourceType.RedMarker];
            var urbanLimit = board.Resource[ResourceType.UrbanLimit];

            var result = board.AggregateOnBuildingCell(new List<PlayerAction>(), (actions, cell) =>
            {
                TryAddDisbandOrDestory(whiteMarker,redMarker,cell,actions);
                TryAddBuild(whiteMarker,redMarker,urbanLimit,resource,workerPool,cell,actions);
                return actions;
            });
            return result;
        }

        private void TryAddDisbandOrDestory(int whiteMarker, int redMarker, BuildingCell cell, List<PlayerAction> actions)
        {
            var isMilitary = Manager.Civilopedia.GetRuleBook().IsMilitary(cell.Card);
            if (isMilitary)
            {
                if (redMarker > 0)
                {
                    var playerAction = new PlayerAction();
                    playerAction.ActionType = PlayerActionType.Disband;
                    playerAction.Data[0] = cell.Card;
                    actions.Add(playerAction);
                }
            }
            else
            {
                if (whiteMarker > 0)
                {
                    var playerAction = new PlayerAction();
                    playerAction.ActionType = PlayerActionType.Destory;
                    playerAction.Data[0] = cell.Card;
                    actions.Add(playerAction);
                }
            }
        }

        private void TryAddBuild(int whiteMarker, int redMarker,int urbanLimit,int resource,int workerPool, BuildingCell cell,
            List<PlayerAction> actions)
        {
            var isMilitary = Manager.Civilopedia.GetRuleBook().IsMilitary(cell.Card);
            var isUrban = Manager.Civilopedia.GetRuleBook().IsUrban(cell.Card);

            if (workerPool < 1)
            {
                return;
            }

            if (isMilitary&&redMarker<0)
            {
                return;
            }

            if (whiteMarker < 0)
            {
                return;
            }

            if (isUrban)
            {
                if (cell.Worker >= urbanLimit)
                {
                    return;
                }
            }
            if (resource >= cell.Card.BuildCost[0])
            {
                var playerAction = new PlayerAction();
                playerAction.ActionType = PlayerActionType.BuildBuilding;
                playerAction.Data[0] = cell.Card;
                playerAction.Data[1] = cell.Card.BuildCost[0];
                actions.Add(playerAction);
            }
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            var board = Manager.CurrentGame.Boards[playerNo];
            var response = new ActionResponse {Type = ActionResponseType.ChangeList};

            if (action.ActionType == PlayerActionType.BuildBuilding)
            {
                CardInfo card=(CardInfo)action.Data[0];
                int resCost = (int)action.Data[1];

                Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
                Manager.SimSpendResource(playerNo, BuildingType.Mine, ResourceType.ResourceIncrement, resCost, markers);
                response.Changes.Add(GameMove.Production(ResourceType.Resource, 0 - resCost, markers));
                Manager.PerformMarkerChange(playerNo, markers);

                var to=board.AggregateOnBuildingCell(0, (i, cell) =>
                {
                    if (cell.Card == card)
                    {
                        cell.Worker++;
                        return cell.Worker;
                    }
                    return i;
                });
                response.Changes.Add(GameMove.Build(card, to-1, to));

                int originalWorkerPool = board.Resource[ResourceType.WorkerPool];
                board.Resource[ResourceType.WorkerPool] = originalWorkerPool - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WorkerPool, originalWorkerPool, originalWorkerPool - 1));
                
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));

                return response;
            }
            else if (action.ActionType == PlayerActionType.Disband || action.ActionType == PlayerActionType.Destory)
            {
                CardInfo card = (CardInfo)action.Data[0];
                var to = board.AggregateOnBuildingCell(0, (i, cell) =>
                {
                    if (cell.Card == card)
                    {
                        cell.Worker--;
                        return cell.Worker;
                    }
                    return i;
                });
                response.Changes.Add(GameMove.Build(card, to + 1, to));

                int originalWorkerPool = board.Resource[ResourceType.WorkerPool];
                board.Resource[ResourceType.WorkerPool] = originalWorkerPool + 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WorkerPool, originalWorkerPool, originalWorkerPool + 1));
                
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));

                return response;
            }
            return null;
        }

        /// <summary>
        /// 确定一个建筑物的造价（矿物）
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static int GetBuildingCost(GameLogicManager manager, int playerNo, BuildingCell cell)
        {
            return cell.Card.BuildCost[0];
        }
    }
}
