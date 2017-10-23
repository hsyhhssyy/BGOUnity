using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.Wcf;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic
{
    public partial class GameManager
    {
        private static readonly Dictionary<int,GameManager> CachedGames=new Dictionary<int, GameManager>();

        /// <summary>
        /// 在内存和数据库中查找一个已有的比赛
        /// </summary>
        /// <param name="room"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static GameManager GetManager(GameRoom room,WcfContext context)
        {
            int roomid = room.Id;
            if (CachedGames.ContainsKey(roomid))
            {
                return CachedGames[roomid];
            }
            else
            {
                GameManager manager=new GameManager();
                var result=manager.LoadFromPesistance(room, context);
                if (result == false)
                {
                    return null;
                }
                CachedGames.Add(roomid,manager);
                return manager;
            }
        }

        /// <summary>
        /// 根据指定的room设置一个新的GameManager
        /// </summary>
        /// <param name="room"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static GameManager SetupNew(GameRoom room, WcfContext context)
        {
            GameManager manager = new GameManager();
            manager.SetupNewGame(room);
            manager.SaveToPesistance( context);

            if(!CachedGames.ContainsKey(room.Id))
            {
                CachedGames.Add(room.Id, manager);
            }
            else
            {
                CachedGames[room.Id] = manager;
            }


            return manager;
        }

        public static GameManager GetOrSetup(GameRoom room, WcfContext context)
        {
            var manager = GetManager(room, context);
            if (manager == null)
            {
                return SetupNew(room, context);
            }
            return manager;
        }

        public static GameManager ReloadFromPesistance(GameRoom room, WcfContext context)
        {
            GameManager manager = new GameManager();
            var result = manager.LoadFromPesistance(room, context);

            if (!CachedGames.ContainsKey(room.Id))
            {
                CachedGames.Add(room.Id, manager);
            }
            else
            {
                CachedGames[room.Id] = manager;
            }
            return manager;
        }

        public static void PesistAllMatch(WcfContext context)
        {
            foreach (var gameManager in CachedGames)
            {
                gameManager.Value.SaveToPesistance(context);
            }
        }
        
    }
}