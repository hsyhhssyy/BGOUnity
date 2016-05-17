using System;
using Assets.CSharpCode.UI.Util;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class SpecificCodeActionTrigger : InputActionTriggerMonoBehaviour
    {

        public Action ActionOnMouseClick;
        public Action ActionOnMouseClickOutside;

        private PCBoardBehavior _boardBehavior;

        public PCBoardBehavior BoardBehavior
        {
            get
            {
                return _boardBehavior;
            }
            set
            {
                Controller = null;
                _boardBehavior = value;
                Controller = value == null ? null : _boardBehavior.ActionTriggerController;
            }
        }
        

        public override bool OnMouseClick()
        {
            if (ActionOnMouseClick == null)
            {
                return false;
            }

            ActionOnMouseClick();
            return true;
        }

        public override bool OnMouseClickOutside()
        {
            if (ActionOnMouseClickOutside == null)
            {
                return false;
            }

            ActionOnMouseClickOutside();
            return true;
        }
    }
}
