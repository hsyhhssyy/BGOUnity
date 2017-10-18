using System;
using System.Linq;
using HSYErpBase.EntityDefinition.SessionModel;
using HSYErpBase.EntityDefinition.UserModel;
using HSYErpBase.NHibernate;
using NHibernate;

namespace HSYErpBase.Wcf.CommonApi
{
    public static class SessionManager
    {
        public const int TimeoutInSecond=60*60*24;
        /// <summary>
        /// 设置成常量是为了让编译器内联，从而避免服务器被黑入时修改此开关
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const bool DEBUG_ALLOW_EVERYTHING = true;

        public const int PermissionStateTrue = 1;
        public const int PermissionStateFalse = 0;

        public static User GetCurrentUser(String session)
        {
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var query =
                    hibernateSession.CreateQuery("from SessionToken where SessionGuid = ?");
                query.SetString(0, session);

                var sessionToken = query.List<SessionToken>().FirstOrDefault();

                if (sessionToken?.User == null || sessionToken.User <= 0)
                {
                    return null;
                }

                User u = hibernateSession.Get<User>(sessionToken.User);
                return u;
            }
        }

        public static WcfError ValidateSession(String session, String moduleName)
        {
            //第一步，获取用户
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {

                var query =
                    hibernateSession.CreateQuery("from SessionToken where SessionGuid = ?");
                query.SetString(0, session);

                var sessionToken=query.List<SessionToken>().FirstOrDefault();

                if (sessionToken?.User == null || sessionToken.User <= 0)
                {
                    return WcfError.InvalidSession;
                }

                //timeout
                if (sessionToken.LastOperationTime < DateTime.Now.Subtract(new TimeSpan(0,0,TimeoutInSecond)))
                {
                    return WcfError.SessionTimeout;
                }

                sessionToken.LastOperationTime=DateTime.Now;
                sessionToken.LastOperation = moduleName;

                hibernateSession.Update(sessionToken);

                hibernateSession.Flush();

                //找到Module
                query =
                    hibernateSession.CreateQuery("from ModuleFunction where end_date is null and FunctionQualifier = ?" );
                query.SetString(0, moduleName);

                var module = query.List<ModuleFunction>().FirstOrDefault();

                if (module == null)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (DEBUG_ALLOW_EVERYTHING)
                    // ReSharper disable once HeuristicUnreachableCode
                    #pragma warning disable 162
                    {
                        query =
                            hibernateSession.CreateQuery(
                                "from ModuleFunction where end_date is null and FunctionQualifier = ?");
                        query.SetString(0, "[DEBUG]" + moduleName);

                        module = query.List<ModuleFunction>().FirstOrDefault();

                        if (module == null)
                        {
                            module = new ModuleFunction
                            {
                                FunctionQualifier = "[DEBUG]" + moduleName,
                                BusinessName = moduleName,
                                ServiceName = moduleName
                            };
                            module.Id = (int) hibernateSession.Save(module);
                            hibernateSession.Flush();
                        }
                    }
                    else
                    // ReSharper disable once HeuristicUnreachableCode
                    {
                        return WcfError.NoSuchModule;
                    }
                    #pragma warning restore 162
                }

                int userId = sessionToken.User.Value;
                
                //找到该用户的所有用户组，看是否有权限

                var user = hibernateSession.Get<User>(userId);

                var privilege = PrivilegeApi.GetModuleFunctionPrivilege(userId, module.Id, hibernateSession);

                if (privilege != null)
                {
                    if (privilege.Status == PermissionStateTrue)
                    {
                        return WcfError.None;
                    }
                    else
                    {
                        return WcfError.InsufficientPrivilege;
                    }
                }

                //TODO:寻找用户组间接授权

                //没有授权
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (DEBUG_ALLOW_EVERYTHING)
                // ReSharper disable once HeuristicUnreachableCode
                #pragma warning disable 162
                {
                    var result=new ModuleFunctionMap();
                    result.State = PermissionStateTrue;
                    result.ModuleId = module.Id;
                    result.EntityId = userId;
                    result.EntityTypeId = EntityStructureApi.GetStructureByTypeName("UserGroup")?.Id??0;
                    result.Description = "[Debug]为用户[" + user.Name + "]赋予" + module.FunctionQualifier + "的执行权限。";
                    hibernateSession.Save(result);
                    hibernateSession.Flush();
                    return WcfError.None;
                }
                else
                {
                    // ReSharper disable once HeuristicUnreachableCode
                    return WcfError.InsufficientPrivilege;
                }
                #pragma warning restore 162
            }
        }
    }
}