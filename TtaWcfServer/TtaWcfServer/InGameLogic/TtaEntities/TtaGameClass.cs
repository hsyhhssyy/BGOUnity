using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.InGameLogic.TtaEntities
{
    [DataContract]
    public class TtaGame
    {
        [DataMember]
        public int Id;
        public GameRoom Room;
        [DataMember]
        public TtaPhase CurrentPhase;

        [DataMember]
        public String Version = "2.0";

        [DataMember]
        public Age CurrentAge;
        [DataMember]
        public int CurrentRound;


        [DataMember]
        public List<CardInfo> CurrentEventDeck;
        [DataMember]
        public List<CardInfo> FutureEventDeck;
        [DataMember]
        public List<CardInfo> PastEventDeck;


        [DataMember]
        public List<CardInfo> CivilCardsDeck;
        [DataMember]
        public List<CardInfo> MilitaryCardsDeck;


        [DataMember]
        public List<CardInfo> DiscardedMilitaryCardsDeck;



        [DataMember]
        public List<TtaBoard> Boards;


        [DataMember]
        public List<CardInfo> SharedTactics;


        [DataMember]
        public List<CardInfo> CardRow;
        
    }

    [DataContract]
    public class Warning
    {
        [DataMember]
        public WarningType Type;

        [DataMember]
        public String Data;
    }
    
}
