using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard
{
    public class YellowMarkerController : TtaUIControllerMonoBehaviour
    {
        private bool _refreshRequired;
        private bool _highlight;

        public GameObject[] YellowBankMarkers;

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            UIKey = "PCBoard.YellowMarker." + Guid;
            Manager.Regiseter(this);
        }
        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
            }else if (args.EventType == GameUIEventType.AllowSelect)
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

        private void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            int yellowMarkerOwn = board.Resource[ResourceType.YellowMarker];
            for (int yellowMarkerDisplay = 17;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = YellowBankMarkers[17 - yellowMarkerDisplay];
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }
        }

        public override bool OnTriggerEnter()
        {
            Channel.Broadcast(new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey));
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
            
            Channel.Broadcast(args);

            return true;
        }
    }
}
