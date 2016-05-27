using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class CardRowController: TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;
        public PCBoardCardDisplayBehaviour SmallCardFrame;
        public GameObject HighLightFrame;

        public int Position;

        private bool _refreshRequired;

        public bool Highlight { get; set; }

        public CardRowController()
        {
            UIKey = "PCBoard.CardRow." + Guid;
        }

        [UsedImplicitly]
        public void Start()
        {
            Manager.GameBoardManagerEvent += OnSubscribedGameEvents;
            Manager.Regiseter(this);
        }

        [UsedImplicitly]
        public void FixedUpdate()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh(Manager.CurrentGame.CardRow[Position]);
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
            Broadcast(new ControllerGameUIEventArgs(GameUIEventType.TrySelect,UIKey));
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

            //附加Position
            args.AttachedData["Position"] = Position;

            Broadcast(args);
            
            return true;
        }

        private void Refresh(CardRowCardInfo cardRowInfo)
        {
            var whitePrefab = Resources.Load<GameObject>("Dynamic-PC/WhiteMarker");
            
            var civilCostFrame = gameObject.FindObject("CivilActionCost");

            if (Position > 2)
            {
                gameObject.FindObject("DiscardMark").SetActive(false);
            }

            foreach (Transform trans in civilCostFrame.transform)
            {
                Destroy(trans.gameObject);
            }

            if (cardRowInfo.Card != null)
            {
                if (cardRowInfo.CanPutBack != true)
                {
                    SmallCardFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(cardRowInfo.Card);
                    SmallCardFrame.transform.position=new Vector3(SmallCardFrame.transform.position.x, SmallCardFrame.transform.position.y,-0.01f);
                }


                if (cardRowInfo.CivilActionCost > 0)
                {
                    float init = -0.15f*cardRowInfo.CivilActionCost + 0.15f;
                    for (int j = 0; j < cardRowInfo.CivilActionCost; j++)
                    {
                        var mSp = Instantiate(whitePrefab);
                        mSp.transform.SetParent(civilCostFrame.transform);
                        mSp.transform.localPosition = new Vector3(init + j*0.15f, 0f);
                        mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    }
                }
            }
        }
    }
}
