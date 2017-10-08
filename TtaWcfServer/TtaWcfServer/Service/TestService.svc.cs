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

namespace TtaWcfServer.Service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TestService
    {
        [OperationContract]
        public void TestMain()
        {
            using (ISession hSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext("BlaBlaBla", hSession);
                GameRoom room = new GameRoom();
                room.GameRuleVersion = "Original-TTA2.0";
                GameManager.SetupNew(room, context);
            }
        }
    }
}
