using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using Assets.CSharpCode.UI.PCBoardScene.PlayerBoard.Buildings;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    public class WonderMenuController: DisplayOnlyUIController
    {

        public GameObject MenuFrame;

        protected override void Refresh()
        {
            Collapse();
            //Do nothing
        }

        public override bool OnTriggerClick()
        {
            //阻断所有后面的Click
            return true;
        }

        public override bool OnTriggerClickOutside()
        {
            Collapse();
            return true;
        }

        public void Popup(List<PlayerAction> actions)
        {
            if (actions.Count == 0)
            {
                return;
            }
           if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Collapse")||
                gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("CollapseComplete"))
           {
                var prefab = Resources.Load<GameObject>("Dynamic-PC/Menu/WonderMenuItem");

                foreach (Transform child in MenuFrame.transform)
                {
                    Destroy(child.gameObject);
                }

               for (int index = 0; index < actions.Count; index++)
               {
                   var action = actions[index];
                   GameObject mSp = Instantiate(prefab);
                   mSp.FindObject("ResCost").GetComponent<TextMesh>().text = action.Data[2].ToString();
                   mSp.FindObject("StageText").GetComponent<TextMesh>().text = action.Data[1].ToString();

                   mSp.transform.parent = MenuFrame.transform;
                   mSp.transform.localPosition = new Vector3(0, -0.14f* index, 0f);
                   mSp.transform.localScale = new Vector3(1f, 1f, 1f);

                   var controller=mSp.GetComponent<WonderMenuItemController>();
                   controller.Manager = Manager;
                    controller.SetAction(action);
               }

               gameObject.GetComponent<Animator>().SetBool("Collapsed",false);
           }
        }

        public void Collapse()
        {
            if(gameObject.GetComponent<Animator>())
            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup"))
            {
                    //gameObject.GetComponent<Animator>().Stop();
                    //this.gameObject.GetComponent<Animator>().SetTrigger("Collapse");
                    gameObject.GetComponent<Animator>().SetBool("Collapsed", true);
                }
        }

    }
}
