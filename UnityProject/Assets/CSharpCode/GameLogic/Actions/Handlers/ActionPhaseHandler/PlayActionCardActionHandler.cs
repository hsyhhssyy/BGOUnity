using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;
using Assets.CSharpCode.UI;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class PlayActionCardActionHandler : ActionHandler
    {
        public PlayActionCardActionHandler(GameLogicManager manager) : base(manager)
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

            var result = new List<PlayerAction>();
            foreach (var handInfo in board.CivilCards)
            {
                if (handInfo.Card.CardType == CardType.Action)
                {
                    if (handInfo.TurnTaken >= Manager.CurrentGame.CurrentRound)
                    {
                        continue;
                    }

                    var valid = true;
                    foreach (var effect in handInfo.Card.ImmediateEffects)
                    {
                        if (!EffectExecutor.CheckEffect(Manager, playerNo, effect))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        var internalAction = IsInternalAction(handInfo.Card);
                        PlayerAction action = new PlayerAction
                        {
                            ActionType = PlayerActionType.PlayActionCard,
                            Internal = internalAction
                        };
                        action.Data[0] = handInfo.Card;
                        result.Add(action);
                    }
                }
            }

            return result;        
        }

        
        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            if (action.ActionType != PlayerActionType.PlayActionCard)
            {
                return null;
            }


            var board = Manager.CurrentGame.Boards[playerNo];
            var card = (CardInfo) action.Data[0];
            var handInfo = board.CivilCards.FirstOrDefault(info =>
                info.Card == card && info.TurnTaken < Manager.CurrentGame.CurrentRound);
            
            if (handInfo == null)
            {
                return new ActionResponse(){Type =  ActionResponseType.ForceRefresh};
            }

            var response = new ActionResponse();

            board.CivilCards.Remove(handInfo);
            response.Changes.Add(GameMove.CardUsed(handInfo));

            var originalWhite = board.Resource[ResourceType.WhiteMarker];
            board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
            response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));


            foreach (var effect in card.ImmediateEffects)
            {
                var moves=EffectExecutor.ExecuteEffect(Manager, playerNo, effect,action);
                if (moves != null)
                {
                    response.Changes.AddRange(moves);
                }
            }

            return response;
        }

        public override ActionResponse PerfromInternalAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData, 
            out List<PlayerAction> followingActions)
        {
            followingActions=new List<PlayerAction>();
            if (action.ActionType != PlayerActionType.PlayActionCard||action.Internal!=true)
            {
                return null;
            }


            var board = Manager.CurrentGame.Boards[playerNo];
            var card = (CardInfo)action.Data[0];
            var rulebook = Manager.Civilopedia.GetRuleBook();
            var handInfo = board.CivilCards.FirstOrDefault(info =>
                info.Card == card && info.TurnTaken < Manager.CurrentGame.CurrentRound);

            if (handInfo == null)
            {
                return new ActionResponse() { Type = ActionResponseType.ForceRefresh };
            }

            var response = new ActionResponse();
            response.Type = ActionResponseType.Accepted;

            var resource = board.Resource[ResourceType.Resource];
            var science = board.Resource[ResourceType.Science];
            var scienceForMili = board.Resource[ResourceType.ScienceForMilitary];
            var scienceForSpec = board.Resource[ResourceType.ScienceForSpecialTech];

            foreach (var effect in card.ImmediateEffects)
            {
                var effect1 = effect;
                List<PlayerAction> actions = followingActions;

                switch (effect.FunctionId)
                {
                    case CardEffectType.E402:
                    {
                        board.AggregateOnBuildingCell(true, (b, cell) =>
                        {
                            if (cell.Card.CardType != CardType.ResourceTechFarm &&
                                cell.Card.CardType != CardType.ResourceTechMine)
                            {
                                return true;
                            }

                            var buildPrice = BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, cell) -
                                             effect1.Data[0];
                            if (buildPrice <= resource)
                            {
                                var actualAction = new PlayerAction { ActionType = 
                                    PlayerActionType.BuildBuilding };
                                actualAction.Data[0] = cell.Card;
                                actualAction.Data[1] = buildPrice;
                                actualAction.Data[2] = card;

                                actions.Add(action);
                            }
                            return true;
                        });
                        break;
                    } 
                    case CardEffectType.E403:
                    {
                        board.AggregateOnBuildingUpgradePair(true, (b, fromCell, toCell) =>
                        {
                            if (fromCell.Card.CardType!= CardType.ResourceTechFarm&&
                            fromCell.Card.CardType!= CardType.ResourceTechMine)
                            {
                                    return true;
                            }

                            var pricediff = BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, toCell)
                                            - BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, fromCell)
                                            - effect1.Data[0];

                            if (pricediff <= resource)
                            {
                                var actualAction = new PlayerAction { ActionType = PlayerActionType.UpgradeBuilding };
                                actualAction.Data[0] = fromCell.Card;
                                actualAction.Data[1] = toCell.Card;
                                actualAction.Data[2] = pricediff;
                                actualAction.Data[3] = card;

                                actions.Add(action);
                            }

                            return true;
                        });
                        break;
                    }
                    case CardEffectType.E404:
                    {
                        board.AggregateOnBuildingCell(true, (b, cell) =>
                        {
                            if (!rulebook.IsUrban(cell.Card))
                            {
                                return true;
                            }

                            var buildPrice = BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, cell) -
                                             effect1.Data[0];
                            if (buildPrice <= resource)
                            {
                                var actualAction = new PlayerAction
                                {
                                    ActionType =
                                        PlayerActionType.BuildBuilding
                                };
                                actualAction.Data[0] = cell.Card;
                                actualAction.Data[1] = buildPrice;
                                actualAction.Data[2] = card;

                                actions.Add(action);
                            }
                            return true;
                        });
                        break;
                    }
                    case CardEffectType.E405:
                    {
                        board.AggregateOnBuildingUpgradePair(true, (b, fromCell, toCell) =>
                        {
                            if (!rulebook.IsUrban(fromCell.Card))
                            {
                                return true;
                            }

                            var pricediff = BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, toCell)
                                            - BuildAndDestoryActionHandler.GetBuildingCost(Manager, playerNo, fromCell)
                                            - effect1.Data[0];

                            if (pricediff <= resource)
                            {
                                var actualAction = new PlayerAction { ActionType = PlayerActionType.UpgradeBuilding };
                                actualAction.Data[0] = fromCell.Card;
                                actualAction.Data[1] = toCell.Card;
                                actualAction.Data[2] = pricediff;
                                actualAction.Data[3] = card;

                                actions.Add(actualAction);
                            }
                            return true;
                        });
                        break;
                    }
                    case CardEffectType.E408:
                    {
                        board.CivilCards.Aggregate(true, (b, hInfo) =>
                        {
                            var actionType = PlayTechCardActionHandler.CanDevelopThisTechCard(hInfo, rulebook, board,
                                science+effect.Data[0],
                                scienceForSpec + effect.Data[0], scienceForMili + effect.Data[0]);
                            if (actionType == PlayerActionType.DevelopTechCard
                            )
                            {
                                PlayerAction actualAction = new PlayerAction { ActionType = PlayerActionType.DevelopTechCard };
                                actualAction.Data[0] = handInfo.Card;
                                actualAction.Data[1] = handInfo.Card.ResearchCost[0]- effect.Data[0];
                                actions.Add(actualAction);
                            }
                            else if (actionType == PlayerActionType.Revolution)
                            {
                                PlayerAction actualAction = new PlayerAction { ActionType = PlayerActionType.Revolution };
                                actualAction.Data[0] = handInfo.Card;
                                actualAction.Data[1] = handInfo.Card.ResearchCost[1] - effect.Data[0];
                                actions.Add(actualAction);
                            }
                            return true;
                        });
                        break;
                    }
                }
            }

            return response;
        }

        
        /// <summary>
        /// 判断该卡牌所含有的效果类型是否应被解释为InternalAction
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IsInternalAction(CardInfo card)
        {
            if (card.ImmediateEffects != null)
            {
                if (card.ImmediateEffects.Any(e => e.FunctionId == CardEffectType.E402
                                                   || e.FunctionId == CardEffectType.E403 ||
                                                   e.FunctionId == CardEffectType.E404 ||
                                                   e.FunctionId == CardEffectType.E405||
                e.FunctionId == CardEffectType.E408 ))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
