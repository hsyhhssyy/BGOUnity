using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    [DataContract]
    public class EntityStructure
    {
        [DataMember, PrimaryKey]
        public virtual int Id { get; set; }

        [DataMember]
        public virtual String Name { get; set; }

        [DataMember]
        public virtual String TypeName { get; set; }
        [DataMember]
        public virtual String PropertyMapType { get; set; }

        [DataMember]
        public virtual DateTime? BeginDate { get; set; }
        [DataMember]
        public virtual DateTime? EndDate { get; set; }

        protected bool Equals(EntityStructure other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityStructure)obj);
        }

        public override int GetHashCode()
        {
            //这里没有办法，因为为了让NHibernate能够加载这个类，这个key不能readonly
            return Id.GetHashCode();
        }
    }
}