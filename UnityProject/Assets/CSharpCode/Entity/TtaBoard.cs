using System;
using System.Collections.Generic;
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

        public List<String> Warnings;

        public List<CardInfo> CivilCards;

        public List<CardInfo> MilitaryCards;

        public List<CardInfo> CurrentEventPlayed;
        public List<CardInfo> FutureEventPlayed;
        
        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;

        public readonly Dictionary<ResourceType, int> Resource=new Dictionary<ResourceType, int>();
    }

    public class BuildingCell
    {
        public CardInfo Card;

        public int Worker;
        public int Storage;

    }
}
