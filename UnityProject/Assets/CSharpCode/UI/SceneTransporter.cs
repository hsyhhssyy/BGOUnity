using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Network;

namespace Assets.CSharpCode.UI
{
    public static class SceneTransporter
    {
        public static Dictionary<String, Object> data;

        public static IServerAdapter server;

        public static List<TtaGame> LastListedGames; 

        public static TtaGame CurrentGame;
    }
}
