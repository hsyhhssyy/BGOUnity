using System.Collections.Generic;
using System.Linq;
using HSYErpBase.EntityDefinition.UserModel;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf.UserModelApi;
using NHibernate;

namespace HSYErpBase.Wcf.CommonApi
{
    public class PrivilegeApi
    {
        internal static ModulePrivilegeRelationship GetModuleFunctionPrivilege(int userId, int moduleId, ISession hibernateSession = null)
        {
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                var trees = UserApi.GetUserGroupTrees(userId, mySession);
                return GetModuleFunctionPrivilege(userId, moduleId, trees, mySession);
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        internal static ModulePrivilegeRelationship GetModuleFunctionPrivilege(int userId, int moduleId, GroupTrees userGroupTrees, ISession hibernateSession = null)
        {
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                var user = mySession.Get<User>(userId);
                var privilegeResult = new ModulePrivilegeRelationship()
                {
                    FunctionId = moduleId
                };
                //注意优先级
                //第一级，用户自己的设置
                var query =
                    mySession.CreateQuery("from ModuleFunctionMap where end_date is null and EntityTypeId = " +
                    EntityStructureApi.GetStructureByTypeName("User")?.Id
                    + " and EntityId = " + userId + " and ModuleId = " + moduleId);

                var result = query.List<ModuleFunctionMap>().FirstOrDefault();
                if (result != null)
                {
                    privilegeResult.IsGroupLevel = false;
                    privilegeResult.Source = user.Name;
                    privilegeResult.Status = result.State;
                    return privilegeResult;
                }

                privilegeResult.IsGroupLevel = true;
                var groupPrivileges = new Dictionary<int, int>();
                foreach (var leaf in userGroupTrees.Leaves)
                {
                    var node = leaf;
                    bool found = false;
                    while (true)
                    {
                        if (found)
                            groupPrivileges[node.GroupId] = -1;
                        else
                        {
                            int status;
                            if (!groupPrivileges.TryGetValue(node.GroupId, out status))
                            {
                                query =
                                    mySession.CreateQuery("from ModuleFunctionMap where end_date is null and EntityTypeId = " +
                                    EntityStructureApi.GetStructureByTypeName("UserGroup")?.Id
                                    + " and EntityId = ? and ModuleId = " + moduleId);
                                query.SetInt32(0, node.GroupId);
                                result = query.List<ModuleFunctionMap>().FirstOrDefault();
                                if (result != null)
                                {
                                    groupPrivileges.Add(node.GroupId, result.State);
                                    found = true;
                                }
                                else
                                {
                                    groupPrivileges.Add(node.GroupId, -1);
                                }
                            }
                        }
                        if (node.Parent == null)
                            break;
                        node = node.Parent;
                    }                    
                }
                if (groupPrivileges.ContainsValue(SessionManager.PermissionStateFalse))
                {
                    privilegeResult.Status = SessionManager.PermissionStateFalse;
                    privilegeResult.Source = mySession.Get<UserGroup>(
                        groupPrivileges.First(gp => gp.Value == SessionManager.PermissionStateFalse).Key).Name;
                    return privilegeResult;
                }
                else if (groupPrivileges.ContainsValue(SessionManager.PermissionStateTrue))
                {
                    privilegeResult.Status = SessionManager.PermissionStateTrue;
                    privilegeResult.Source = mySession.Get<UserGroup>(
                        groupPrivileges.First(gp => gp.Value == SessionManager.PermissionStateTrue).Key).Name;
                    return privilegeResult;
                }
                return null;
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }
    }
}