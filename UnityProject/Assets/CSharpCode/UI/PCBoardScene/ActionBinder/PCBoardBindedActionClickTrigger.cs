using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class PCBoardBindedActionClickTrigger : InputActionTriggerMonoBehaviour
    {

        private PlayerAction _action;

        private PCBoardBehavior _boardBehavior;

        public void Bind(PlayerAction action, PCBoardBehavior boardBehavior)
        {
            Controller = boardBehavior.ActionTriggerController;
            this._boardBehavior = boardBehavior;
            this._action = action;
        }

        public void Unbind()
        {
            _action = null;
            Controller = null;
        }

        public override bool OnMouseClick()
        {
            if (_action == null)
            {
                return false;
            }

            if (_action.ActionType == PlayerActionType.ProgramDelegateAction)
            {
                var act = _action.Data[0] as System.Action;
                if (act != null)
                {
                    act();
                }
            }
            else
            {
                _boardBehavior.TakeAction(_action);
            }

            return true;
        }
    }
}
