using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.EntityDefinition.UserModel;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaCommonLibrary.Entities.UserModel;

namespace TtaWcfServer.ServerApi.LobbyService
{
    public class LobbyServiceApi
    {
        public static GameRoom FillUserLitesOfGameRoom(GameRoom room, ISession session)
        {
            room.Host = CreateUserLite(room.HostId, session);

            room.ObserverPlayer = CreateUserLites(room.Observers, session);
            room.RefereePlayer = CreateUserLites(room.Referees, session);
            room.JoinedPlayer = CreateUserLites(room.Players, session);

            return room;
        }

        public static GameRoom CreateStrFromUserLite(GameRoom room, ISession session)
        {
            room.Host = CreateUserLite(room.HostId, session);

            room.ObserverPlayer = CreateUserLites(room.Observers, session);
            room.RefereePlayer = CreateUserLites(room.Referees, session);
            room.JoinedPlayer = CreateUserLites(room.Players, session);

            return room;
        }

        public static UserLite CreateUserLite(int userId, ISession session)
        {
            var user = session.Get<User>(userId);
            var lite=new UserLite();
            lite.UserName =user.Name;
            lite.UserId = user.Id;

            return lite;
        }
        public static List<UserLite> CreateUserLites(String uidStr, ISession session)
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
                    result.Add(CreateUserLite(Int32.Parse(s), session));
                }
            }

            return result;
        }
    }
}