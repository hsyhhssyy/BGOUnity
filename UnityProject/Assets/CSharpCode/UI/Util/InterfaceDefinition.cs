using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.Util
{
    public interface TtaActionBinder
    {
        /// <summary>
        /// 除非特殊需要，否则在执行Bind之前都要考虑执行一个Unbind
        /// </summary>
        void BindAction(List<PlayerAction> actions,PCBoardBehavior boardBehavior);

        /// <summary>
        /// <para>BoardBehavior在下列情况会考虑执行最高层的Unbind</para>
        ///  <para>1、玩家切换到他人面板</para>
        /// 2、Scene要被销毁<para/>
        /// 3、玩家面板被刷新<para/>
        /// 在下列情况下不会执行最高层的Unbind<para/>
        /// 1、写入了InterAction（然后会立刻执行一个Bind但是不会先Unbind，并且不会refresh board）<para/>
        /// 具体会不会执行Unbind取决于你的父级ActionBinder有没有在他的Unbind里调用你的Unbind<para/>
        /// </summary>
        [UsedImplicitly]
        void Unbind();
    }

    public interface TtaDisplayBehaviour
    {
        void Refresh();
    }
    
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
           Unregister(this);
        }

        public virtual bool OnMouseClick()
        {
            return false;
        }

        public virtual bool OnMouseUp()
        {
            return false;
        }
        public virtual bool OnMouseDown()
        {
            return false;
        }

        public virtual bool OnMouseEnter()
        {
            return false;
        }
        public virtual bool OnMouseExit()
        {
            return false;
        }
        public virtual bool OnMouseClickOutside()
        {
            return false;
        }

        public virtual bool OnMouseDrag()
        {
            return false;
        }
    }
}
