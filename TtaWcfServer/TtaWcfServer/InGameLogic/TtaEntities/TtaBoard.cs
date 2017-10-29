using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.Effects;
using TtaWcfServer.Util;
using System.Linq;

namespace TtaWcfServer.InGameLogic.TtaEntities
{

    public class TtaBoard
    {
        public TtaBoard()
        {
            //以下这些元素不能根据面板算出，因此需要保存
            UncountableResourceCount.Add(ResourceType.Science, 0);
            UncountableResourceCount.Add(ResourceType.Culture, 0);

            UncountableResourceCount.Add(ResourceType.ResourceForMilitary, 0);
            UncountableResourceCount.Add(ResourceType.ResourceForWonderAndProduction, 0);

            UncountableResourceCount.Add(ResourceType.ScienceForMilitary, 0);
            UncountableResourceCount.Add(ResourceType.ScienceForSpecialTech, 0);

            UncountableResourceCount.Add(ResourceType.WhiteMarker, 0);
            UncountableResourceCount.Add(ResourceType.RedMarker, 0);

            UncountableResourceCount.Add(ResourceType.WorkerPool, 0);
        }

        public int PlayerId;
        public List<CardInfo> CompletedWonders;
        public CardInfo ConstructingWonder;

        [XmlIgnore]
        public EffectPool EffectPool=new EffectPool();
        /// <summary>
        /// 表示当前奇迹建设到第几步了
        /// </summary>
        public int ConstructingWonderSteps;
        
        public List<CardInfo> SpecialTechs;
        public List<CardInfo> Colonies;
        
        public CardInfo Government;
        public CardInfo Leader;
        
        public CardInfo Tactic;
        
        public List<Warning> Warnings;
        
        public List<HandCardInfo> CivilCards;
        
        public List<HandCardInfo> MilitaryCards;
        
        public List<CardInfo> CurrentEventPlayed;
        public List<CardInfo> FutureEventPlayed;
        
        public XmlSerializableDictionary<BuildingType, XmlSerializableDictionary<Age, BuildingCell>> Buildings;
        
        public int InitialYellowMarkerCount;
        
        public int InitialBlueMarkerCount;
        
        public XmlSerializableDictionary<ResourceType, int> UncountableResourceCount=new XmlSerializableDictionary<ResourceType, int>();

        //---------Static
        public T AggregateOnBuildingCell<T>(T initial, Func<T,BuildingCell, T> aggregate)
        {
            return
                (from buildingPair in this.Buildings from cellPair in buildingPair.Value select cellPair.Value)
                    .Aggregate(initial, aggregate);
        }
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
