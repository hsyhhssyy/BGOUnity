using System;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public abstract class DisplayOnlyUIController : TtaUIControllerMonoBehaviour
    {
        private bool _refreshRequired;
        
        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public virtual void Start()
        {
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
        }

        [UsedImplicitly]
        public virtual void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        protected abstract void Refresh();
        
    }
}
