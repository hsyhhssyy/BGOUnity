using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.EntityDefinition.UserModel;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;

namespace TtaWcfServer.OffGameLogic.Ranking
{
    public class RankingManager
    {
        public static Dictionary<String, List<User>> RankQueue;
        public static Dictionary<String,List<GameRoom>> UnreportMatch; 

        public const String RankMode1Vs1NewExpantion = "RankMode1vs1NewExpantion";

        public static void StartRankingManager()
        {
            RankQueue=new Dictionary<string, List<User>>();
            RankQueue.Add(RankMode1Vs1NewExpantion, new List<User>());

            UnreportMatch=new Dictionary<string, List<GameRoom>>();
            UnreportMatch.Add(RankMode1Vs1NewExpantion, new List<GameRoom>());
        }

        public static GameRoom QueryMatch(String mode, User u)
        {
            if (mode == RankMode1Vs1NewExpantion)
            {
                var queue = UnreportMatch[RankMode1Vs1NewExpantion];
                var game=queue.FirstOrDefault(gameRoom => gameRoom.Players.Contains("(" + u.Id + ")"));
                return game;
            }
            return null;
        }


        public static void Dequeue(String mode, User u)
        {
            if (mode == RankMode1Vs1NewExpantion)
            {
                var queue = RankQueue[RankMode1Vs1NewExpantion];
                if (queue.Contains(u))
                {
                    queue.Remove(u);
                }
            }
        }

        public static void Queue(String mode, User u)
        {
            if (mode == RankMode1Vs1NewExpantion)
            {
                var queue = RankQueue[RankMode1Vs1NewExpantion];
                if (queue.Count > 0)
                {
                    var user1 = queue[0];
                    queue.RemoveAt(0);
                    //
                    CreateMatch(user1, u);
                }
                else
                {
                    queue.Add(u);
                }
            }
        }

        private static void CreateMatch(User user1, User user2)
        {
            GameRoom room=new GameRoom();
            room.Ranked = true;
            room.AutoStart = true;
            room.ObserverMax = 0;
            room.RefereeMax = 0;
            room.HostId = 0;
            room.RoomName = "1v1天梯比赛" + DateTime.Now.ToString("yyyyMMddHHmmss");
            room.Players += "(" + user1.Id + ")";
            room.Players += "(" + user2.Id + ")";

            room.GameRuleVersion = "Original-TTA2.0";

            GameManager.SetupNew(room);

            UnreportMatch[RankMode1Vs1NewExpantion].Add(room);
        }
    }
}