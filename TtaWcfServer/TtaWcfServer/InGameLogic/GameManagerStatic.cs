using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;

namespace TtaWcfServer.InGameLogic
{
    public partial class GameManager
    {
        private static readonly Dictionary<int,GameManager> CachedGames=new Dictionary<int, GameManager>();
         
        /// <summary>
        /// 在内存和数据库中查找一个已有的比赛
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static GameManager GetManager(GameRoom room)
        {
            int roomid = room.Id;
            if (CachedGames.ContainsKey(roomid))
            {
                return CachedGames[roomid];
            }
            else
            {
                GameManager manager=new GameManager();
                var result=manager.LoadFromPesistance(room);
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
        /// <returns></returns>
        public static GameManager SetupNew(GameRoom room)
        {
            GameManager manager = new GameManager();
            manager.SetupNewGame(room);
            manager.SaveToPesistance();
            CachedGames.Add(room.Id, manager);


            return manager;
        }

        public static GameManager GetOrSetup(GameRoom room)
        {
            var manager = GetManager(room);
            if (manager == null)
            {
                return SetupNew(room);
            }
            return manager;
        }
        
    }
}