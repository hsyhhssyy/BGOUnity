using System;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.Util.Input;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    [Obsolete]
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
                _boardBehavior = value;
            }
        }
        

        public override bool OnTriggerClick()
        {
            if (ActionOnMouseClick == null)
            {
                return false;
            }

            ActionOnMouseClick();
            return true;
        }

        public override bool OnTriggerClickOutside()
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
