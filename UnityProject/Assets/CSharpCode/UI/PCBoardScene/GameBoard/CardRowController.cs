using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class CardRowController: SimpleClickUIController
    {
        public PCBoardCardDisplayBehaviour SmallCardFrame;
        public GameObject HighLightFrame;
        //表示卡牌列上本Controller的位置，编辑器设定值
        public int Position;
        
        
        protected override string GetUIKey()
        {
            return "PCBoard.CardRow." + Guid;
        }
        
        protected override void OnHoveringHighlightChanged()
        {
            HighLightFrame.SetActive(IsHoveringAndAllowSelected);
        }

        protected override void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Position"] = Position;
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Position"] = Position;
        }
        

        protected override void Refresh()
        {
            if (Manager.CurrentGame.CardRow == null)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }
            if (Manager.CurrentGame.CardRow.Count <= Position)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }

            CardRowCardInfo cardRowInfo = Manager.CurrentGame.CardRow[Position];

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
                if (cardRowInfo.CanTake)
                {
                    SmallCardFrame.gameObject.SetActive(true);
                    SmallCardFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(cardRowInfo.Card);
                    SmallCardFrame.transform.localPosition = new Vector3(SmallCardFrame.transform.localPosition.x,
                        SmallCardFrame.transform.localPosition.y, -0.01f);
                }
                else
                {
                    //能放回去（空格）
                    SmallCardFrame.gameObject.SetActive(false);
                }


                if (cardRowInfo.CivilActionCost > 0)
                {
                    float init = -0.15f* cardRowInfo.CivilActionCost/2+0.15f/2;
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
