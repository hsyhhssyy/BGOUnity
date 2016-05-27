using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class BlueMarkerController : TtaUIControllerMonoBehaviour
    {
        public GameObject[] BlueBankMarkers;

        private bool _refreshRequired;

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
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
        public void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        public void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];

            int blueMarkerOwn = board.Resource[ResourceType.BlueMarker];
            for (int blueMarkerDisplay = 15;
                blueMarkerOwn >= 0 || blueMarkerDisplay >= 0;
                blueMarkerDisplay--, blueMarkerOwn--)
            {
                if (blueMarkerDisplay >= 0)
                {
                    var bankGo = BlueBankMarkers[15 - blueMarkerDisplay];
                    bankGo.SetActive(blueMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }
        }
    }
}
