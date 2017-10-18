using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.EntityDefinition.UserModel;
using HSYErpBase.Wcf;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaCommonLibrary.Entities.UserModel;
using TtaWcfServer.OffGameLogic.Ranking;

namespace TtaWcfServer.ServerApi.LobbyService
{
    public class LobbyServiceApi
    {
        static LobbyServiceApi()
        {
            RankMaster.UserName = "天梯管理员";

        }

        public static UserLite RankMaster=new UserLite();

        public static GameRoom FillUserLitesOfGameRoom(GameRoom room, WcfContext context)
        {
            room.Host = CreateUserLite(room.HostId, context);

            room.ObserverPlayer = CreateUserLites(room.Observers, context);
            room.RefereePlayer = CreateUserLites(room.Referees, context);
            room.JoinedPlayer = CreateUserLites(room.Players, context);

            return room;
        }

        public static GameRoom CreateStrFromUserLite(GameRoom room, WcfContext context)
        {
            room.Host = CreateUserLite(room.HostId, context);

            room.ObserverPlayer = CreateUserLites(room.Observers, context);
            room.RefereePlayer = CreateUserLites(room.Referees, context);
            room.JoinedPlayer = CreateUserLites(room.Players, context);

            return room;
        }

        public static UserLite CreateUserLite(int userId, WcfContext context)
        {
            if (userId == 0)
            {
                return RankMaster;
            }
            var user = context.HibernateSession.Get<User>(userId);
            var lite=new UserLite();
            lite.UserName =user.Name;
            lite.Id = user.Id;

            return lite;
        }
        public static List<UserLite> CreateUserLites(String uidStr, WcfContext context)
        {
            List<UserLite> result=new List<UserLite>();
            if (uidStr == null)
            {
                return result;
            }

            var strSp = uidStr.Replace(")", "").Split('(');
            foreach (var s in strSp)
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    result.Add(CreateUserLite(Int32.Parse(s), context));
                }
            }

            return result;
        }
    }
}