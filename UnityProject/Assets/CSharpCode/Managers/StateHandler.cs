using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Managers
{
    public abstract class GameBoardStateHandler
    {
        protected readonly GameBoardManager Manager;

        protected GameEventChannel Channel
        {
            get { return Manager.Channel; }
        }

        protected TtaGame CurrentGame
        {
            get { return Manager.CurrentGame; }
        }

        protected Dictionary<String, Object> StateData
        {
            get { return Manager.StateData; }
        }

        public GameBoardStateHandler(GameBoardManager manager)
        {
            Manager = manager;
        }

        public abstract void EnteringState();

        public abstract void LeaveState();

        public abstract void ProcessGameEvents(System.Object sender, GameUIEventArgs args);

        public abstract void OnUnityUpdate();
    }
}
