using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition.UserModel
{
    [DataContract]
    public class User:BusinessEntity
    {
        /// <summary>
        /// 固定忽略EntityTypeId
        /// </summary>
        public override int EntityTypeId => -1;

        [DataMember]
        public virtual string Name { get; set; }
        
        [DataMember]
        public virtual string Username { get; set; }

        //Password不能是DataMember，他永远不能被传到用户那里！
        /// <summary>
        /// 用户的Password（的MD5）
        /// </summary>
        public virtual string Password { get; set; }
       
        [DataMember]
        public virtual int MainUserGroup { get; set; }
        
    }
}
