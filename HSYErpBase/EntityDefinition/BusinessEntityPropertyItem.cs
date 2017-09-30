using System;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    [DataContract]
    public class BusinessEntityPropertyItem
    {
        [DataMember]
        public virtual ErpPropertyInfo PropertyInfo { get; set; }
        [DataMember]
        public virtual String Value { get; set; }

        public BusinessEntityPropertyItem Clone()
        {
            return (BusinessEntityPropertyItem)MemberwiseClone();
        }
    }
}