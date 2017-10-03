using System;
using System.Collections.Generic;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.InGameLogic.TtaEntities
{
    public class TtaGame
    {
        public int Id;
        public GameRoom Room;
        public TtaPhase CurrentPhase;

        public String Version = "2.0";

        public Age CurrentAge;
        public int CurrentRound;

        public List<CardInfo> CurrentEventDeck;
        public List<CardInfo> FutureEventDeck;
        public List<CardInfo> PastEventDeck;

        public List<CardInfo> CivilCardsDeck; 
        public List<CardInfo> MilitaryCardsDeck;

        public List<CardInfo> DiscardedMilitaryCardsDeck; 
        

        public List<TtaBoard> Boards;

        public List<CardInfo> SharedTactics; 

        public List<CardInfo> CardRow;
        
    }

    public class Warning
    {
        public WarningType Type;
        public String Data;
    }
    
}
