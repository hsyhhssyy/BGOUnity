using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.GameLogic.Effect;
using System.Linq;
using Assets.CSharpCode.GameLogic;

namespace Assets.CSharpCode.Entity
{
    public class TtaBoard
    {
        public TtaBoard()
        {
            Resource = new TtaResourceCounter(GameLogicManager.CurrentManager, this);
        }

        public String PlayerName;
        
        public List<CardInfo> CompletedWonders;

        public CardInfo ConstructingWonder;
        /// <summary>
        /// 表示完成的步数
        /// </summary>
        public int ConstructingWonderSteps;

        [XmlIgnore]
        public EffectPool EffectPool;

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
        
        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;

        public TtaResourceCounter Resource;
        
        public int InitialYellowMarkerCount;

        public int InitialBlueMarkerCount;

        //CalcuatedProperty
        /// <summary>
        /// 表示需要多少个不满工人才能镇压住暴动
        /// </summary>
        public int DisorderValue
        {
            get
            {
                //目前不满需求
                int faceRequired = 0;
                int yellowMarker = Resource[ResourceType.YellowMarker];
                if (yellowMarker <= 12)
                {
                    faceRequired = 8 - ((int) (yellowMarker/2));
                }else if (yellowMarker <= 16)
                {
                    faceRequired = 1;
                }else if (yellowMarker > 16)
                {
                    faceRequired = 0;
                }

                var discorderV= faceRequired-Resource[ResourceType.HappyFace];
                return discorderV < 0 ? 0 : discorderV;
            }
        }

        //---------Static
        public T AggregateOnBuildingCell<T>(T initial, Func<T, BuildingCell, T> aggregate)
        {
            return
                (from buildingPair in this.Buildings from cellPair in buildingPair.Value select cellPair.Value)
                    .Aggregate(initial, aggregate);
        }
    }

    public class BuildingCell
    {
        public CardInfo Card;

        public int Worker;
        public int Storage;

    }
}
