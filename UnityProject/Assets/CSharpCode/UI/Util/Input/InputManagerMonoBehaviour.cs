using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.Util.Input
{
    [UsedImplicitly]
    public class InputManagerMonoBehaviour : MonoBehaviour
    {
        private bool _buttonDown;
        private bool _buttonPressed;
        private bool _buttonUp;

        [UsedImplicitly]
        public void FixedUpdate()
        {
            for (int i = InputActionTriggerMonoBehaviour.RegisteredTriggers.Count - 1; i >= 0; i--)
            {
                if (InputActionTriggerMonoBehaviour.RegisteredTriggers[i] == null)
                {
                    InputActionTriggerMonoBehaviour.RegisteredTriggers.RemoveAt(i);
                }
            }

            if (UnityEngine.Input.GetMouseButton(0))
            {
                if (_buttonDown || _buttonPressed)
                {
                    _buttonPressed = true;
                    _buttonDown = false;
                    _buttonUp = false;
                }
                else
                {
                    _buttonDown = true;
                    _buttonUp = false;
                    _buttonPressed = false;
                }
            }
            else
            {
                if (_buttonDown || _buttonPressed)
                {
                    _buttonPressed = false;
                    _buttonDown = false;
                    _buttonUp = true;
                }
                else
                {
                    _buttonPressed = false;
                    _buttonDown = false;
                    _buttonUp = false;
                }
            }
            TestMouseWithColliderOverlap();
        }

        private void TestMouseWithColliderOverlap()
        {
            Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition));

            var colliderList = new List<Collider2D>();
            if (col.Length > 0)
            {
                colliderList = col.ToList();
                colliderList.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));
            }
            bool onMouseClickTriggered = false;
            bool onMouseDownTriggered = false;
            bool onMouseUpTriggered = false;
            bool onMouseEnterTriggered = false;
            //bool onMouseExitTriggered = false;

            if (_buttonDown || _buttonUp)
            {
                List<InputActionTriggerMonoBehaviour> clickedTrigger = new List<InputActionTriggerMonoBehaviour>();
                foreach (Collider2D c in colliderList)
                {
                    GameObject gameObj = c.gameObject;
                    if (!onMouseClickTriggered && _buttonUp)
                    {
                        var behaves = gameObj.GetComponents<InputActionTriggerMonoBehaviour>();

                        bool res = false;
                        foreach (var behaviour in behaves)
                        {
                            res = res || behaviour.OnTriggerClick();
                        }

                        if (res)
                        {
                            clickedTrigger.AddRange(gameObj.GetComponents<InputActionTriggerMonoBehaviour>());
                            onMouseClickTriggered = true;
                        }
                    }
                    if (!onMouseDownTriggered && _buttonDown)
                    {
                        bool res = gameObj.GetComponents<InputActionTriggerMonoBehaviour>()
                            .Aggregate(false, (current, behaviour) => current || behaviour.OnTriggerDown());

                        if (res)
                        {
                            onMouseDownTriggered = true;
                        }
                    }
                    if (!onMouseUpTriggered && _buttonUp)
                    {
                        bool res = gameObj.GetComponents<InputActionTriggerMonoBehaviour>()
                            .Aggregate(false, (current, behaviour) => current || behaviour.OnTriggerUp());

                        if (res)
                        {
                            onMouseUpTriggered = true;
                        }
                    }
                }
                if (_buttonUp)
                {
                    foreach (var trigger in InputActionTriggerMonoBehaviour.RegisteredTriggers)
                    {
                        if (!clickedTrigger.Contains(trigger))
                        {
                            trigger.OnTriggerClickOutside();
                        }
                    }
                }


            }

            foreach (Collider2D c in colliderList)
            {
                GameObject gameObj = c.gameObject;
                if (!onMouseEnterTriggered)
                {
                    bool res = false;
                    foreach (var trigger in gameObj.GetComponents<InputActionTriggerMonoBehaviour>())
                    {
                        if (trigger.OnEnterJustFired != true)
                        {
                            res = res || trigger.OnTriggerEnter();
                            trigger.OnEnterJustFired = true;
                            trigger.OnExitJustFired = false;
                        }
                    }
                    if (res)
                    {
                        onMouseEnterTriggered = true;
                    }
                }
            }

            var toggleList =
                new List<InputActionTriggerMonoBehaviour>(InputActionTriggerMonoBehaviour.RegisteredTriggers);
            foreach (var trigger in toggleList)
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
                            /* bool res = */
                            trigger.OnTriggerExit();
                            //理论上压在下面的Trigger将不会再收到Exit的消息
                            //但是这里UI会有点小问题
                            /*
                            if (res == true)
                            {
                                onMouseExitTriggered = true;
                                
                            }*/
                            trigger.OnEnterJustFired = false;
                            trigger.OnExitJustFired = true;
                        }
                    }
                }
            }


        }

    }
}
