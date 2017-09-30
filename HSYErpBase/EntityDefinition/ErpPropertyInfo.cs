using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    [DataContract]
    public class ErpPropertyInfo
    {
        [DataMember]
        public virtual int PropertyId { get; set; } = -1;

        [DataMember]
        public virtual String Name { get; set; }

        [DataMember]
        public virtual bool Inherited { get; set; }
        [DataMember]
        public virtual int StructureId { get; set; }
        [DataMember]
        public virtual String DatabaseColumn { get; set; }
        [DataMember]
        public virtual String TableName { get; set; }
        [DataMember]
        public virtual String CSharpType { get; set; }
        [DataMember]
        public virtual String DbType { get; set; }
        [DataMember]
        public virtual DateTime? BeginDate { get; set; }
        [DataMember]
        public virtual DateTime? EndDate { get; set; }

        protected bool Equals(ErpPropertyInfo other)
        {
            return PropertyId == other.PropertyId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ErpPropertyInfo)obj);
        }

        public override int GetHashCode()
        {
            //这里没有办法，因为为了让NHibernate能够加载这个类，这个key不能readonly
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return PropertyId.GetHashCode();
        }


    }
}