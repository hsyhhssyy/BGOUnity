using System.Collections.Generic;
using UnityEngine;

namespace Assets.CSharpCode.UI.Util.Input
{
    public abstract class InputActionTriggerMonoBehaviour: MonoBehaviour
    {
        public static readonly List<InputActionTriggerMonoBehaviour> RegisteredTriggers = new List<InputActionTriggerMonoBehaviour>();

        public static void Register(InputActionTriggerMonoBehaviour candidate)
        {
            RegisteredTriggers.Add(candidate);
        }
        public static void Unregister(InputActionTriggerMonoBehaviour candidate)
        {
            RegisteredTriggers.Remove(candidate);
        }

        protected InputActionTriggerMonoBehaviour()
        {
            Register(this);
        }

        public void OnDestory()
        {
           LogRecorder.Log("Destoryed");
           Unregister(this);
        }

        public bool OnEnterJustFired { get; set; }
        public bool OnExitJustFired { get; set; }

        public virtual bool OnTriggerClick()
        {
            return false;
        }

        public virtual bool OnTriggerUp()
        {
            return false;
        }
        public virtual bool OnTriggerDown()
        {
            return false;
        }

        public virtual bool OnTriggerEnter()
        {
            return false;
        }
        public virtual bool OnTriggerExit()
        {
            return false;
        }
        public virtual bool OnTriggerClickOutside()
        {
            return false;
        }

        public virtual bool OnMouseDrag()
        {
            return false;
        }
    }
}
