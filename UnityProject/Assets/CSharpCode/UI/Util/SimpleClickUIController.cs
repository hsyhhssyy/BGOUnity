using System;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public abstract class SimpleClickUIController : TtaUIControllerMonoBehaviour
    {
        private bool _refreshRequired;
        private bool _highlight;
        
        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            UIKey = GetUIKey();
            Manager.GameBoardManagerEvent += OnSubscribedGameEvents;
            Manager.Regiseter(this);
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
            }
            else if (args.EventType == GameUIEventType.AllowSelect)
            {
                _highlight = true;
            }
        }

        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        protected abstract String GetUIKey();
        protected abstract void Refresh();

        public override bool OnTriggerEnter()
        {
            var args = new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey);
            AttachDataOnTrySelect(args);
            Broadcast(args);
            return true;
        }

        public override bool OnTriggerExit()
        {
            _highlight = false;
            return true;
        }

        public override bool OnTriggerClick()
        {
            if (!_highlight) return false;

            var args = new ControllerGameUIEventArgs(GameUIEventType.Selected, UIKey);
            AttachDataOnSelected(args);
            Broadcast(args);

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
