using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace HSYErpBase.Wcf
{
    [DataContract]
    public class WcfServicePayload
    {
        public WcfServicePayload(WcfError error, string message)
        {
            Error = error;
            Message = message;
        }
        [DataMember]
        public WcfError Error { get; private set; }
        [DataMember]
        public String Message { get; private set; }

        public static string ParseValidateResult(WcfError validateResult)
        {
            return validateResult.ToString();
        }
    }

    [DataContract]
    public class WcfServicePayload<T> : WcfServicePayload
    {
        public WcfServicePayload(WcfError error, string message, T payload) : base(error, message)
        {
            Payload = payload;
        }

        public WcfServicePayload(WcfError error, string message) : base(error, message)
        {
            Payload = default(T);
        }


        public WcfServicePayload(T payload) : base(WcfError.None, null)
        {
            Payload = payload;
        }

        [DataMember]
        public T Payload { get; private set; }
    }

    public enum WcfError
    {
        None = 0,
        [ErrorDescription("无法删除数据，系统中数据已被他人删除。")]
        DeleteMiss,
        [ErrorDescription("无法更新数据，系统中数据已被他人删除。")]
        UpdateMiss,
        [ErrorDescription("系统中已有此数据，无法重复添加。")]
        AddDuplicate,
        [ErrorDescription("权限不足，您没有权限访问此功能。")]
        InsufficientPrivilege,
        [ErrorDescription("凭据无效，请使用正确的用户名和密码重新登录后再试。")]
        InvalidSession,
        [ErrorDescription("登录超时，请关闭客户端并重新登录。")]
        SessionTimeout,
        [ErrorDescription("未知错误")]
        UnknownError,
        [ErrorDescription("无此功能")]
        NoSuchModule
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorDescriptionAttribute : Attribute
    {
        public String ErrorName;

        public ErrorDescriptionAttribute(string errorName)
        {
            ErrorName = errorName;
        }

    }

    public static class WcfErrorStatics
    {
        public static String GetName(this WcfError enumValue, Boolean nameInstead = true)
        {
            Type type = enumValue.GetType();
            string name = Enum.GetName(type, enumValue);
            if (name == null)
            {
                return null;
            }

            FieldInfo field = type.GetField(name);
            ErrorDescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(ErrorDescriptionAttribute)) as ErrorDescriptionAttribute;

            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute?.ErrorName;
        }
    }
}