using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Assets.CSharpCode.Helper;
using Assets.CSharpScripts.Helper;
using JetBrains.Annotations;

namespace Assets.CSharpCode.Network.Wcf.Json
{
    public class JsonSerializerDeserializedEventArgs : EventArgs
    {
        public Object DeserializedObj { get; set; }
        public JSONObject Json { [UsedImplicitly] get; private set; }

        public JsonSerializerDeserializedEventArgs(object deserializedObj, JSONObject json)
        {
            DeserializedObj = deserializedObj;
            Json = json;
        }
    }

    public class JsonSerializer
    {
        public Dictionary<Type, DataContractAttribute> ReplaceAttributes { get; private set; } 

        public event EventHandler<JsonSerializerDeserializedEventArgs> ObjectDeserialized;

        public JsonSerializer()
        {
            ReplaceAttributes =
                new Dictionary<Type, DataContractAttribute>();
        }

        #region  序列化

        /// <summary>
        /// 序列化一个对象，并将其转化为JSONObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreDataMemberAttr"></param>
        /// <returns></returns>
        public JSONObject Serialize(Object obj,bool ignoreDataMemberAttr=false)
        {
            if (obj == null)
            {
                return null;
            }

            #region 收束基础类型
            if (obj is Int32)
            {
                return new JSONObject((int)obj);
            }
            else if (obj is String)
            {
                return new JSONObject((String)obj);
            }
            else if (obj is Boolean)
            {
                return new JSONObject((Boolean)obj);
            }
            else if (obj is Int64)
            {
                return new JSONObject((Int64)obj);
            }

            #endregion

            if (obj is Enum)
            {
                //不知道怎么处理，先放着
                // ReSharper disable once PossibleInvalidCastException
                return new JSONObject((int)obj);
            }

            JSONObject jsonObj = new JSONObject();
            var type = obj.GetType();

            var attr = ReplaceAttributes.ContainsKey(type) ? ReplaceAttributes[type] : type.GetCustomAttributes(typeof(DataContractAttribute), false).Cast<DataContractAttribute>().FirstOrDefault();

            #region 检查IsReference
            if (attr != null && attr.IsReference)
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

                jsonObj.AddField("__type", typeName + ":#" + nameSpace);
            }
            #endregion

            //没有处理AmbiguousMatch（这个项目就算了）
            #region 检查IDictionary
            var dict = type.GetInterface("IDictionary`2");
            if (dict != null)
            {
                SerializeIDictionary(jsonObj, obj);
                return jsonObj;
            }
            /*
            dict = type.GetInterface("IDictionary");
            if (dict != null)
            {
                SerializeIDictionary(jsonObj, obj);
                return jsonObj;
            }*/

            #endregion

            #region 检查IEnumerable
            var iterator = type.GetInterface("IEnumerable`1");
            if (iterator != null)
            {
                SerializeIEnumerable(jsonObj, obj);
                return jsonObj;
            }

            iterator = type.GetInterface("IEnumerable");
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
                var dmAttr = fieldInfo.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
                if (dmAttr != null || ignoreDataMemberAttr)
                {
                    var value = fieldInfo.GetValue(obj);
                    jsonObj.AddField(fieldInfo.Name, Serialize(value));
                }
            }

            var allProperties = type.GetProperties();
            foreach (var propInfo in allProperties)
            {
                var dmAttr = propInfo.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
                if (dmAttr != null|| ignoreDataMemberAttr)
                {
                    var getMi = propInfo.GetGetMethod(true);
                    var value = getMi.Invoke(obj, null);
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
            if (getEnumratorMi == null)
            {
                throw new ArgumentException("Dictionary doesn't implement GetEnumerator");
            }
            var iterator = getEnumratorMi.Invoke(dict, null);

            while (true)
            {
                JSONObject pairObject = new JSONObject();

                var moveNextMi = iterator.GetType().GetMethod("MoveNext");
                if (moveNextMi == null)
                {
                    throw new ArgumentException("Dictionary doesn't implement MoveNext");
                }
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
            if (getEnumratorMi == null)
            {
                throw new ArgumentException("IEnumerable doesn't implement GetEnumerator");
            }
            var iterator = getEnumratorMi.Invoke(list, null);
            

            while (true)
            {
                var moveNextMi = iterator.GetType().GetMethod("MoveNext");
                if (moveNextMi == null)
                {
                    throw new ArgumentException("IEnumerable doesn't implement MoveNext");
                }
                var moveNext = (Boolean)moveNextMi.Invoke(iterator, null);
                
                if (moveNext == false)
                {
                    break;
                }

                var current = GetPropertyValue(iterator, "Current");

                jsonObject.Add(Serialize(current));
            }

        }

        #endregion

        #region  反序列化

        public T Deserialize<T>(JSONObject jsonObject) where T : new()
        {
            return (T) Deserialize(jsonObject, typeof(T));
        }

        public Object Deserialize(JSONObject jsonObject,Type objectType)
        {
            if (jsonObject == null)
            {
                return null;
            }

            //Object类型等同于没给
            if (objectType == typeof(Object))
            {
                objectType = null;
            }

            #region 收束Enum类型

            if (objectType!=null&&objectType.IsSubclassOf(typeof(Enum)))
            {
                return Enum.Parse(objectType, jsonObject.i.ToString());
            }

            #endregion

            #region 收束基础类型

            if (jsonObject.IsNumber)
            {
                if (jsonObject.i > Int32.MaxValue)
                {
                    return jsonObject.i;
                }
                else
                {
                    return (Int32)jsonObject.i;
                }
            }
            if (jsonObject.IsBool)
            {
                return jsonObject.b;
            }
            if (jsonObject.IsString)
            {
                return jsonObject.str;
            }

            if (objectType ==typeof(Int32))
            {
                return (int)jsonObject.i;
            }
            else if (objectType == typeof(String))
            {
                return jsonObject.str;
            }
            else if (objectType == typeof(Boolean))
            {
                return jsonObject.b;
            }
            else if (objectType == typeof(Int64))
            {
                //为了防止将来有一天i被改成int（万一手贱呢）
                // ReSharper disable once RedundantCast
                return (Int64)jsonObject.i;
            }

            #endregion

            #region 探测Collection类型

            if (objectType != null)
            {
                //猜测Collection的内容
                var iteratorType = objectType.GetInterface("IDictionary`2");
                if (iteratorType != null)
                {
                    var args = iteratorType.GetGenericArguments();
                    return DeserializeIDictionary(jsonObject, objectType, args[0], args[1]);
                }
                /*
                iteratorType = objectType.GetInterface("IDictionary");
                if (iteratorType != null)
                {
                    return DeserializeIDictionary(jsonObject, objectType, typeof(object), typeof(object));
                }
                */
                iteratorType = objectType.GetInterface("ICollection`1");
                if (iteratorType != null)
                {
                    var args = iteratorType.GetGenericArguments();
                    return DeserializeICollection(jsonObject, objectType, args[0]);
                }

                iteratorType = objectType.GetInterface("ICollection");
                if (iteratorType != null)
                {
                    return DeserializeICollection(jsonObject, objectType, typeof(object));
                }

                if (jsonObject.IsArray)
                {
                    //试着用List<Object>来承载
                    return DeserializeICollection(jsonObject, objectType, typeof(object));
                }

                //这里程序控制流向外跳，继续正常的流程
            }
            else
            {
                if (jsonObject.IsArray)
                {
                    //如果是空List，则初始化为List<Object>
                    if (jsonObject.list.Count == 0)
                    {
                        return new List<Object>();
                    }
                    //如果他含有的第一个元素含有__type属性
                    var firstElement = jsonObject.list[0];
                    if (firstElement.HasField("__type"))
                    {
                        Type firstElementType = GuessType(firstElement.TryGetField("__type").str);
                        if (firstElementType == null)
                        {
                            throw new ArgumentException("Can't find type for " + firstElement.TryGetField("__type").str);
                        }
                        var iteratorType = firstElementType.GetInterface("KeyValuePair`2");
                        if (iteratorType != null)
                        {
                            //是KeyValuePair
                            var args = iteratorType.GetGenericArguments();

                            //构造Dictionary的Type
                            var dictType = typeof(Dictionary<object, object>).GetGenericTypeDefinition()
                                .MakeGenericType(
                                    args[0], args[1]);

                            return DeserializeIDictionary(jsonObject, dictType, args[0], args[1]);
                        }
                        else
                        {
                            //不是KeyValuePair
                            var listType = typeof(List<object>).GetGenericTypeDefinition()
                                .MakeGenericType(
                                    firstElementType);
                            return DeserializeICollection(jsonObject, listType, firstElementType);
                        }

                    }
                    else
                    {
                        //或者第一个元素含有Key，Value两个字段
                        if ((firstElement.HasField("Key") || firstElement.HasField("key"))
                            &&(firstElement.HasField("Value") || firstElement.HasField("value")))
                        {
                            return DeserializeIDictionary(jsonObject, typeof(Dictionary<object,object>),typeof(object), typeof(object));
                        }
                    }

                    //否则就是List
                    return DeserializeICollection(jsonObject, typeof(List<object>), typeof(object));
                }
            }

            #endregion

            if (jsonObject.IsNull)
            {
                return null;
            }

            //确认Type
            if (jsonObject.HasField("__type"))
            {
                Type t = GuessType(jsonObject.TryGetField("__type").str);

                if (t == null)
                {
                    throw new ArgumentException("Can't find type for "+ jsonObject.TryGetField("__type").str);
                }
                if (objectType != null && objectType != t)
                {
                    //可能是潜在的继承关系，依照__type进行
                    if (t.IsSubclassOf(objectType))
                    {
                        objectType = t;
                    }
                    else
                    {
                        throw new ArgumentException("Type "+t.Name+" specified in Json is not a sub-class of given type "+objectType.Name);
                    }
                }
                else
                {
                    objectType = t;
                }
            }

            if (objectType == null)
            {
                throw new ArgumentException("Can't Determine type in deserialization");
            }

            //构造空对象
            var ctor= objectType.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public,null,Type.EmptyTypes,null);
            if (ctor == null)
            {
                throw new ArgumentException("Type "+objectType.Name+" doesn't have empty ctor");
            }
            Object obj = ctor.Invoke(null);
            if (obj == null)
            {
                throw new ArgumentException("Type " + objectType.Name + " ctor return null");
            }

            //写入属性

            var keySet = jsonObject.keys;

            foreach (var key in keySet)
            {
                var valueJson = jsonObject.GetField(key);

                //寻找Field
                var fieldInfo = objectType.GetField(key);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(obj,Deserialize(valueJson,fieldInfo.FieldType));
                    continue;
                }

                //寻找Property
                var propertyInfo = objectType.GetProperty(key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, Deserialize(valueJson, propertyInfo.PropertyType),null);
                }
            }

            if (ObjectDeserialized != null)
            {
                var args = new JsonSerializerDeserializedEventArgs(obj, jsonObject);
                ObjectDeserialized(this,args);
                obj = args.DeserializedObj;
            }
            return obj;
        }

        private Type GuessType(String typeStr)
        {
            var name = typeStr.CutBefore(":#");
            var nameSpace = typeStr.CutAfter(":#");

            Type t;
            if (ReplaceAttributes.Any(pair => pair.Value.Name == name && pair.Value.Namespace == nameSpace))
            {
                var attrPair = ReplaceAttributes.FirstOrDefault(pair =>
                    pair.Value.Name == name && pair.Value.Namespace == nameSpace);
                t = attrPair.Key;
            }
            else
            {
                t = Type.GetType(nameSpace + "." + name);
            }
            return t;
        }

        private Object DeserializeICollection(JSONObject listObject, Type collectionType, Type elementType)
        {
            //构造集合
            var ctor = collectionType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                return new List<Object>();
            }
            Object obj = ctor.Invoke(null);
            if (obj == null)
            {
                return new List<Object>();
            }

            var addMethodInfo = collectionType.GetMethod("Add", new[] {elementType});
            if (addMethodInfo == null)
            {
                throw new ArgumentException("Type ICollection or ICollection`1 doesn't have Add method.");
            }

            if (listObject == null||listObject.IsNull)
            {
                return obj;
            }

            //替换Array
            foreach (var elementJson in listObject.list)
            {
                var element = Deserialize(elementJson, elementType);
                addMethodInfo.Invoke(obj, new[] {element});
            }

            return obj;
        }

        private Object DeserializeIDictionary(JSONObject listObject, Type collectionType,Type keyType,Type valueType)
        {
            //构造集合
            var ctor = collectionType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                return new List<Object>();
            }
            Object obj = ctor.Invoke(null);
            if (obj == null)
            {
                return new List<Object>();
            }

            var addMethodInfo = collectionType.GetMethod("Add", new[] { keyType,valueType });
            if (addMethodInfo == null)
            {
                throw new ArgumentException("Type IDictionary or IDictionary`2 doesn't have Add method.");
            }

            if (listObject == null || listObject.IsNull)
            {
                return obj;
            }

            foreach (var elementJson in listObject.list)
            {
                //Key 
                JSONObject keyJson=null;
                if (elementJson.HasField("Key"))
                {
                    keyJson = elementJson.GetField("Key");
                }
                else if (elementJson.HasField("key"))
                {
                    keyJson = elementJson.GetField("key");
                }
                var key = Deserialize(keyJson, keyType);

                JSONObject valueJson = null;
                if (elementJson.HasField("Value"))
                {
                    valueJson = elementJson.GetField("Value");
                }else if (elementJson.HasField("value"))
                {
                    valueJson = elementJson.GetField("value");
                }
                var value = Deserialize(valueJson, valueType);
                
                //这里不明确指定Object会出现奇怪的错误，虽然这仅仅是一个语法糖
                // ReSharper disable once RedundantExplicitArrayCreation
                addMethodInfo.Invoke(obj, new Object[] { key,value });
            }

            return obj;
        }

        #endregion

        private object GetPropertyValue(object obj, String propName)
        {
            var pi = obj.GetType().GetProperty(propName);
            if (pi == null)
            {
                throw new ArgumentException("Type " + obj.GetType().Name + " doesn't have property " + propName);
            }
            var mi = pi.GetGetMethod();
            if (mi == null)
            {
                throw new ArgumentException("Type "+obj.GetType().Name+" property "+propName+" has no get method");
            }
            var value = mi.Invoke(obj, null);
            return value;
        }
    }
}
