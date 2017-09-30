using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition.UserModel
{
    [DataContract]
    public class UserGroup:BusinessEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual String Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual UserGroupType GroupType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual int ParentId { get; set; }
        
    }

    public enum UserGroupType
    {
        System, Department, ChatGroup, WorkFlowGroup
    }
}
