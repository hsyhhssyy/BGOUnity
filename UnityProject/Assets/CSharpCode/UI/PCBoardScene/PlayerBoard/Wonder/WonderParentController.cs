using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class WonderParentController : TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;

        public GameObject ConstructingWonderFrame;
        public GameObject CompletedWondersFrame;

        private bool _refreshRequired =false;

        public WonderParentController()
        {
            UIKey = "PCBoard.Wonder.Parent." + Guid;
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
                Refresh(Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo]);
                _refreshRequired = false;
            }

        }

        private void Refresh(TtaBoard board)
        {
            //建造中
            if (board.ConstructingWonder != null)
            {
                ConstructingWonderFrame.SetActive(true);
                ConstructingWonderFrame.FindObject("WonderName").GetComponent<TextMesh>().text =
                    board.ConstructingWonder.CardName;
                var stepFrame = ConstructingWonderFrame.FindObject("Steps");

                foreach (Transform child in stepFrame.transform)
                {
                    Destroy(child.gameObject);
                }

                var stepPrefab = Resources.Load<GameObject>("Dynamic-PC/WonderBuildingStage");

                float init = -0.16f* board.ConstructingWonder.BuildCost.Count + 0.16f;
                for (int index = 0; index < board.ConstructingWonder.BuildCost.Count; index++)
                {
                    var mSp = Instantiate(stepPrefab);
                    if (index < board.ConstructingWonderSteps)
                    {
                        mSp.FindObject("BlueMarker").SetActive(false);
                        mSp.FindObject("StepText").GetComponent<TextMesh>().text =
                            board.ConstructingWonder.BuildCost[index].ToString();
                    }
                    else
                    {
                        mSp.FindObject("StepText").SetActive(false);
                    }
                    mSp.transform.parent = stepFrame.transform;
                    mSp.transform.localPosition = new Vector3(init + 0.16f*index, 0, 0);
                }

                ConstructingWonderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.ConstructingWonder);
            }
            else
            {
                ConstructingWonderFrame.SetActive(false);
            }

            //建造完成
            var wonderPrefab = Resources.Load<GameObject>("Dynamic-PC/CompletedWonder");

            foreach (Transform child in CompletedWondersFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.CompletedWonders.Count; i++)
            {
                var wonderCard = board.CompletedWonders[i];

                GameObject cardGo = Instantiate(wonderPrefab);
                cardGo.FindObject("WonderName").GetComponent<TextMesh>().text = wonderCard.CardName;

                cardGo.transform.SetParent(CompletedWondersFrame.transform);
                cardGo.transform.localPosition = new Vector3(0, -0.27f*i);

                cardGo.FindObject("Mask").GetComponent<PCBoardCardDisplayBehaviour>().Bind(wonderCard);

                var sp = UnityResources.GetSprite(wonderCard.SpecialImage);
                if (sp != null)
                {
                    cardGo.FindObject("Image").GetComponent<SpriteRenderer>().sprite = sp;
                }

                cardGo.GetComponent<WonderChildController>().Card = wonderCard;
            }
        }
    }
}
