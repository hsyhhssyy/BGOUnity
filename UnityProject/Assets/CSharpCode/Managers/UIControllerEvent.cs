using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Managers
{
    public class GameUIEventArgs : EventArgs
    {
        public GameUIEventType EventType;
    }

    public class ControllerGameUIEventArgs : GameUIEventArgs
    {
        
    }

    public enum GameUIEventType
    {
        /// <summary>
        /// 玩家希望选中某UI元素
        /// </summary>
       TrySelect,
    }
}
