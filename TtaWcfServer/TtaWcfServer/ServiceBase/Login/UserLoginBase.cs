using System;
using System.Security.Cryptography;
using System.Text;
using NHibernate;
using TtaPesistanceLayer.NHibernate;
using System.Linq;
using HSYErpBase.EntityDefinition.SessionModel;
using HSYErpBase.EntityDefinition.UserModel;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;

namespace TtaWcfServer.ServiceBase.Login
{
    public class UserLoginBase
    {
        /// <summary>
        /// 这是所有服务里唯一一个不需要SessionString的，也是唯一一个不使用WcfServicePayload的服务
        /// </summary>
        /// <returns></returns>
        public string GenerateSessionKey()
        {
            SessionToken token = new SessionToken();
            token.GenerationTime = DateTime.Now;
            token.SessionGuid = Guid.NewGuid().ToString("N").ToUpper();

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                hibernateSession.Save(token);
                hibernateSession.Flush();
                return token.SessionGuid;
            }
        }

        /// <summary>
        /// 这里注释一下，登录时传递的是SessionGuid，用户名和
        /// （Session + 密码的MD5 ）的MD5
        /// 数据库里存的是密码的MD5
        /// （明文密码是老生常谈了）
        /// </summary>
        /// <param name="session"></param>
        /// <param name="username"></param>
        /// <param name="sessionPasswordMd5"></param>
        /// <returns></returns>
        public WcfServicePayload<String> Login(string session, string username, string sessionPasswordMd5)
        {
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var query =
                    hibernateSession.CreateQuery("from SessionToken where SessionGuid = ?");
                query.SetString(0, session);

                var token = query.List<SessionToken>().FirstOrDefault();
                //Token 60秒内必须使用
                if (token == null)
                {
                    return new WcfServicePayload<String>(WcfError.InvalidSession, "NoSuchSession");
                }
                if (token.GenerationTime < DateTime.Now.Subtract(new TimeSpan(60 * 10000000)))
                {
                    hibernateSession.Delete(token);
                    return new WcfServicePayload<String>(WcfError.InvalidSession, "LoginSessionTimeout");
                }

                //把User查出来
                query =
                    hibernateSession.CreateQuery("from User where username = ? and end_date is null");
                query.SetString(0, username);

                var user = query.List<User>().FirstOrDefault();

                //注意，用户名或者密码不正确时的提示信息必须完全一致，防止爆破用户名
                if (user == null)
                {
                    return new WcfServicePayload<String>(WcfError.InvalidSession, "用户名或密码不正确");
                }

                String passwordMd5 = user.Password;

                var md5Crpt = MD5.Create();
                var result =
                    BitConverter.ToString(md5Crpt.ComputeHash(Encoding.UTF8.GetBytes(session + passwordMd5))).Replace("-", "");

                if (result != sessionPasswordMd5)
                {
                    return new WcfServicePayload<String>(WcfError.InvalidSession, "用户名或密码不正确");
                }

                token.User = user.Id;
                token.LastOperationTime = DateTime.Now;
                token.LastOperation = "UserLoginService.Login";

                hibernateSession.Update(token);

                hibernateSession.Flush();

                return new WcfServicePayload<String>(user.Id.ToString());
            }
        }
    }
}