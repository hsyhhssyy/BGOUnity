using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
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

                    PlayerAction action = new PlayerAction {ActionType = PlayerActionType.PlayActionCard};
                    action.Data[0] = handInfo.Card;
                    result.Add(action);
                }
            }

            return result;        
        }

        
        public override bool CheckAction(int playerNo, PlayerAction action, Dictionary<String, object> data)
        {
            //拉比换领袖，必须要为Action不停的附加数据,直到用户做出选择
            if (action.ActionType == PlayerActionType.ElectLeader)
            {
                var leaderCard = (CardInfo) action.Data[0];
                if (leaderCard.SustainedEffects.Any(e => e.FunctionId == CardEffectType.E603))
                {
                    if (!data.ContainsKey("E603Choice"))
                    {
                        //InternalAction
                    }
                }
            }
            return true;
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


            //这儿就厉害了
            foreach (var effect in card.ImmediateEffects)
            {
                var moves=EffectExecutor.ExecuteEffect(Manager, playerNo, effect);
                if (moves != null)
                {
                    response.Changes.AddRange(moves);
                }
            }

            return response;
        }
    }
}
