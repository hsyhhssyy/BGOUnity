using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.Entity
{
    public class TtaBoard
    {
        public String PlayerName;
        
        public List<CardInfo> CompletedWonders;

        public CardInfo ConstructingWonder;
        public List<String> ConstructingWonderSteps;

        public List<CardInfo> SpecialTechs;
        public List<CardInfo> Colonies;
        
        public CardInfo Government;
        public CardInfo Leader;

        public CardInfo Tactic;

        public List<Warning> Warnings;

        public List<CardInfo> CivilCards;

        public List<CardInfo> MilitaryCards;

        public List<CardInfo> CurrentEventPlayed;
        public List<CardInfo> FutureEventPlayed;
        
        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;

        public readonly Dictionary<ResourceType, int> Resource=new Dictionary<ResourceType, int>();

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
    }

    public class BuildingCell
    {
        public CardInfo Card;

        public int Worker;
        public int Storage;

    }
}
