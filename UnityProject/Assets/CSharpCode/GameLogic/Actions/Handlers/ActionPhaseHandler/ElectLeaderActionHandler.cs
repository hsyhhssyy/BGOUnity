using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class ElectLeaderActionHandler:ActionHandler
    {
        public ElectLeaderActionHandler(GameLogicManager manager) : base(manager)
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
            var leaders = board.CivilCards.Where(handInfo => handInfo.Card.CardType == CardType.Leader);
            foreach (var leaderCard in leaders)
            {
                var action=new PlayerAction();
                action.ActionType = PlayerActionType.ElectLeader;
                action.Data[0] = leaderCard;
                result.Add(action);
            }

            return result;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            if (action.ActionType != PlayerActionType.ElectLeader)
            {
                return null;
            }

            var board = Manager.CurrentGame.Boards[playerNo];
            var response = new ActionResponse();
            var ruleBook = Manager.Civilopedia.GetRuleBook();
            response.Type = ActionResponseType.ChangeList;

            var leaderCard = (CardInfo) action.Data[0];
            if (board.Leader != null)
            {
                board.EffectPool.RemoveCardInfo(board.Leader);
                //可能需要消耗一白
                if (!ruleBook.RuleFlags.Contains(TtaRuleBook.ReturnCivilCostOnReplaceLeader))
                {
                    var originalWhite = board.Resource[ResourceType.WhiteMarker];
                    board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                    response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));
                }
            }
            else
            {
                //消耗一白
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));
            }
            board.EffectPool.AddCardInfo(leaderCard);
            board.Leader = leaderCard;
            response.Changes.Add(GameMove.ElectLeader(leaderCard));
            
            return response;
        }
    }
}
