using System;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    /// <summary>
    /// 仅用于显示的UiController，他能够响应Refresh事件
    /// </summary>
    public abstract class DisplayOnlyUIController : TtaUIControllerMonoBehaviour
    {
        protected bool RefreshRequired;

        private bool _isHoveringAndAllowSelected;
        /// <summary>
        /// 表示该按钮是否允许被选中，隐含了当前鼠标指针就在该按钮上的
        /// </summary>
        protected bool IsHoveringAndAllowSelected
        {
            get { return _isHoveringAndAllowSelected; }
            set {
                if (_isHoveringAndAllowSelected != value)
                {
                    _isHoveringAndAllowSelected = value;
                    OnHoveringHighlightChanged();
                }
            }
        }

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public virtual void Start()
        {
            Manager.Regiseter(this);
            OnHoveringHighlightChanged();
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
                    IsHoveringAndAllowSelected = true;
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

        /// <summary>
        /// 当悬停状态被改变时触发
        /// </summary>
        protected virtual void OnHoveringHighlightChanged()
        {
            //Do nothing
        }
    }
}
