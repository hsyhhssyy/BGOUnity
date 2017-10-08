using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.InGameLogic.TtaEntities
{
    [DataContract]
    public class TtaBoard
    {
        [DataMember]
        public int PlayerId;
        [DataMember]
        public List<CardInfo> CompletedWonders;
        [DataMember]
        public CardInfo ConstructingWonder;
        [DataMember]
        public int ConstructingWonderSteps;

        [DataMember]
        public List<CardInfo> SpecialTechs;
        [DataMember]
        public List<CardInfo> Colonies;

        [DataMember]
        public CardInfo Government;
        [DataMember]
        public CardInfo Leader;

        [DataMember]
        public CardInfo Tactic;

        [DataMember]
        public List<Warning> Warnings;

        [DataMember]
        public List<CardInfo> CivilCards;

        [DataMember]
        public List<CardInfo> MilitaryCards;

        [DataMember]
        public List<CardInfo> CurrentEventPlayed;
        [DataMember]
        public List<CardInfo> FutureEventPlayed;

        [DataMember]
        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;

        /*
        /// <summary>
        /// 这里的每一个元素都可以通过面板算出来，所以这整个集合都是被动运算得到的，将来改为属性或方法
        /// </summary>
        [DataMember]
        public Dictionary<ResourceType, int> Resource=new Dictionary<ResourceType, int>();
        */

        [DataMember]
        public int InitialYellowMarkerCount;

        [DataMember]
        public int InitialBlueMarkerCount;
    }

    [DataContract]
    public class BuildingCell
    {
        [DataMember]
        public CardInfo Card;

        [DataMember]
        public int Worker;
        /// <summary>
        /// 这里的Storage指的是蓝点的个数，不是矿物的价值
        /// </summary>
        [DataMember]
        public int Storage;

    }
}
