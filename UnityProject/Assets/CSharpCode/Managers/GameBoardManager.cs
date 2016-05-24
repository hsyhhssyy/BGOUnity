using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI;

namespace Assets.CSharpCode.Managers
{
    public class GameBoardManager
    {
        public event EventHandler<GameUIEventArgs> GameBoardManagerEvent;

        private void OnSubscribedGameEvents(Object sender, GameUIEventArgs args)
        {
            switch (State)
            {
                case GameManagerState.Idle:
                    ProcessIdleStateEvents(sender, args);
                    break;
            }
        }

        #region 一系列复杂的用于维护当前游戏状态机的变量

        public TtaGame CurrentGame { get { return SceneTransporter.CurrentGame; } }

        public GameManagerState State;
        public Dictionary<String, Object> StateData;

        #endregion

        #region Idle State

        private void ProcessIdleStateEvents(Object sender, ControllerGameUIEventArgs args)
        {
            //在空闲状态，允许点击的UI元素如下
            if (args.EventType == GameUIEventType.TrySelect)
            {
                
            }
        }

        #endregion

    }


    public enum GameManagerState
    {
        Idle,
    }
}
