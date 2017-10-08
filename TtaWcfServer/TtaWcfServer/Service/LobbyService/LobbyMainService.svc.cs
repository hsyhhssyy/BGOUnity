using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.OffGameLogic.Ranking;
using TtaWcfServer.ServerApi.LobbyService;

namespace TtaWcfServer.Service.LobbyService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LobbyMainService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<List<GameRoom>> ListMyGame(String sessionString)
        {
            var val= ValidateSessionApi.CurrentValidator.ValidateSession<List<GameRoom>>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var user = SessionManager.GetCurrentUser(sessionString);
                var query=hibernateSession.CreateQuery(
                    "from GameRoom where (HostId = ? or Players like ? or Observers like ? or Referees like ?) and end_date is null");
                var likeStr = "%(" + user.Id + ")%";
                query.SetInt32(0, user.Id);
                query.SetString(1, likeStr);
                query.SetString(2, likeStr);
                query.SetString(3, likeStr);

                var result = new List<GameRoom>();
                result.AddRange(query.List<GameRoom>());

                foreach (var gameRoom in result)
                {
                    LobbyServiceApi.FillUserLitesOfGameRoom(gameRoom,hibernateSession);
                }

                return new WcfServicePayload<List<GameRoom>>(result);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<List<GameRoom>> ListActiveUnrankedGames(String sessionString)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<List<GameRoom>>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var query = hibernateSession.CreateQuery(
                    "from GameRoom where RelatedMatchId is null and end_date is null");

                var result = new List<GameRoom>();
                result.AddRange(query.List<GameRoom>());

                foreach (var gameRoom in result)
                {
                    LobbyServiceApi.FillUserLitesOfGameRoom(gameRoom, hibernateSession);
                }

                return new WcfServicePayload<List<GameRoom>>(result);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<List<GameRoom>> SearchActiveUnrankedGames(String sessionString,String keyword)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<List<GameRoom>>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var query = hibernateSession.CreateQuery(
                    "from GameRoom where RelatedMatchId is null and end_date is null and RoomName like ?");
                query.SetString(0, "%" + keyword + "%");

                var result = new List<GameRoom>();
                result.AddRange(query.List<GameRoom>());

                foreach (var gameRoom in result)
                {
                    LobbyServiceApi.FillUserLitesOfGameRoom(gameRoom, hibernateSession);
                }

                return new WcfServicePayload<List<GameRoom>>(result);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<GameRoom> CreateUnrankedGame(String sessionString, GameRoom roomSetting,String password)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<GameRoom>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                roomSetting.Password = password;
                int id=(int)hibernateSession.Save(roomSetting);
                roomSetting.Id = id;

                hibernateSession.Flush();

                return new WcfServicePayload<GameRoom>(roomSetting);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<bool> StartRanking(String sessionString,String mode)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<bool>(sessionString);
            if (val != null)
            {
                return val;
            }
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);
                var user = SessionManager.GetCurrentUser(sessionString);
                RankingManager.Queue(RankingManager.RankMode1Vs1NewExpantion, user,context);
                return new WcfServicePayload<bool>(true);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<GameRoom> RankingStatus(String sessionString, String mode)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<GameRoom>(sessionString);
            if (val != null)
            {
                return val;
            }
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);
                var user = SessionManager.GetCurrentUser(sessionString);
                var match=RankingManager.QueryMatch(RankingManager.RankMode1Vs1NewExpantion, user,context);
                if (match == null)
                {
                    return new WcfServicePayload<GameRoom>(null);
                }
                if (match.Id <= 0)
                {
                    match.Id = (int)hibernateSession.Save(match);
                    hibernateSession.Flush();
                }

                return new WcfServicePayload<GameRoom>(match);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<bool> StopRanking(String sessionString, String mode)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<bool>(sessionString);
            if (val != null)
            {
                return val;
            }
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);
                var user = SessionManager.GetCurrentUser(sessionString);
                RankingManager.Dequeue(RankingManager.RankMode1Vs1NewExpantion, user,context);
                return new WcfServicePayload<bool>(true);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<GameRoom> JoinUnrankedRoom(String sessionString, int roomId, String password)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<GameRoom>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);
                var room=hibernateSession.Get<GameRoom>(roomId);
                if (room == null)
                {
                    return new WcfServicePayload<GameRoom>(null);
                }

                if (room.HasPassword == true)
                {
                    if (password != room.Password)
                    {
                        return new WcfServicePayload<GameRoom>(WcfError.UnknownError,"Password Incorrect");
                    }
                }
                var user = SessionManager.GetCurrentUser(sessionString);

                var sp = room.Players.Replace("(", "").Split(")".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Contains(user.Id.ToString()))
                {
                    return new WcfServicePayload<GameRoom>(WcfError.UnknownError, "Already Joined");
                }

                if (sp.Length >= room.PlayerMax)
                {
                    return new WcfServicePayload<GameRoom>(WcfError.UnknownError, "No Seat");
                }
                
                room.Players += "(" + user.Id + ")";

                hibernateSession.Save(room);
                if (sp.Length +1 >= room.PlayerMax)
                {
                    if (room.AutoStart == true)
                    {
                        GameManager.SetupNew(room,context);
                    }
                }

                hibernateSession.Flush();

                return new WcfServicePayload<GameRoom>(room);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<GameRoom> JoinUnrankedRoomAsObserver(String sessionString, int roomId, String password)
        {
            return null;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<GameRoom> JoinUnrankedRoomAsReferee(String sessionString, int roomId, String password)
        {
            return null;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<bool> QuitRoom(String sessionString, int roomId)
        {
            return null;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<bool> StartRoom(String sessionString, int roomId)
        {
            return null;
        }

    }
}
