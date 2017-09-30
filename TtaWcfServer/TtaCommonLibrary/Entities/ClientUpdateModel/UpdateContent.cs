using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HSYErpBase.EntityDefinition;

namespace TtaCommonLibrary.Entities.ClientUpdateModel
{
    [DataContract]
    public class UpdateContent:BasicEntity
    {
        [DataMember]
        public virtual int Version { get; set; }
        [DataMember]
        public virtual UpdateType Type { get; set; }
        [DataMember]
        public virtual String ContentGuid { get; set; }
        [DataMember]
        public virtual String PackagePath { get; set; }
        [DataMember]
        public virtual String DestinationRelatedPath { get; set; }
    }

    public enum UpdateType
    {
        SteamingAsset,
        RuleBookScript,
        Localization,
        Civilpedia
    }
}
