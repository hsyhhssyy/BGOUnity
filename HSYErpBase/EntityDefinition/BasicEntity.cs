using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    /// <summary>
    /// BasicEntity就是没有属性的，单长表实体
    /// </summary>
    [DataContract]
    public class BasicEntity
    {
        [DataMember, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// 如果获取到的EntityTypeId为-1，表示该类型忽略此数据，否则可查找对应的Structure
        /// </summary>
        [DataMember]
        public virtual int EntityTypeId { get; set; } = -1;

        [DataMember]
        public virtual DateTime? BeginDate { get; set; }
        [DataMember]
        public virtual DateTime? EndDate { get; set; }

        [DataMember]
        public virtual DateTime? CreateDate { get; set; }
        [DataMember]
        public virtual String CreateBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifyDate { get; set; }
        [DataMember]
        public virtual String ModifyBy { get; set; }

        public virtual BasicEntity Clone()
        {
            BasicEntity obj = (BasicEntity)this.MemberwiseClone();

            return obj;
        }
    }
}