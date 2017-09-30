using System;
using System.Collections.Generic;
using System.Linq;
using HSYErpBase.EntityDefinition.UserModel;
using HSYErpBase.NHibernate;
using NHibernate;

namespace HSYErpBase.Wcf.UserModelApi
{
    public class UserApi
    {
        public static bool IsInGroupStr(int user, String groupStr, ISession hibnateSession = null)
        {
            var mySession = hibnateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                //用户直接包含
                if (groupStr.Contains("U" + user))
                {
                    return true;
                }

                var sp = groupStr.Split(",".ToCharArray());
                var groups = sp.Where(a => a.StartsWith("G")).Select(a => Convert.ToInt32(a.Substring(1))).ToList();

                foreach (var group in groups)
                {
                    if (IsInGroup(user, group, mySession))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                if (hibnateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        public static bool IsInGroup(int user, int group, ISession hibernateSession = null)
        {
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                var u = BusinessEntityApi.GetDetail<UserPropertyMap, User>(mySession, user).Payload;
                if (u.MainUserGroup == group)
                    return true;
                var query = mySession.CreateQuery("from UserGroupRship where end_date is null and RelatedEntityId = ?");
                query.SetInt32(0, user);
                var list = query.List<UserGroupRship>().Select(ship => ship.GroupId).ToList<int>();
                list.Add(u.MainUserGroup);
                foreach (var item in list)
                {
                    var id = item;
                    while (true)
                    {
                        if (id == group)
                            return true;
                        var g = mySession.Get<UserGroup>(id);
                        if (g == null || g.ParentId == 0)
                            break;
                        id = g.ParentId;
                    }
                }
                return false;
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        internal static GroupTrees GetUserGroupTrees(int user, ISession hibernateSession = null)
        {
            var trees = new GroupTrees();
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                var u = mySession.Get<User>(user);
                if (u == null)
                    return trees;
                var query = mySession.CreateQuery("from UserGroupRship where end_date is null and RelatedEntityId = ?");
                query.SetInt32(0, user);
                var list = query.List<UserGroupRship>().Select(ship => ship.GroupId).ToList<int>();
                list.Add(u.MainUserGroup);
                mySession.Evict(u);
                var groupMap = new Dictionary<int, GroupTreeNode>();
                foreach (var item in list)
                {
                    var id = item;
                    GroupTreeNode last = null;
                    while (true)
                    {
                        GroupTreeNode node;
                        bool canBreak = false;
                        if (!groupMap.TryGetValue(id, out node))
                        {
                            node = new GroupTreeNode();
                            node.GroupId = id;
                            groupMap.Add(id, node);
                        }
                        else
                            canBreak = true;
                        if (last != null)
                        {
                            last.Parent = node;
                            node.Children.Add(last);
                        }
                        if (canBreak)
                            break;
                        var g = mySession.Get<UserGroup>(id);
                        if (g.ParentId == 0)
                        {
                            trees.Roots.Add(node);
                            break;
                        }
                        id = g.ParentId;
                        last = node;                
                    }
                }
                foreach (var pair in groupMap)
                {
                    if (pair.Value.Children.Count == 0)
                    {
                        trees.Leaves.Add(pair.Value);
                    }
                }
                return trees;
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        internal static ISet<int> GetUserGroups(int user, ISession hibernateSession = null)
        {
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {
                var u = mySession.Get<User>(user);
                if (u == null)
                {
                    return null;
                }                
                var query = mySession.CreateQuery("from UserGroupRship where end_date is null and RelatedEntityId = ?");
                query.SetInt32(0, user);
                var list = query.List<UserGroupRship>().Select(ship => ship.GroupId).ToList<int>();
                list.Add(u.MainUserGroup);
                var set = new HashSet<int>();
                mySession.Evict(u);
                foreach (var item in list)
                {
                    var id = item;
                    while (true)
                    {
                        set.Add(id);                        
                        var g = mySession.Get<UserGroup>(id);
                        if (g == null || g.ParentId == 0)
                            break;
                        id = g.ParentId;
                    }
                }
                return set;
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        public static ISet<int> GetGroupUsers(int group, ISession hibernateSession = null)
        {
            var mySession = hibernateSession ?? NHibernateHelper.CurrentHelper.OpenSession();
            try
            {                
                var set = new HashSet<int>();
                GetGroupUsers(group, set, mySession);
                return set;
            }
            finally
            {
                if (hibernateSession == null)
                {
                    mySession.Dispose();
                }
            }
        }

        private static void GetGroupUsers(int group, ISet<int> users, ISession mySession)
        {
            var u = mySession.Get<UserGroup>(group);
            if (u == null)
            {
                return;
            }
            var query = mySession.CreateQuery("from UserGroupRship where end_date is null and BaseEntityId = ?");
            query.SetInt32(0, group);
            var list = query.List<UserGroupRship>().Select(ship => ship.UserId);
            users.UnionWith(list);
            mySession.Evict(u);
            query = mySession.CreateQuery("from UserGroup where end_date is null and ParentId = ?");
            query.SetInt32(0, group);
            list = query.List<UserGroup>().Select(g => g.Id);
            foreach (var item in list)
            {
                GetGroupUsers(item, users, mySession);
            }
        }
    }

    internal class GroupTreeNode
    {
        public int GroupId { get; set; }

        public GroupTreeNode Parent { get; set; }

        public IList<GroupTreeNode> Children { get; private set; }

        public GroupTreeNode()
        {
            Children = new List<GroupTreeNode>();
        }
    }

    internal class GroupTrees
    {
        public IList<GroupTreeNode> Roots { get; private set; }

        public IList<GroupTreeNode> Leaves { get; private set; }

        public GroupTrees()
        {
            Roots = new List<GroupTreeNode>();
            Leaves = new List<GroupTreeNode>();
        }
    }
}