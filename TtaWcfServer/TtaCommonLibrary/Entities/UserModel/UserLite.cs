using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HSYErpBase.EntityDefinition;

namespace TtaCommonLibrary.Entities.UserModel
{
    [DataContract]
    public class UserLite:BasicEntity
    {
        [DataMember]
        public virtual String UserName { get; set; }
        [DataMember]
        public virtual String Avatar { get; set; }
        [DataMember]
        public virtual int Level { get; set; }
        [DataMember]
        public virtual int Rank { get; set; }
    }
}
