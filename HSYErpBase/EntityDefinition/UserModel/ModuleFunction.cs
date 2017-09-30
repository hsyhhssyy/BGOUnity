using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition.UserModel
{
    /// <summary>
    /// 用于标记系统的功能
    /// </summary>
    [DataContract]
    public class ModuleFunction:BasicEntity
    {
        /// <summary>
        /// 一个英文的，唯一的字符串形式的标识符，和Validate函数的参数相同
        /// </summary>
        [DataMember]
        public virtual string FunctionQualifier { get; set; }
        /// <summary>
        /// 一个中文的，友好的模块名称（可能重复）
        /// </summary>
        [DataMember]
        public virtual string BusinessName { get; set; }
        /// <summary>
        /// 一个英文的，唯一的服务名（可能重复）
        /// </summary>
        [DataMember]
        public virtual string ServiceName { get; set; }
    }

}