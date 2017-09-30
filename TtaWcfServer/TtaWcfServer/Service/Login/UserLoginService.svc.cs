using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.Wcf;
using TtaWcfServer.ServiceBase.Login;

namespace TtaWcfServer.Service.Login
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [JavascriptCallbackBehavior(UrlParameterName = "jsoncallback")]
    public class UserLoginService : UserLoginBase, IUserLoginService
    {/*
        public new WcfServicePayload<String> Login(string session, string username, string sessionPasswordMd5)
        {
            return base.Login(session, username, sessionPasswordMd5);
        }*/
    }
}
