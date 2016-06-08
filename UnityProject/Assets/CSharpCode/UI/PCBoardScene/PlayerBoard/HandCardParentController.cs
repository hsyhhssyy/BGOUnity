using System;
using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class HandCardParentController : TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;
        public GameObject HandCardFrame;
        public String HandType;

        private bool _refreshRequired = false;

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
            Manager.Regiseter(this);
        }

        [UsedImplicitly]
        public void FixedUpdate()
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

            foreach (Transform child in HandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;

            List<CardInfo> cards  = HandType == "HandCivilCard" ? board.CivilCards : board.MilitaryCards;

            if (cards.Count > 5)
            {
                incr = 0.7f * 4 / (cards.Count - 1);
            }

            for (int i = 0; i < cards.Count; i++)
            {
                var mSp = Instantiate(unknownCardPrefab);
                var childController = mSp.AddComponent<HandCardChildController>();
                childController.ParentController=this;
                childController.Card = cards[i];

                mSp.GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(cards[i], HandCardFrame.transform, new Vector3(i * incr, 0, -0.1f * i));

            }
        }
    }
}
