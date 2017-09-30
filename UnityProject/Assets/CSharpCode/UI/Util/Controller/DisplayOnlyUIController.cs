using System;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    /// <summary>
    /// 一个仅用于显示的UiController，他能够响应Refresh事件。
    /// 可以通过设置IsHoveringAndAllowSelected来改变当前物体的选择边框。并且可以通过覆盖OnHoveringHighlightChanged来响应事件
    /// </summary>
    public abstract class DisplayOnlyUIController : TtaUIControllerMonoBehaviour
    {
        protected bool RefreshRequired;
        private GameManagerState savedManagerState;

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
                    RefreshRequired = true;
                    savedManagerState = Manager.State;

                    OnHoveringHighlightChanged();
                }
            }
        }

        //这里的Manager不能使用属性，因为Unity编辑器需要指定他
        //而且，修改它会导致编辑器丢失所有Manager的信息
        [UsedImplicitly] public GameBoardManager Manager;

        [UsedImplicitly]
        public virtual void Start()
        {
            if (Manager == null)
            {
                Manager=GameBoardManager.ActiveManager;
            }
            Manager.Regiseter(this);
            OnHoveringHighlightChanged();
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                RefreshRequired = true;
                savedManagerState = Manager.State;

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
            //如果游戏状态发生了变化，所有元素会强制进行刷新
            if (savedManagerState != Manager.State)
            {
                RefreshRequired = true;
                savedManagerState = Manager.State;
            }

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
