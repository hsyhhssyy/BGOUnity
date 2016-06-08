using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Managers
{
    public class GameUIEventArgs : EventArgs
    {
        public GameUIEventType EventType;

        public String UIKey;

        public Dictionary<String,Object> AttachedData=new Dictionary<string, object>();
    }

    public class ControllerGameUIEventArgs : GameUIEventArgs
    {
        public ControllerGameUIEventArgs(GameUIEventType allowSelect, string uIKey)
        {
            EventType = allowSelect;
            UIKey = uIKey;
        }
    }
    public class ManagerGameUIEventArgs : GameUIEventArgs
    {
        public ManagerGameUIEventArgs(GameUIEventType allowSelect, string uIKey)
        {
            EventType = allowSelect;
            UIKey = uIKey;
        }
    }
    public class ServerGameUIEventArgs : GameUIEventArgs
    {
        public ServerGameUIEventArgs(GameUIEventType allowSelect, string uIKey)
        {
            EventType = allowSelect;
            UIKey = uIKey;
        }
    }

    public enum GameUIEventType
    {
        /// <summary>
        /// 玩家希望选中某UI元素
        /// </summary>
       TrySelect,
       /// <summary>
       /// 玩家已经选择了某个UI元素
       /// </summary>
        Selected,


       /// <summary>
       /// 允许玩家选中特定UI元素
       /// </summary>
        AllowSelect,
        /// <summary>
        /// 要求UI元素刷新自己
        /// </summary>
        Refresh,
        /// <summary>
        /// 通知网络服务器玩家进行某行为
        /// </summary>
        TakeAction,
        /// <summary>
        /// 表示请UI提示玩家等待网络通讯
        /// </summary>
        WaitingNetwork,
        /// <summary>
        /// 要求弹出一个选单
        /// </summary>
        PopupMenu,
        /// <summary>
        /// 要求UI元素进入已选中状态
        /// </summary>
        SelectionActive,
        /// <summary>
        /// 要求UI元素取消自己的已选中状态
        /// </summary>
        SelectionDeactive,
        /// <summary>
        /// 强制要求进行网络刷新
        /// </summary>
        ForceRefresh,
        //
    }
}
