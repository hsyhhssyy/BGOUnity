using System;

namespace HSYErpBase.EntityDefinition
{

    /// <summary>
    /// 表示本属性在被本地化存储时，该字段为Key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute
    {

    }
}