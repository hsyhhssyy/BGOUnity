using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.CSharpCode.Network.Wcf.Json
{
    public class JsonSerializer
    {
        public Dictionary<Type, DataContractAttribute> ReplaceAttributes=new Dictionary<Type, DataContractAttribute>();

        public JSONObject Serialize(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            #region 收束基础类型
            if (obj is Int32)
            {
                return new JSONObject((int)obj);
            }else if (obj is String)
            {
                return new JSONObject((String)obj);
            }
            else if(obj is Boolean)
            {
                return new JSONObject((Boolean)obj);
            }
            else if(obj is Int64)
            {
                return new JSONObject((Int64)obj);
            }

            #endregion

            if (obj is Enum)
            {
                return new JSONObject((int)obj);
            }

            JSONObject jsonObj=new JSONObject();
            var type = obj.GetType();
            DataContractAttribute attr;

            if (ReplaceAttributes.ContainsKey(type))
            {
                attr = ReplaceAttributes[type];
            }
            else
            {
                attr = type.GetCustomAttributes(typeof(DataContractAttribute), false).Cast<DataContractAttribute>().FirstOrDefault();
            }

            #region 检查IsReference
            if (attr!=null&&attr.IsReference)
            {
                String typeName = type.Name;
                String nameSpace = type.Namespace;
                if (attr.Name != null)
                {
                    typeName = attr.Name;
                }
                if (attr.Namespace != null)
                {
                    nameSpace = attr.Namespace;
                }

                jsonObj.AddField("__type", typeName+":#" + nameSpace);
            }
            #endregion

            //TODO 没有处理AmbiguousMatch（这个项目就算了）
            #region 检查IDictionary
            var dict = type.GetInterface("IDictionary`2");
            if (dict != null)
            {
                dict = type.GetInterface("IDictionary");
            }
            if (dict != null)
            {
                SerializeIDictionary(jsonObj, obj);
                return jsonObj;
            }

            #endregion

            #region 检查IEnumerable
            var iterator = type.GetInterface("IEnumerable`1");
            if (iterator != null)
            {
                iterator = type.GetInterface("IEnumerable");
            }
            if (iterator != null)
            {
                SerializeIEnumerable(jsonObj, obj);
                return jsonObj;
            }

            #endregion

            #region 常规Field和Property
            var allField = type.GetFields();
            foreach (var fieldInfo in allField)
            {
                var dmAttr = fieldInfo.GetCustomAttributes(typeof (DataMemberAttribute), true).FirstOrDefault();
                if (dmAttr != null)
                {
                    var value = fieldInfo.GetValue(obj);
                    jsonObj.AddField(fieldInfo.Name,Serialize(value));
                }
            }

            var allProperties = type.GetProperties();
            foreach (var propInfo in allProperties)
            {
                var dmAttr = propInfo.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
                if (dmAttr != null)
                {
                    var getMi = propInfo.GetGetMethod(true);
                    var value=getMi.Invoke(obj, null);
                    jsonObj.AddField(propInfo.Name, Serialize(value));
                }
            }
            #endregion

            return jsonObj;
        }

        private void SerializeIDictionary(JSONObject jsonObject, object dict)
        {

            var genericType = dict.GetType();

            var getEnumratorMi = genericType.GetMethod("GetEnumerator");
            var iterator = getEnumratorMi.Invoke(dict, null);

            while (true)
            {
                JSONObject pairObject = new JSONObject();

                var moveNextMi = iterator.GetType().GetMethod("MoveNext");
                var moveNext = (Boolean)moveNextMi.Invoke(iterator, null);
                if (moveNext == false)
                {
                    break;
                }

                var current = GetPropertyValue(iterator, "Current");

                //KeyValuePair
                if (ReplaceAttributes.ContainsKey(current.GetType()))
                {
                    var attr = ReplaceAttributes[current.GetType()];

                    pairObject.AddField("__type", attr.Name + ":#" + attr.Namespace);
                    pairObject.AddField("key", Serialize(GetPropertyValue(current, "Key")));
                    pairObject.AddField("value", Serialize(GetPropertyValue(current, "Value")));
                }
                else
                {
                    pairObject.AddField("Key", Serialize(GetPropertyValue(current, "Key")));
                    pairObject.AddField("Value", Serialize(GetPropertyValue(current, "Value")));
                }
                jsonObject.Add(pairObject);
            }

        }

        private void SerializeIEnumerable(JSONObject jsonObject, object list)
        {
            var genericType = list.GetType();

            var getEnumratorMi = genericType.GetMethod("GetEnumerator");
            var iterator = getEnumratorMi.Invoke(list, null);

            while (true)
            {
                var moveNextMi = iterator.GetType().GetMethod("MoveNext");
                var moveNext = (Boolean)moveNextMi.Invoke(iterator, null);
                if (moveNext == false)
                {
                    break;
                }

                var current = GetPropertyValue(iterator, "Current");

                jsonObject.Add(Serialize(current));
            }

        }

        private object GetPropertyValue(object obj, String propName)
        {
            var pi = obj.GetType().GetProperty(propName);
            var mi = pi.GetGetMethod();
            var value = mi.Invoke(obj, null);
            return value;
        }
    }
}
