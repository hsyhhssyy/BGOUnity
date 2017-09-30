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
using TtaCommonLibrary.Entities.ClientUpdateModel;
using TtaPesistanceLayer.NHibernate;

namespace TtaWcfServer.Service.UpdateService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [JavascriptCallbackBehavior(UrlParameterName = "jsoncallback")]
    public class ClientUpdateService : IClientUpdateService
    {
        public WcfServicePayload<List<VersionInfo>> RequestVersion(string sessionString, int currentVersion)
        {
            var validateResult = ValidateSessionApi.
                CurrentValidator.ValidateSession<List<VersionInfo>>(sessionString);
            if (validateResult != null)
            {
                return validateResult;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var updates = new List<VersionInfo>();

                var query = hibernateSession.CreateQuery("from UpdateContent where Version > ? and EndDate is null");
                query.SetInt32(0, currentVersion);

                updates.AddRange(query.List<VersionInfo>());

                return new WcfServicePayload<List<VersionInfo>>(updates);
            }
        }

        public WcfServicePayload<List<UpdateContent>> RequestUpdate(string sessionString, int currentVersion)
        {
            var validateResult = ValidateSessionApi.
                CurrentValidator.ValidateSession<List<UpdateContent>>(sessionString);
            if (validateResult != null)
            {
                return validateResult;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var updates = new List<UpdateContent>();

                var query = hibernateSession.CreateQuery("from UpdateContent where Version > ? and EndDate is null");
                query.SetInt32(0, currentVersion);

                updates.AddRange(query.List<UpdateContent>());

                return new WcfServicePayload<List<UpdateContent>>(updates);
            }
        }
    }
}
