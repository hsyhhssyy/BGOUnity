using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class ActionPhaseInternalQueryHandler:GameBoardStateHandler
    {
        public ActionPhaseInternalQueryHandler(GameBoardManager manager) : base(manager)
        {
        }

        public override void EnteringState()
        {
            var action = StateData[ActionPhaseChooseTargetStateHandler.StateNameTriggerAction];
            var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
            msg.AttachedData.Add("PlayerAction", action);
            Channel.Broadcast(msg);
        }

        public override void LeaveState()
        {
            if (SavedActions != null)
            {
                CurrentGame.PossibleActions = SavedActions;
            }
        }

        public List<PlayerAction> SavedActions=new List<PlayerAction>();

        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
        {
            if (args.EventType == GameUIEventType.ReportInternalAction)
            {
                //进行一番处理
                SavedActions = CurrentGame.PossibleActions;
                CurrentGame.PossibleActions = args.AttachedData["Actions"] as List<PlayerAction>;

                //切换状态
                Manager.SwitchState(GameManagerState.ActionPhaseChooseTarget, CreateStateData(args));
            }
        }

        public override void OnUnityUpdate()
        {
        }

        private Dictionary<string, object> CreateStateData(GameUIEventArgs args)
        {
            var dict = new Dictionary<string, object>();

            var actions = args.AttachedData["Actions"] as List<PlayerAction>;
            dict.Add(ActionPhaseChooseTargetStateHandler.StateNameAvailableActions,
                actions);

            dict.Add(ActionPhaseChooseTargetStateHandler.StateNameTriggerCard,
                 StateData[ActionPhaseChooseTargetStateHandler.StateNameTriggerCard]);

            dict.Add(ActionPhaseChooseTargetStateHandler.StateNameTriggerAction, 
                StateData[ActionPhaseChooseTargetStateHandler.StateNameTriggerAction]);
            return dict;
        }
    }
}
