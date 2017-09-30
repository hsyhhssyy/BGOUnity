using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    /// <summary>
    /// BasicEntity����û�����Եģ�������ʵ��
    /// </summary>
    [DataContract]
    public class BasicEntity
    {
        [DataMember, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// �����ȡ����EntityTypeIdΪ-1����ʾ�����ͺ��Դ����ݣ�����ɲ��Ҷ�Ӧ��Structure
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