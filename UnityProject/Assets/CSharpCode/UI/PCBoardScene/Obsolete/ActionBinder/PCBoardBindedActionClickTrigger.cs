using System;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.Util.Input;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    [Obsolete]
    // ReSharper disable once InconsistentNaming
    public class PCBoardBindedActionClickTrigger : InputActionTriggerMonoBehaviour
    {

        private PlayerAction _action;

        private PCBoardBehavior _boardBehavior;

        public void Bind(PlayerAction action, PCBoardBehavior boardBehavior)
        {
            this._boardBehavior = boardBehavior;
            this._action = action;
        }

        public void Unbind()
        {
            _action = null;
        }

        public override bool OnTriggerClick()
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
                _boardBehavior.TakeAction(_action,null);
            }

            return true;
        }
    }
}
