using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.UI.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [UsedImplicitly]
    public class PCBoardActionTriggerController: MonoBehaviour
    {
        public bool buttonDown = false;
        public bool buttonPressed = false;
        public bool buttonUp=false;

        void Update()
        {
            for (int i = InputActionTriggerMonoBehaviour.RegisteredTriggers.Count - 1; i >= 0; i--)
            {
                if (InputActionTriggerMonoBehaviour.RegisteredTriggers[i]==null)
                {
                    InputActionTriggerMonoBehaviour.RegisteredTriggers.RemoveAt(i);
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (buttonDown == true|| buttonPressed == true)
                {
                    buttonPressed = true;
                    buttonDown = false;
                    buttonUp = false;
                }
                else
                {
                    buttonDown = true;
                    buttonUp = false;
                    buttonPressed = false;
                }
            }
            else
            {
                if (buttonDown == true || buttonPressed == true)
                {
                    buttonPressed = false;
                    buttonDown = false;
                    buttonUp = true;
                }
                else
                {
                    buttonPressed = false;
                    buttonDown = false;
                    buttonUp = false;
                }
            }
            TestMouseWithColliderOverlap();
    }

        private void TestMouseWithColliderOverlap()
        {
            Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            var colliderList = new List<Collider2D>();
            if (col.Length > 0)
            {
                colliderList = col.ToList();
                colliderList.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));
            }
            bool onMouseClickTriggered = false;
            bool onMouseEnterTriggered = false;
            bool onMouseExitTriggered = false;

            if (buttonDown)
            {
                List<InputActionTriggerMonoBehaviour> ClickedTrigger = new List<InputActionTriggerMonoBehaviour>();
                foreach (Collider2D c in colliderList)
                {
                    GameObject gameObj = c.gameObject;
                    if (gameObj.GetComponent<InputActionTriggerMonoBehaviour>() != null)
                    {
                        if (!onMouseClickTriggered)
                        {
                            bool res = gameObj.GetComponent<InputActionTriggerMonoBehaviour>().OnTriggerClick();
                            ClickedTrigger.Add(gameObj.GetComponent<InputActionTriggerMonoBehaviour>());
                            if (res == true)
                            {
                                onMouseClickTriggered = true;
                            }
                        }
                    }
                }
                foreach (var trigger in InputActionTriggerMonoBehaviour.RegisteredTriggers)
                {
                    if (!ClickedTrigger.Contains(trigger))
                    {
                        trigger.OnTriggerClickOutside();
                    }
                }
            }

            foreach (Collider2D c in colliderList)
            {
                GameObject gameObj = c.gameObject;
                if (gameObj.GetComponent<InputActionTriggerMonoBehaviour>() != null)
                {
                    if (!onMouseEnterTriggered)
                    {
                        var trigger = gameObj.GetComponent<InputActionTriggerMonoBehaviour>();
                        if (trigger.OnEnterJustFired != true)
                        {
                            bool res = trigger.OnTriggerEnter();
                            if (res == true)
                            {
                                onMouseEnterTriggered = true;
                            }
                            trigger.OnEnterJustFired = true;
                            trigger.OnExitJustFired = false;
                        }
                    }
                }
            }
            foreach (var trigger in InputActionTriggerMonoBehaviour.RegisteredTriggers)
            {
                if (trigger == null)
                {
                    continue;
                }
                if (trigger.gameObject.GetComponent<BoxCollider2D>() != null)
                {
                    if (!colliderList.Contains(trigger.gameObject.GetComponent<BoxCollider2D>()))
                    {
                        if (trigger.OnExitJustFired != true)
                        {
                            bool res = trigger.OnTriggerExit();
                            if (res == true)
                            {
                                onMouseExitTriggered = true;
                            }
                            trigger.OnEnterJustFired = false;
                            trigger.OnExitJustFired = true;
                        }
                    }
                }
            }


        }

        void TestMouseWithRayCast()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero);
            if (hitInfo.collider != null)
            {
                GameObject gameObj = hitInfo.collider.gameObject;
                Debug.Log("click object name is " + gameObj.name);
                if (gameObj.GetComponent<InputActionTriggerMonoBehaviour>() != null)
                {
                    gameObj.GetComponent<InputActionTriggerMonoBehaviour>().OnTriggerClick();
                    Debug.Log("pick up!");
                }
            }
        }
    }
}
