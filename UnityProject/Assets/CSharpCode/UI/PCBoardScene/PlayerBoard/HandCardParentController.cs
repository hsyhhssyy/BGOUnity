using System;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class HandCardParentController : TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;
        public GameObject CivilHandCardFrame;
        public String HandType;

        private bool _refreshRequired = true;

        public HandCardParentController()
        {
            UIKey = "PCBoard."+ HandType + ".Parent." + Guid;
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //Parent 只响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
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
            var unknownCardPrefab = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in CivilHandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.CivilCards.Count > 5)
            {
                incr = 0.7f * 4 / (board.CivilCards.Count - 1);
            }

            for (int i = 0; i < board.CivilCards.Count; i++)
            {
                var mSp = Instantiate(unknownCardPrefab);
                var childController = mSp.AddComponent<HandCardChildController>();
                childController.ParentController=this;
                childController.Card = board.CivilCards[i];

                mSp.GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(board.CivilCards[i], CivilHandCardFrame.transform, new Vector3(i * incr, 0, -0.1f * i));

            }
        }
    }
}
