using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class WorkerPoolController: SimpleClickUIController
    {
        public GameObject WorkerPoolFrame;
        
        protected override string GetUIKey()
        {
            return "PCBoard.PlayerBoard.WorkerPool";
        }
        
        public override bool OnTriggerEnter()
        {
            Channel.Broadcast(new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey));
            return true;
        }
        
        protected override void Refresh()
        {
            Refresh(Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo]);
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

            int total = board.Resource[ResourceType.WorkerPool];
            for (int i = 0; i < total; i++)
            {
                var mSp = Instantiate(i < board.DisorderValue ? unhappyPrefab : happyPrefab);

                mSp.transform.SetParent(WorkerPoolFrame.transform);
                mSp.transform.localPosition = new Vector3(0.33f+(total/2-i)*0.15f, 0);
            }
        }
    }
}
