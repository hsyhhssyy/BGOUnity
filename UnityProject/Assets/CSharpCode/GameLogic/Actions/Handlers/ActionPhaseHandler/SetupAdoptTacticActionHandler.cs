using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler
{
    public class SetupAdoptTacticActionHandler:ActionHandler
    {
        public SetupAdoptTacticActionHandler(GameLogicManager manager) : base(manager)
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
            var redMarker = board.Resource[ResourceType.RedMarker];
            if (redMarker <= 0)
            {
                return null;
            }

            var result = new List<PlayerAction>();

            var currentTactic = board.Tactic;
            var ownTactic = board.MilitaryCards.Where(c => c.Card.CardType == CardType.Tactic).Select(c => c.Card)
                .Distinct().ToList();
            var sharedTactic = Manager.CurrentGame.SharedTactics;

            if (redMarker >= 1)
            {
                foreach (var tac in ownTactic)
                {
                    if (tac != currentTactic)
                    {
                        var action = new PlayerAction {ActionType = PlayerActionType.SetupTactic};
                        action.Data[0] = tac;
                        result.Add(action);
                    }
                }
            }

            if (redMarker >= 2)
            {
                foreach (var tac in sharedTactic)
                {
                    if (tac != currentTactic&&
                        (!ownTactic.Contains(tac)))
                    {
                        var action = new PlayerAction {ActionType = PlayerActionType.AdoptTactic};
                        action.Data[0] = tac;
                        result.Add(action);
                    }
                }
            }

            return result;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> boardManagerStateData)
        {
            if (action.ActionType != PlayerActionType.AdoptTactic && action.ActionType != PlayerActionType.SetupTactic)
            {
                return null;
            }

            var board = Manager.CurrentGame.Boards[playerNo];
            //var ruleBook = Manager.Civilopedia.GetRuleBook();
            var response = new ActionResponse {Type = ActionResponseType.ChangeList};
            var tacCard = (CardInfo) action.Data[0];

            if (action.ActionType == PlayerActionType.AdoptTactic)
            {
                board.Tactic = tacCard;
                response.Changes.Add(GameMove.SetupTactic(tacCard));

                var originalRed = board.Resource[ResourceType.RedMarker];
                board.Resource[ResourceType.WhiteMarker] = originalRed - 2;
                response.Changes.Add(GameMove.Resource(ResourceType.RedMarker, originalRed, originalRed - 2));

            }
            else
            {
                board.Tactic = tacCard;
                response.Changes.Add(GameMove.SetupTactic(tacCard));

                var originalRed = board.Resource[ResourceType.RedMarker];
                board.Resource[ResourceType.WhiteMarker] = originalRed - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.RedMarker, originalRed, originalRed - 1));

                //打牌
                var infoList = board.MilitaryCards.Where(info => info.Card == tacCard).ToList();
                infoList.Sort((a, b) => a.TurnTaken.CompareTo(b.TurnTaken));
                var handInfo = infoList.FirstOrDefault();

                response.Changes.Add(GameMove.CardUsed(handInfo));

            }


            return response;
        }
    }
}
