using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public abstract class TtaUIControllerMonoBehaviour: InputActionTriggerMonoBehaviour
    {
        public String UIKey { get; protected set; }

        protected String Guid { get; private set; }

        public event EventHandler<GameUIEventArgs> GameBoardControllerEvent;

        protected abstract void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args);
        
        protected TtaUIControllerMonoBehaviour()
        {
            Guid= System.Guid.NewGuid().ToString();
        }

        protected void Broadcast(GameUIEventArgs args)
        {
            if (GameBoardControllerEvent != null)
            {
                GameBoardControllerEvent(this, args);
            }
        }
    }
}
