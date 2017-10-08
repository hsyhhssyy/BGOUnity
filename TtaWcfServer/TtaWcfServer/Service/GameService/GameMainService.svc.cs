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
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.InGameLogic.WcfEntities;

namespace TtaWcfServer.Service.GameService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GameMainService
    {

        [OperationContract]
        public WcfServicePayload<WcfGame> QueryGameBoard(String sessionString,int roomId)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<WcfGame>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                GameRoom room = hibernateSession.Get<GameRoom>(roomId);
                if (room != null)
                {
                    //验证权限

                    //给出结果
                    var manager=GameManager.GetManager(room);
                    
                    WcfGame game = new WcfGame(manager.CurrentGame);

                    return new WcfServicePayload<WcfGame>(game);
                }
                
            }


            return new WcfServicePayload<WcfGame>(null);
        }

        [OperationContract]
        public WcfServicePayload<ActionResponse> PerformAction(String sessionString, int roomId, PlayerAction action)
        {
            return null;
        }

        [OperationContract]
        public WcfServicePayload<List<TtaBoard>> QueryJournal(String sessionString, int roomId)
        {
            return null;
        }

        [OperationContract]
        public WcfServicePayload<String> SendInGameChat(String sessionString, int matchId)
        {
            return null;
        }
    }
}
