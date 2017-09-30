using System;
using System.Runtime.Serialization;
using HSYErpBase.EntityDefinition;

namespace TtaCommonLibrary.Entities.FileStorageModel
{
    [DataContract]
    public class UploadedFileToken:BasicEntity
    {
        [DataMember]
        public virtual String Guid { get; set; }
        [DataMember]
        public virtual String Folder { get; set; }
        [DataMember]
        public virtual String OriginalName { get; set; }
        [DataMember]
        public virtual String Type { get; set; }

    }
}
