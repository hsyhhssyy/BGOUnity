using System;
using System.Collections.Generic;
using HSYErpBase.Wcf;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.ActionDefinition.Handlers
{
    public class EndPhaseActionHandler : ActionHandler
    {
        public EndPhaseActionHandler(GameManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> CheckAbleToPerform(int playerNo)
        {
            var actions= new List<PlayerAction>();
            if (Manager.CurrentGame.CurrentPlayer != playerNo)
            {
                return actions;
            }
            
            if (Manager.CurrentGame.CurrentPhase == TtaPhase.ActionPhase||Manager.CurrentGame.CurrentPhase== TtaPhase.FirstTurnActionPhase)
            {
                var action = new PlayerAction {ActionType = PlayerActionType.EndActionPhase};
                action.Data.Add(0,new Dictionary<CardInfo,int>() { { CardInfo.UnknownMilitaryCard(Age.A),1 }});
                actions.Add(action);
                action = new PlayerAction {ActionType = PlayerActionType.ResetActionPhase};
                actions.Add(action);
            }
            else if (Manager.CurrentGame.CurrentPhase == TtaPhase.PoliticalPhase)
            {
                var action = new PlayerAction {ActionType = PlayerActionType.PassPoliticalPhase};
                actions.Add(action);
                action = new PlayerAction {ActionType = PlayerActionType.Resign};
                actions.Add(action);
            }
            return actions;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data, WcfContext context)
        {
            if (action.ActionType == PlayerActionType.EndActionPhase)
            {
                if (Manager.CurrentGame.CurrentPlayer != playerNo)
                {
                    return new ActionResponse() {Type = ActionResponseType.ForceRefresh};
                }

                var response=Manager.ExecuteProduction(playerNo);
                
                if (Manager.CurrentGame.CurrentPhase != TtaPhase.FirstTurnProductionPhase)
                {
                    Manager.NextTurn(true,context);
                }
                else
                {
                    if (playerNo + 1 >= Manager.CurrentGame.Room.PlayerMax)
                    {
                        Manager.NextTurn(true,context);
                        //如果没进入AgeI，补一个NextAge（其实没进应该是常态）
                        if (Manager.CurrentGame.CurrentAge != Age.I)
                        {
                            Manager.NextAge();
                        }
                    }
                    else
                    {
                        Manager.CurrentGame.CurrentPlayer = playerNo + 1;
                        //因为没调用NextTurn，因此要补一个Phase切换
                        Manager.CurrentGame.CurrentPhase = TtaPhase.FirstTurnActionPhase;
                    }
                }

                return response;
            }
            
            return null;
        }

    }
}