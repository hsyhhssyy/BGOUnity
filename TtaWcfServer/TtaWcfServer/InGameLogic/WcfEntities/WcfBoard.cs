using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.WcfEntities
{
    [DataContract]
    public class WcfBoard
    {
        public WcfBoard(TtaGame game,int playerIndex,GameManager manager, bool observer)
        {
            SetupFullBoard(game, playerIndex, manager);
            if (observer) { ObserverMode();}
        }
        //设置裁判能看到的Board
        private void SetupFullBoard(TtaGame game, int playerIndex, GameManager manager)
        {
            TtaBoard board = game.Boards[playerIndex];
            PlayerName = game.Room.JoinedPlayer.FirstOrDefault(p=>p.Id==board.PlayerId)?.UserName??"Unknown Player";

            CompletedWonders = new List<CardInfo>();
            CompletedWonders.AddRange(board.CompletedWonders);

            ConstructingWonder = board.ConstructingWonder;
            ConstructingWonderSteps=new List<string>();
            if (ConstructingWonder != null)
            {
                for (int index = 0; index < ConstructingWonder.BuildCost.Count; index++)
                {
                    var cost = ConstructingWonder.BuildCost[index];
                    if (index < board.ConstructingWonderSteps)
                    {
                        ConstructingWonderSteps.Add("X");
                    }
                    else
                    {
                        ConstructingWonderSteps.Add(cost.ToString());
                    }
                }
            }

            SpecialTechs = new List<CardInfo>();
            SpecialTechs.AddRange(board.SpecialTechs);
            
            Colonies = new List<CardInfo>();
            Colonies.AddRange(board.Colonies);

            Government = board.Government;
            Leader = board.Leader;

            Tactic = board.Tactic;
            Warnings = board.Warnings;
            
            CivilCards = new List<HandCardInfo>();
            CivilCards.AddRange(board.CivilCards);
            
            MilitaryCards = new List<HandCardInfo>();
            MilitaryCards.AddRange(board.MilitaryCards);

            CurrentEventPlayed = new List<CardInfo>();
            CurrentEventPlayed.AddRange(board.CurrentEventPlayed);

            FutureEventPlayed = new List<CardInfo>();
            FutureEventPlayed.AddRange(board.FutureEventPlayed);

            InitialBlueMarkerCount = board.InitialBlueMarkerCount;
            InitialYellowMarkerCount = board.InitialYellowMarkerCount;

            //buildings
            Buildings =new Dictionary<BuildingType, Dictionary<Age, BuildingCell>>();
            foreach (var ttabuilding in board.Buildings)
            {
                var wcfbuilding=new Dictionary<Age, BuildingCell>();
                
                foreach (var cellPair in ttabuilding.Value)
                {
                    wcfbuilding.Add(cellPair.Key,cellPair.Value);
                }

                Buildings.Add(ttabuilding.Key,wcfbuilding);
            }

            var values = Enum.GetValues(typeof (ResourceType));
            foreach (var value in values)
            {
                Resource[(ResourceType) value] = manager.ReturnResourceCount((ResourceType) value, playerIndex);
            }
        }

        //在RefereeBoard的基础上隐藏观战者/选手不能看到的内容
        private void ObserverMode()
        {
            var cards = new List<HandCardInfo>();
            cards.AddRange(MilitaryCards);

            MilitaryCards=new List<HandCardInfo>();
            foreach (var cardInfo in cards)
            {
                MilitaryCards.Add(new HandCardInfo(CardInfo.UnknownMilitaryCard(cardInfo.Card.CardAge),
                    cardInfo.TurnTaken));
            }
        }


        #region 要传递过去的内容

        [DataMember]public String PlayerName;
        
        [DataMember]
        public List<CardInfo> CompletedWonders;

        [DataMember]
        public CardInfo ConstructingWonder;
        [DataMember]
        public List<String> ConstructingWonderSteps;

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
        public List<HandCardInfo> CivilCards;

        [DataMember]
        public List<HandCardInfo> MilitaryCards;

        [DataMember]
        public List<CardInfo> CurrentEventPlayed;
        [DataMember]
        public List<CardInfo> FutureEventPlayed;
        [DataMember]
        public int InitialYellowMarkerCount;
        [DataMember]
        public int InitialBlueMarkerCount;

        [DataMember]
        public Dictionary<BuildingType, Dictionary<Age, BuildingCell>> Buildings;
        
        [DataMember]
        public readonly Dictionary<ResourceType, int> Resource = new Dictionary<ResourceType, int>();
        
        #endregion
    }
}