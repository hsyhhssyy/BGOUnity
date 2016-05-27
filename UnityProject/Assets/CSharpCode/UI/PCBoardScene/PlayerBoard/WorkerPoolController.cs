using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class WorkerPoolController: TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;
        public GameObject WorkerPoolFrame;
        public GameObject HighLightFrame;
        
        private bool _refreshRequired = false;

        public bool Highlight { get; set; }

        public WorkerPoolController()
        {
            UIKey = "PCBoard.WorkerPool." + Guid;
        }

        [UsedImplicitly]
        public void Start()
        {
            Manager.GameBoardManagerEvent += OnSubscribedGameEvents;
            Manager.Regiseter(this);
        }

        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh(Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo]);
            }

            //HighLight
            if (HighLightFrame != null) HighLightFrame.SetActive(Highlight);

        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
                return;
            }

            if (!args.UIKey.Contains(Guid))
            {
                return;
            }

            //含有自己的GUID，是发给自己的
            if (args.EventType == GameUIEventType.AllowSelect)
            {
                Highlight = true;
            }
        }


        public override bool OnTriggerEnter()
        {
            Broadcast(new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey));
            return true;
        }

        public override bool OnTriggerExit()
        {
            Highlight = false;
            return true;
        }

        public override bool OnTriggerClick()
        {
            if (!Highlight) return false;

            var args = new ControllerGameUIEventArgs(GameUIEventType.Selected, UIKey);
            
            Broadcast(args);

            return true;
        }

        private void Refresh(TtaBoard board)
        {
            var happyPrefab = Resources.Load<GameObject>("Dynamic-PC/HappyWorker");
            var unhappyPrefab = Resources.Load<GameObject>("Dynamic-PC/UnhappyWorker");

            foreach (Transform child in WorkerPoolFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = board.Resource[ResourceType.WorkerPool] > 1
                ? (0.66f / (board.Resource[ResourceType.WorkerPool] - 1))
                : 0f;

            for (int i = 0; i < board.Resource[ResourceType.WorkerPool]; i++)
            {
                var mSp = Instantiate(i < board.DisorderValue ? unhappyPrefab : happyPrefab);

                mSp.transform.SetParent(WorkerPoolFrame.transform);
                mSp.transform.localPosition = new Vector3(incr * i, 0);
            }
        }
    }
}
