using System;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.Util.Controller
{
    /// <summary>
    /// 一个Controller，除了DisplayOnlyUIController提供的功能以外，还能接受用户点击。
    /// </summary>
    public abstract class SimpleClickUIController : DisplayOnlyUIController
    {
        public override String UIKey
        {
            get { return GetUIKey(); }
            protected set
            {
                //Invalid set
            }
        }

        [UsedImplicitly]
        public override void Start()
        {
            base.Start();
        }
        

        protected abstract String GetUIKey();

        public override bool OnTriggerEnter()
        {
            var args = new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey);
            AttachDataOnTrySelect(args);
            Channel.Broadcast(args);
            return true;
        }

        public override bool OnTriggerExit()
        {
            IsHoveringAndAllowSelected = false;
            return false;
        }

        public override bool OnTriggerClick()
        {
            if (!IsHoveringAndAllowSelected) return false;

            var args = new ControllerGameUIEventArgs(GameUIEventType.Selected, UIKey);
            AttachDataOnSelected(args);
            Channel.Broadcast(args);

            return true;
        }

        protected virtual void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
        }
        protected virtual void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
        }
    }
}
