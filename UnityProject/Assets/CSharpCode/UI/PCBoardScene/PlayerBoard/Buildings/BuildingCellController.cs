using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard
{
    /// <summary>
    /// 这是每一个建筑物单独的Controller，如需获取其实例，请尝试查找BuildingChildController
    /// </summary>
    public class BuildingCellController : TtaUIControllerMonoBehaviour
    {
        private BuildingChildController _parentController;
        public BuildingChildController ParentController
        {
            get { return _parentController; }
            set
            {
                if (value != null)
                {
                    value.Manager.Regiseter(this);
                }
                _parentController = value;
            }
        }

        public CardInfo Card;

        public bool Highlight { get; set; }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains(Guid))
            {
                return;
            }

            //含有自己的GUID，是发给自己的
            if (args.EventType == GameUIEventType.AllowSelect)
            {
                Highlight = true;
            }
            else
            if (args.EventType == GameUIEventType.PopupMenu)
            {
                //要求弹出选单
                //MenuController调用Parent的BroadCast
                ParentController.MenuFrame.Popup(args.AttachedData["Actions"] as List<PlayerAction>, this);
                //ParentController.PopupMenu(this,args.AttachedData["Items"]);
            }
        }

        [UsedImplicitly]
        public void Start()
        {
            UIKey = "PCBoard.BuildingCell.Child." + Guid;
            

        }

        public override bool OnTriggerEnter()
        {
            Channel.Broadcast(new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey));
            return true;
        }

        public override bool OnTriggerExit()
        {
            Highlight = false;
            return true;
        }

        public override bool OnTriggerClickOutside()
        {
            if (_parentController != null)
            {
                //虽然点在了外面，还要判断是不是点在Menu上了
                var collider= _parentController.MenuFrame.GetComponent<Collider2D>();
                if (collider != null)
                {
                    if (collider.IsMouseHitting())
                    {
                        //表示自己不处理这个事件
                        return false;
                    }
                }

                _parentController.MenuFrame.Collapse();
            }
            return true;
        }

        public override bool OnTriggerClick()
        {
            if (!Highlight) return false;

            var args = new ControllerGameUIEventArgs(GameUIEventType.Selected, UIKey);

            //附加Card
            args.AttachedData["Card"] = Card;

            Channel.Broadcast(args);

            return true;
        }
    }
}
