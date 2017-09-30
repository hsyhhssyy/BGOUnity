using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.Wcf;
using TtaCommonLibrary.Entities.ClientUpdateModel;

namespace TtaWcfServer.Service.UpdateService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IClientUpdateService”。
    [ServiceContract]
    public interface IClientUpdateService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        WcfServicePayload<List<VersionInfo>> RequestVersion(String sessionString, int currentVersion);
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        WcfServicePayload<List<UpdateContent>> RequestUpdate(String sessionString,int currentVersion);
    }
}
