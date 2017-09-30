using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HSYErpBase.Wcf.CommonApi
{

    /// <summary>
    /// 这个类型只是用于向用户展示权限所使用，并不对应任何数据库中实际存在的表，仅仅由Service动态生成
    /// </summary>
    [DataContract]
    public class ModulePrivilegeRelationship
    {
        /// <summary>
        /// 表示该条目对应的功能的Id
        /// </summary>
        [DataMember]
        public virtual int FunctionId { get; set; }
        /// <summary>
        /// 1表示允许，0（null）表示禁止
        /// </summary>
        [DataMember]
        public virtual int Status { get; set; }

        /// <summary>
        /// Source表示权限的来源，如果是来自用户组，就是用户组的名字，如果是来源于用户，就是用户的Name
        /// </summary>
        [DataMember]
        public virtual string Source { get; set; }
        /// <summary>
        /// 和上面那个变量一起指明权限的来源
        /// </summary>
        [DataMember]
        public virtual bool IsGroupLevel { get; set; }
    }
}
