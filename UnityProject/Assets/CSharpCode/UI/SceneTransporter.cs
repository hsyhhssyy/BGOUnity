using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network;

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
    }
}
