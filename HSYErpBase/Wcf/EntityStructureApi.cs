using System;
using System.Collections.Generic;
using System.Linq;
using HSYErpBase.EntityDefinition;
using HSYErpBase.NHibernate;
using NHibernate;

namespace HSYErpBase.Wcf
{
    /// <summary>
    /// 便捷的提供对Structure的访问和查询的操作的Helper，内置了缓存。
    /// </summary>
    public class EntityStructureApi
    {
        private static List<EntityStructure> _cache; 

        private static List<EntityStructure> GetCachedStructure()
        {
            if (_cache == null)
            {
                return GetAllStructure();
            }
            else
            {
                return _cache;
            }
        }
        /// <summary>
        /// 获取所有Structure的列表
        /// </summary>
        /// <returns></returns>
        public static List<EntityStructure> GetAllStructure()
        {
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                var critera=hibernateSession.CreateCriteria<EntityStructure>();
                var list=critera.List<EntityStructure>();

                _cache=new List<EntityStructure>();
                _cache.AddRange(list);
                
            }

            return _cache;
        }
        /// <summary>
        /// 根据一个Structure的业务逻辑名称来获取该对象的实例。
        /// </summary>
        /// <param name="name">业务逻辑名称</param>
        /// <returns>Structure的实例</returns>
        public static EntityStructure GetStructureByName(String name)
        {
            var str= GetCachedStructure().FirstOrDefault(cache => cache.Name == name);
            if (str == null)
            {
                CacheMiss();
                str = GetCachedStructure().FirstOrDefault(cache => cache.Name == name);
            }
            return str;
        }

        /// <summary>
        /// 根据一个Structure的TypeName（类名）来获取该对象的实例。
        /// </summary>
        /// <param name="name">TypeName（类名）</param>
        /// <returns>Structure的实例</returns>
        public static EntityStructure GetStructureByTypeName(String name)
        {
            var str= GetCachedStructure().FirstOrDefault(cache => cache.TypeName == name);
            if (str == null)
            {
                CacheMiss();
                str = GetCachedStructure().FirstOrDefault(cache => cache.Name == name);
            }
            return str;
        }

        private static void CacheMiss()
        {
            GetAllStructure();
        }
    }
}