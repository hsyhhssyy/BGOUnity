using System;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public abstract class DisplayOnlyUIController : TtaUIControllerMonoBehaviour
    {
        protected bool RefreshRequired;

        private bool _highlight;
        protected bool Highlight
        {
            get { return _highlight; }
            set {
                if (_highlight != value)
                {
                    _highlight = value;
                    OnHighlightChanged();
                }
            }
        }

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public virtual void Start()
        {
            Manager.Regiseter(this);
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                RefreshRequired = true;
                this.gameObject.SetActive(true);
            }

            if (args.UIKey.Contains(Guid))
            {
                if (args.EventType == GameUIEventType.AllowSelect)
                {
                    Highlight = true;
                }
            }
        }

        [UsedImplicitly]
        public virtual void FixedUpdate()
        {
            if (RefreshRequired)
            {
                RefreshRequired = false;
                Refresh();
            }
        }

        protected abstract void Refresh();

        protected virtual void OnHighlightChanged()
        {
            //Do nothing
        }
    }
}
