using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.Controller;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    class BuildAndDestoryActionHandler:ActionHandler
    {
        public BuildAndDestoryActionHandler(GameLogicManager manager) : base(manager)
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

            var workerPool = board.Resource[ResourceType.WorkerPool];
            if (workerPool < 1)
            {
                return null;
            }
            
            var resource = board.Resource[ResourceType.Resource];
            var whiteMarker = board.Resource[ResourceType.WhiteMarker];
            var redMarker = board.Resource[ResourceType.RedMarker];
            var result = board.AggregateOnBuildingCell(new List<PlayerAction>(), (actions, cell) =>
            {
                switch (cell.Card.CardType)
                {
                        case CardType.MilitaryTechAirForce:
                        case CardType.MilitaryTechArtillery:
                        case CardType.MilitaryTechCavalry:
                        case CardType.MilitaryTechInfantry:
                        if (redMarker <=0)
                        {
                            break;
                        }
                        //Disband总是可行的
                        var playerAction = new PlayerAction();
                            playerAction.ActionType = PlayerActionType.Disband;
                            playerAction.Data[0] = cell.Card;
                            actions.Add(playerAction);
                        if (resource >= cell.Card.BuildCost[0])
                        {
                            playerAction = new PlayerAction();
                            playerAction.ActionType = PlayerActionType.BuildBuilding;
                            playerAction.Data[0] = cell.Card;
                            playerAction.Data[1] = cell.Card.BuildCost[0];
                            actions.Add(playerAction);
                        }
                        break;
                    case CardType.ResourceTechFarm:
                    case CardType.ResourceTechMine:
                        if (whiteMarker <= 0)
                        {
                            break;
                        }
                        playerAction = new PlayerAction();
                        playerAction.ActionType = PlayerActionType.Destory;
                        playerAction.Data[0] = cell.Card;
                        actions.Add(playerAction);

                        if (resource >= cell.Card.BuildCost[0])
                        {
                            playerAction = new PlayerAction();
                            playerAction.ActionType = PlayerActionType.BuildBuilding;
                            playerAction.Data[0] = cell.Card;
                            playerAction.Data[1] = cell.Card.BuildCost[0];
                            actions.Add(playerAction);
                        }
                        break;
                    case CardType.UrbanTechLab:
                    case CardType.UrbanTechArena:
                    case CardType.UrbanTechLibrary:
                    case CardType.UrbanTechTemple:
                    case CardType.UrbanTechTheater:
                        if (whiteMarker <= 0)
                        {
                            break;
                        }
                        playerAction = new PlayerAction();
                        playerAction.ActionType = PlayerActionType.Destory;
                        playerAction.Data[0] = cell.Card;
                        actions.Add(playerAction);

                        var urbanLimit = board.Resource[ResourceType.UrbanLimit];
                        if (cell.Worker >= urbanLimit)
                        {
                            break;
                        }
                        if (resource >= cell.Card.BuildCost[0])
                        {
                            playerAction = new PlayerAction();
                            playerAction.ActionType = PlayerActionType.BuildBuilding;
                            playerAction.Data[0] = cell.Card;
                            playerAction.Data[1] = cell.Card.BuildCost[0];
                            actions.Add(playerAction);
                        }
                        break;
                }
                return actions;
            });
            return result;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data)
        {
            if (action.ActionType == PlayerActionType.BuildBuilding)
            {
                
            }else if (action.ActionType == PlayerActionType.Disband || action.ActionType == PlayerActionType.Destory)
            {
                
            }
            return null;
        }
    }
}
