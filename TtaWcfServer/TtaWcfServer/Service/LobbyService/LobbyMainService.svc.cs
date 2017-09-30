using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
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
            return null;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<List<GameRoom>> SearchActiveUnrankedGames(String sessionString,String keyword)
        {
            return null;
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
        public WcfServicePayload<GameRoom> JoinRoom(String sessionString, int roomId, String password)
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

    }
}
