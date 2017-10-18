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
        public ServerGameUIEventArgs(GameUIEventType eventType, string uIKey)
        {
            EventType = eventType;
            UIKey = uIKey;
        }
    }

    public class LogicGameUIEventArgs : GameUIEventArgs
    {
        public LogicGameUIEventArgs(GameUIEventType eventType, string uIKey)
        {
            EventType = eventType;
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
        /// 通知UI元素收到InternalAction的实际Action结果
        /// </summary>
        ReportInternalAction,
        /// <summary>
        /// 通知网络服务器玩家进行某行为
        /// </summary>
        TakeAction,
        /// <summary>
        /// 表示服务器对玩家的Action做出了响应（注意一个action可能会触发多次此事件，因为TtaServer本地可能在服务器之前响应。）
        /// </summary>
        ActionResponse,
        /// <summary>
        /// 表示请UI提示玩家等待网络通讯
        /// </summary>
        WaitingNetwork,
        /// <summary>
        /// 表示请UI不再提示玩家等待网络通讯
        /// </summary>
        CancelWaitingNetwork,
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
        /// 强制要求进行网络刷新（界面暗转）
        /// </summary>
        ForceRefresh,
        /// <summary>
        /// 后台进行网络刷新（界面保持不变）
        /// </summary>
        BackgroundRefresh,

        //-------------------Logic Event ------------
        /// <summary>
        /// 表示GameChange事件被触发
        /// </summary>
        LogicGameChanged,
    }
}
