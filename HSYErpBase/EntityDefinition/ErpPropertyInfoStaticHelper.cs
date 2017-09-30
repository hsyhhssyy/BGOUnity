using System;
using System.Collections.Generic;
using System.Linq;

namespace HSYErpBase.EntityDefinition
{
    public static class ErpPropertyInfoStaticHelper
    {
        #region ¾²Ì¬°ïÖúº¯Êý

        public static String GetValueByPropertyName(this List<BusinessEntityPropertyItem> props, String key)
        {
            if (null == props)
            {
                return null;
            }

            return (from prop in props where prop.PropertyInfo.Name == key select prop.Value).FirstOrDefault();
        }

        public static String GetValueByPropertyInfo(this List<BusinessEntityPropertyItem> props, ErpPropertyInfo key)
        {
            return (from prop in props where prop.PropertyInfo.PropertyId == key.PropertyId select prop.Value).FirstOrDefault();
        }

        public static List<BusinessEntityPropertyItem> GetItemsByPropertyName(this List<BusinessEntityPropertyItem> props, String key)
        {
            return (from prop in props where prop.PropertyInfo.Name == key select prop).ToList();
        }

        public static void AddValueByPropertyInfo(this List<BusinessEntityPropertyItem> props, ErpPropertyInfo key, String value)
        {
            foreach (var prop in props)
            {
                if (prop.PropertyInfo.PropertyId == key.PropertyId)
                {
                    prop.Value = value;
                    return;
                }
            }

            props.Add(new BusinessEntityPropertyItem { PropertyInfo = key, Value = value });
        }

        public static void AddValueByPropertyName(this List<BusinessEntityPropertyItem> props, String key, String value)
        {
            foreach (var prop in props)
            {
                if (prop.PropertyInfo.Name == key)
                {
                    prop.Value = value;
                    return;
                }
            }

        }

        public static List<ErpPropertyInfo> GetKeys(this List<BusinessEntityPropertyItem> props)
        {
            return props.Select(prop => prop.PropertyInfo).ToList();
        }

        public static bool ContainsNameKey(this List<BusinessEntityPropertyItem> props, String key)
        {
            return props.Any(prop => prop.PropertyInfo.Name == key && prop.Value != null);
        }

        public static bool ContainsInfoKey(this List<BusinessEntityPropertyItem> props, ErpPropertyInfo key)
        {
            return props.Any(prop => Equals(prop.PropertyInfo, key) && prop.Value != null);
        }

        #endregion
    }
}