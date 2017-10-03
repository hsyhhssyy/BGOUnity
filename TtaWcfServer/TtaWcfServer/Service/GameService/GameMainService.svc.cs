using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.Wcf;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.Service.GameService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GameMainService
    {

        [OperationContract]
        public WcfServicePayload<TtaBoard> QueryGameBoard(String sessionString,int matchId)
        {
            return null;
        }

        [OperationContract]
        public WcfServicePayload<ActionResponse> PerformAction(String sessionString, int matchId, PlayerAction action)
        {
            return null;
        }

        [OperationContract]
        public WcfServicePayload<List<TtaBoard>> QueryJournal(String sessionString, int matchId)
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
