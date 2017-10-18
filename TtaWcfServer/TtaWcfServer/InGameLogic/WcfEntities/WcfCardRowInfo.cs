using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.InGameLogic.WcfEntities
{
    [DataContract]
    public class WcfCardRowInfo
    {
        [DataMember]
        public CardInfo Card;
        [DataMember]
        public int CivilCount;
        [DataMember]
        public bool AlreadyTaken;
        [DataMember]
        public bool CanPutBack;
    }
}