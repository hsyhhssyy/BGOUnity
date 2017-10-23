using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.WcfEntities;
using TtaWcfServer.ServerApi.LobbyService;

namespace TtaWcfServer.Service.Test
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TestService
    {

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public void RegenerateMatchData(String sessionString, int roomId)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<WcfGame>(sessionString);
            if (val != null)
            {
                return;
            }


            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var user=SessionManager.GetCurrentUser(sessionString);
                if (user.Id != 1&& user.Id != 2)
                {
                    return;
                }

                WcfContext context = new WcfContext(sessionString, hibernateSession);
                
                GameRoom room = hibernateSession.Get<GameRoom>(roomId);
                if (room != null)
                {
                    room.JoinedPlayer = LobbyServiceApi.CreateUserLites(room.Players, context);
                    room.ObserverPlayer = LobbyServiceApi.CreateUserLites(room.Observers, context);
                    room.RefereePlayer = LobbyServiceApi.CreateUserLites(room.Referees, context);

                    hibernateSession.CreateQuery("delete from MatchTableContent where id = " + room.RelatedMatchId);

                    //给出结果
                    var manager = GameManager.SetupNew(room, context);
                    manager.SaveToPesistance(context);

                    hibernateSession.Update(room);

                    hibernateSession.Flush();

                }
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        public void ReloadFromPesistance(String sessionString, int roomId)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<WcfGame>(sessionString);
            if (val != null)
            {
                return;
            }


            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var user = SessionManager.GetCurrentUser(sessionString);
                if (user.Id != 1 && user.Id != 2)
                {
                    return;
                }

                WcfContext context = new WcfContext(sessionString, hibernateSession);

                GameRoom room = hibernateSession.Get<GameRoom>(roomId);
                if (room != null)
                {
                    room.JoinedPlayer = LobbyServiceApi.CreateUserLites(room.Players, context);
                    room.ObserverPlayer = LobbyServiceApi.CreateUserLites(room.Observers, context);
                    room.RefereePlayer = LobbyServiceApi.CreateUserLites(room.Referees, context);
                    
                    //给出结果
                    GameManager.ReloadFromPesistance(room, context);
                    
                    hibernateSession.Flush();

                }
            }
        }
    }
}
