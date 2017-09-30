using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSYErpBase.Wcf.CommonApi;

namespace HSYErpBase.Wcf
{
    public class ValidateSessionApi
    {
        public static ValidateSessionApi CurrentValidator { get; set; }=new ValidateSessionApi();

        public virtual WcfServicePayload<TReturnType> ValidateSession<TReturnType>(String sessionString)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            if (sfs != null && sfs.Length < 2)
            {
                return new WcfServicePayload<TReturnType>(WcfError.InsufficientPrivilege,"未知的服务位置");
            }
            var mi = sfs?[1].GetMethod();
            if (mi?.ReflectedType != null)
            {
                String serviceName = mi.ReflectedType.FullName + "." + mi.Name;

                var validateResult = SessionManager.ValidateSession(sessionString,
                    "WCFService." + serviceName);
                if (validateResult != WcfError.None)
                {
                    return new WcfServicePayload<TReturnType>(WcfError.InsufficientPrivilege, "权限不足");
                }

                return null;
            }
            return new WcfServicePayload<TReturnType>(WcfError.InsufficientPrivilege, "未知的服务位置");

        }
    }
}
