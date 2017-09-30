using System;
using System.Collections.Generic;
using System.Linq;
using HSYErpBase.NHibernate;
using NHibernate;
using NHibernate.Criterion;

namespace HSYErpBase.EntityDefinition
{
    public static class PropertyHelper
    {
        public const int PropertyRShipIsCategoryPropertyOf = 4;

        public static List<ErpPropertyInfo> GetPropertyForEntity(ISession session, int entityId)
        {
            var rshipCriteria = session.CreateCriteria<ErpPropertyInfo>();
            rshipCriteria.Add(Restrictions.Not(Restrictions.IsNotNull("EndDate")));
            rshipCriteria.Add(Restrictions.Eq("StructureId", entityId));

            var list = rshipCriteria.List<ErpPropertyInfo>().ToList();
            
            return list;
        }

        public static List<ErpPropertyInfo> GetPropertyForCategory(ISession session,int categoryId)
        {
            var rshipCriteria=session.CreateCriteria<PropertyRship>();
            rshipCriteria.Add(Restrictions.Eq("RelationshipTypeId", PropertyRShipIsCategoryPropertyOf));
            rshipCriteria.Add(Restrictions.Eq("ToEntityId", categoryId));

            var list=rshipCriteria.List<PropertyRship>();

            List < ErpPropertyInfo > result=new List<ErpPropertyInfo>();

            foreach (var pr in list)
            {
                var query=session.CreateQuery("from ErpPropertyInfo where r_id = " + pr.FromEntityId);
                var plist = query.List<ErpPropertyInfo>().FirstOrDefault();
                if (plist != null)
                {
                    plist.Inherited = false;
                    result.Add(plist);
                }
            }

            return result;
        }

        public static void SavePropertyForCategory(ISession session, int categoryId, ErpPropertyInfo prop)
        {

            int id =(int) session.Save(prop);
            prop.PropertyId = id;
            
            PropertyRship pr=new PropertyRship();
            pr.FromEntityId = id;
            pr.ToEntityId = categoryId;
            pr.RelationshipTypeId = PropertyRShipIsCategoryPropertyOf;

            session.Save(pr);
            
        }

        /// <summary>
        /// 从用户数据转换到数据库字段值
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="value">用户输入的数据</param>
        /// <returns>数据库字段应存数值</returns>
        public static String FormatValue(ErpPropertyInfo pro, String value)
        {
            return value;
        }
        /// <summary>
        /// 从数据库字段值转换到用户数据
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="value">数据库字段</param>
        /// <returns>用户应看到的数据</returns>
        public static String ParseValue(ErpPropertyInfo pro, String value)
        {
            return value;
        }
    }


}