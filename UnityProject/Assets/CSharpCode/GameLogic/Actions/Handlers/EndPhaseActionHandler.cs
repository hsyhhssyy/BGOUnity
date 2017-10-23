using System;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI;


namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    public class EndPhaseActionHandler : ActionHandler
    {
        public EndPhaseActionHandler(GameLogicManager manager) : base(manager)
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

        
        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data)
        {
            //TODO 未来可能会改为重置不需要提交给服务器，但目前没有办法实现
            if (action.ActionType == PlayerActionType.EndActionPhase)
            {
                //本地执行一个Production但是不含抽牌，提交给服务器
                //服务器特化处理本action，完成抽牌并返回一个action（带抽牌）
                //本地接收到Response后，再给出抽到的牌

                //接下来通过刷新界面获得卡牌列的变化，并进入回合外
                //决定下一位玩家是由服务器来做的

                //下面是代码实现

                if (Manager.CurrentGame.CurrentPhase == TtaPhase.FirstTurnActionPhase)
                {
                    Manager.CurrentGame.CurrentPhase = TtaPhase.FirstTurnProductionPhase;
                }
                var response=Manager.ExecuteProduction(playerNo);
                return response;
            }else if (action.ActionType == PlayerActionType.ResetActionPhase)
            {
                //特殊逻辑
                return new ActionResponse() {Type = ActionResponseType.ForceRefresh};
            }
            return null;
        }

    }
}