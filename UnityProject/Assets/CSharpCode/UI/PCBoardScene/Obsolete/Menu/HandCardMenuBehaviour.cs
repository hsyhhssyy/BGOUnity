using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    [Obsolete]
    public class HandCardMenuBehaviour:MonoBehaviour
    {
        public SpecificCodeActionTrigger trigger;

        public GameObject OneButtonFrame;
        public GameObject TwoButtonFrame;
        public GameObject ThreeButtonFrame;


        public Action<List<PlayerAction>> InternalActionCallback;
        public List<String> Choice;
        public PlayerAction Action;

        public int ButtonCount = 2;


        public void Start()
        {
            trigger.ActionOnMouseClick = Clicked;
            trigger.ActionOnMouseClickOutside = () =>
            {
                if (
                    !this.GetComponent<BoxCollider2D>()
                        .OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    Collapse();
                }
            };

        }

        public void Popup(PlayerAction action, GameObject boardCardFrame,PCBoardBehavior boardBehavior)
        {
            gameObject.SetActive(true);
            this.transform.position = new Vector3(boardCardFrame.transform.position.x, boardCardFrame.transform.position.y,-5f);
            Action = action;
            trigger.BoardBehavior = boardBehavior;
        }

        public void Collapse()
        {
            gameObject.SetActive(false);
        }

        private void Clicked()
        {
            var mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (ButtonCount == 1)
            {
                if (Action != null)
                {
                    trigger.BoardBehavior.TakeAction(Action, InternalActionCallback);
                }
                Collapse();
            }
            else if (ButtonCount == 2)
            {
                if (mousePoint.y < transform.position.y+0.5f)
                {
                    LogRecorder.Log("Not in Range");
                    return;
                }
                if (mousePoint.x < this.transform.position.x)
                {
                    if (Action != null)
                    {
                        trigger.BoardBehavior.TakeAction(Action, InternalActionCallback);
                    }
                    Collapse();
                }
                else
                {
                    Collapse();
                }
            }
            else if (ButtonCount == 3)
            {
                if (mousePoint.y < transform.position.y + 0.5f)
                {
                    LogRecorder.Log("Not in Range");
                    return;
                }
                if (mousePoint.x < this.transform.position.x-0.1f)
                {
                    if (Action != null)
                    {
                        trigger.BoardBehavior.TakeAction(Action, InternalActionCallback);
                    }
                    Collapse();
                }else 
                if (mousePoint.x > this.transform.position.x - 0.1f&& mousePoint.x < this.transform.position.x + 0.1f)
                {
                    if (Action != null)
                    {
                        trigger.BoardBehavior.TakeAction(Action, InternalActionCallback);
                    }
                    Collapse();
                }
                else
                {
                    Collapse();
                }
            }
        }
    }
}
