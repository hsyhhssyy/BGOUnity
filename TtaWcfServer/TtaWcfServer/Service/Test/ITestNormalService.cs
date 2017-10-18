using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.WcfEntities;
using TtaWcfServer.ServerApi.LobbyService;

namespace TtaWcfServer.Service.Test
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ITestNormalService”。
    [ServiceContract]
    public interface ITestNormalService
    {
        [OperationContract]
        WcfServicePayload<WcfGame> QueryGameBoard(String sessionString, int roomId);

    }
}
