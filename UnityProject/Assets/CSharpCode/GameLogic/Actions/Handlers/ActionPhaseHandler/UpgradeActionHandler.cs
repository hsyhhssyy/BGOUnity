using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;
using JetBrains.Annotations;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class UpgradeBuildingActionHandler : ActionHandler
    {
        public UpgradeBuildingActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override System.Collections.Generic.List<PlayerAction> GenerateAction(int playerNo)
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

            var ruleBook = Manager.Civilopedia.GetRuleBook();
            var originalWhite = board.Resource[ResourceType.WhiteMarker];
            var originalRed = board.Resource[ResourceType.RedMarker];

            var resource = board.Resource[ResourceType.Resource];
            var result = new List<PlayerAction>();
            board.AggregateOnBuildingUpgradePair(true,(b,fromCell,toCell) =>
            {
                if (ruleBook.IsMilitary(fromCell.Card))
                {
                    if (originalRed <= 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (originalWhite <= 0)
                    {
                        return true;
                    }
                }

                var pricediff = BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, toCell)
                                - BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, fromCell);
                if (pricediff <= resource)
                {
                    var action = new PlayerAction { ActionType = PlayerActionType.UpgradeBuilding };
                    action.Data[0] = fromCell.Card;
                    action.Data[1] = toCell.Card;
                    action.Data[2] = pricediff;

                    result.Add(action);
                }

                return true;
            });

            return result;
        }
        

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action,
            Dictionary<string, object> boardManagerStateData)
        {
            if (action.ActionType != PlayerActionType.UpgradeBuilding)
            {
                return null;
            }

            var board = Manager.CurrentGame.Boards[playerNo];
            var ruleBook = Manager.Civilopedia.GetRuleBook();
            var response = new ActionResponse {Type = ActionResponseType.ChangeList};


            var fromCard = (CardInfo) action.Data[0];
            var toCard = (CardInfo) action.Data[1];
            var resCost = (int) action.Data[2];

            if (ruleBook.IsMilitary(fromCard))
            {
                var originalRed = board.Resource[ResourceType.RedMarker];
                board.Resource[ResourceType.WhiteMarker] = originalRed - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.RedMarker, originalRed, originalRed - 1));
            }
            else
            {
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));
            }

            Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
            Manager.SimSpendResource(playerNo,
                BuildingType.Mine, ResourceType.ResourceIncrement, resCost, markers);
            response.Changes.Add(GameMove.Production(ResourceType.Resource, 0 - resCost, markers));
            Manager.PerformMarkerChange(playerNo, markers);

            var buildType = ruleBook.GetBuildingType(fromCard);
            var dict = board.Buildings[buildType];
            var fromCell = dict.FirstOrDefault(pair => pair.Value.Card == fromCard).Value;
            var toCell = dict.FirstOrDefault(pair => pair.Value.Card == toCard).Value;

            fromCell.Worker--;
            toCell.Worker++;

            response.Changes.Add(GameMove.Upgrade(buildType, fromCard, toCard, 1));

            return response;
        }
    }
}
