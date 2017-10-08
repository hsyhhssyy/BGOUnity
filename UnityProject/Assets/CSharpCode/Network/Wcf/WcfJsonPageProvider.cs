using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Network.Wcf.Entities;
using UnityEngine;

namespace Assets.CSharpCode.Network.Wcf
{
    public static class WcfJsonPageProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";

        public static TtaGame CreateRoom(JSONObject room)
        {
            WcfGame game =new WcfGame();
            game.Name = room.GetField("RoomName").str;
            game.RoomId = (int) room.GetField("Id").i;

            return game;
        }


    }
}
