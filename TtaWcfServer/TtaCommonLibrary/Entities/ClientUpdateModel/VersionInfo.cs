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
    public class VersionInfo:BasicEntity
    {
        [DataMember]
        public virtual int Version { get; set; }
        [DataMember]
        public virtual int LatestUpdatableVersion { get; set; }
        [DataMember]
        public virtual int EstimateFileSize { get; set; }
    }
}
