using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class WarningController : TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;
        public GameObject WarningFrame;
        public GameObject WarningPopupFrame;

        private bool _refreshRequired = false;

        public bool Highlight { get; set; }

        public WarningController()
        {
            UIKey = "PCBoard.Warning." + Guid;
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
                return;
            }
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
        }

        private void Refresh(TtaBoard board)
        {
            var warningPrefab = Resources.Load<GameObject>("Dynamic-PC/Warning");

            foreach (Transform child in WarningFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.Warnings.Count; i++)
            {
                var warning = board.Warnings[i];

                GameObject cardGo = Instantiate(warningPrefab);

                cardGo.transform.SetParent(WarningFrame.transform);
                cardGo.transform.localPosition = new Vector3(0.26f * i, 0);

                cardGo.GetComponent<WarningDisplayBehaviour>().DisplayWarning(warning, WarningPopupFrame);
            }
        }
    }
}
