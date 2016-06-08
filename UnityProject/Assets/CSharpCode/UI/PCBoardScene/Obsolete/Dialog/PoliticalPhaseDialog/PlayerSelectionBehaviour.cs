using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.Util.Input;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhaseDialog
{
    public class PlayerSelectionBehaviour:InputActionTriggerMonoBehaviour
    {
        public GameObject[] PlayerBox;

        public void Start()
        {
            for (int i = SceneTransporter.CurrentGame.Boards.Count; i<4;i++)
            {
                PlayerBox[i].SetActive(false);
            }
        }

        public override bool OnTriggerClick()
        {
            var mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var box in PlayerBox)
            {
                if (box.GetComponent<BoxCollider2D>().OverlapPoint(new Vector2(mousePoint.x, mousePoint.y)))
                {
                    //改变Box的外观
                    box.GetComponent<SpriteRenderer>().sprite = null;
                    foreach (var otherBox in PlayerBox)
                    {
                        if (otherBox == box)
                        {
                            box.GetComponent<SpriteRenderer>().sprite = null;
                            continue;
                            //改变其他box的外观
                        }
                    }
                    return true;
                }
            }

            return true;
        }
    }
}
