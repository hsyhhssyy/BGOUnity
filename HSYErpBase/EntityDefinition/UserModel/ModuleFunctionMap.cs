using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition.UserModel
{
    [DataContract]
    public class ModuleFunctionMap:BasicEntity
    {

        [DataMember]
        public virtual int ModuleId { get; set; }

        [DataMember]
        public virtual int EntityId { get; set; }

        [DataMember]
        public override int EntityTypeId { get; set; }

        [DataMember]
        public virtual int State { get; set; }
        
        [DataMember]
        public virtual string Description { get; set; }
    }
}
