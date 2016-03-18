using System;
using System.Collections.Generic;

namespace Assets.CSharpCode.Entity
{
    public class TtaBoard
    {
        public String PlayerName;

        public List<Card> CardRow;

        public List<String> CompletedWonders;

        public String ConstructingWonder;
        public List<String> ConstructingWonderSteps;

        public List<String> SpecialTechs;
        public List<String> Colonies;

        public int CivilActionMax;
        public int CivilActionAvailable;
        public int MilitaryActionMax;
        public int MilitaryActionAvailable;

        public String Government;
        public String Leader;

        public String Tactic;

        public List<String> Warnings;

        public List<String> CivilCards;

        public List<String> MiliatyCards;

        public List<String> CurrentEventPlayed;
        public List<String> FutureEventPlayed;

        public int BlueBank;
        public int YellowBank;

        public int HappyFace;

        public int HappyWorker;
        public int UnhappyWorker;

        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;

        public Dictionary<ResourceType, int> ResourceTotal;
        public Dictionary<ResourceType, int> ResourceIncrement;
    }
}
