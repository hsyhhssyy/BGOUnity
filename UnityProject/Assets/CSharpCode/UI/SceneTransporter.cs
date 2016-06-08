using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.UI
{
    public static class SceneTransporter
    {
        public static Dictionary<String, Object> data;

        public static IServerAdapter Server;

        public static List<TtaGame> LastListedGames; 

        public static TtaGame CurrentGame;

        public static bool IsCurrentGameRefreshed()
        {
            if(SceneTransporter.CurrentGame == null)
            {
                return false;
            }

            if (SceneTransporter.CurrentGame.Boards == null)
            {
                return false;
            }

            return true;
        }

        public static String LastError = "";

        private static GameEventChannel _currentChannel;
        public static GameEventChannel CurrentChannel
        {
            get
            {
                if (_currentChannel == null)
                {
                    _currentChannel=new GameEventChannel();
                    LogRecorder.Log("Channel Created");
                }
                return _currentChannel;
            }
        }
    }
}
