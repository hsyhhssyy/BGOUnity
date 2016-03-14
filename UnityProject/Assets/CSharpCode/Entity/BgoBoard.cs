using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Entity
{
    public class BgoBoard
    {
        public String PlayerName;

        private List<CardRowCard> CardRow;

        private List<String> CompletedWonders;

        private String ConstructingWonder;
        private List<String> ConstructingWonderSteps;

        public List<String> SpecialTechs;
        public List<String> Colonies;

        private int CivilActionMax;
        private int CivilActionAvailable;
        private int MilitaryActionMax;
        private int MilitaryActionAvailable;

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

        public List<List<BuildingCard>> Buildings;
    }
}
