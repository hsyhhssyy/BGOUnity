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

        public override ActionResponse PerfromInternalAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData, out List<PlayerAction> followingActions)
        {
            followingActions=new List<PlayerAction>();
            if (action.ActionType != PlayerActionType.PlayActionCard)
            {
                return null;
            }


            var board = Manager.CurrentGame.Boards[playerNo];
            var card = (CardInfo)action.Data[0];
            var handInfo = board.CivilCards.FirstOrDefault(info =>
                info.Card == card && info.TurnTaken < Manager.CurrentGame.CurrentRound);

            if (handInfo == null)
            {
                return new ActionResponse() { Type = ActionResponseType.ForceRefresh };
            }

            var response = new ActionResponse();
            response.Type = ActionResponseType.Accepted;


            foreach (var effect in card.ImmediateEffects)
            {
                switch (effect.FunctionId)
                {
                    case CardEffectType.E402:
                        //TODO 找到所有可以升级的建筑物加入Action
                        break;
                    case CardEffectType.E403:
                        //TODO 找到所有可以升级的建筑物加入Action
                        break;
                    case CardEffectType.E404:
                        //TODO 找到所有可以升级的建筑物加入Action
                        break;
                    case CardEffectType.E405:
                        //TODO 找到所有可以升级的建筑物加入Action
                        break;
                    case CardEffectType.E408:
                        //TODO 找到所有可以升级的建筑物加入Action
                        break;
                }
            }

            return response;
        }

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
