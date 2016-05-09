using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    public class CardRowDisplayBehaviour:MonoBehaviour
    {
        public List<CardRowCardInfo> CardRow;

        public GameObject[] CardRowGameObjectItems;

        public void Refresh()
        {
            var whitePrefab = Resources.Load<GameObject>("WhiteMarker");

            int i = 0;
            for (; i < CardRow.Count; i++)
            {
                var cardRowInfo = CardRow[i];

                //DisplayCard
                //cardRowInfo.Card;
                var itemFrame=CardRowGameObjectItems[i];
                var civilCostFrame = itemFrame.FindObject("CivilActionCost");

                if (i > 2)
                {
                    itemFrame.FindObject("DiscardMark").SetActive(false);
                }

                foreach (Transform trans in civilCostFrame.transform)
                {
                    Destroy(trans.gameObject);
                }

                if (cardRowInfo.Card != null)
                {
                    var sprite = UnityResources.GetSprite(cardRowInfo.Card.NormalImage);
                    if (sprite != null)
                    {
                        if (cardRowInfo.CanPutBack != true)
                        {
                            itemFrame.FindObject("PCBoardCard-Small").GetComponent<PCBoardCardSmallDisplayBehaviour>().Bind(cardRowInfo.Card);
                        }
                    }


                    if (cardRowInfo.CivilActionCost > 0)
                    {
                        float init = -0.15f * cardRowInfo.CivilActionCost +0.15f;
                        for (int j = 0; j < cardRowInfo.CivilActionCost; j++)
                        {
                            var mSp = Instantiate(whitePrefab);
                            mSp.transform.SetParent(civilCostFrame.transform);
                            mSp.transform.localPosition = new Vector3(init + j * 0.15f, 0f);
                        }
                    }
                }

            }
        }
    }
}
