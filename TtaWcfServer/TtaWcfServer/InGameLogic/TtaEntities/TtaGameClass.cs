using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.InGameLogic.TtaEntities
{
    public class TtaGame
    {
        public int Id;
        
        [XmlIgnore]
        public GameRoom Room;

        public TtaPhase CurrentPhase;
        public int CurrentPlayer=0;
        
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
        
        public List<CardRowInfo> CardRow;

    }

    [DataContract]
    public class CardRowInfo
    {
        [DataMember]
        public CardInfo Card;
        /// <summary>
        /// [已过时]表示能否放回，请用拿走玩家是否是当前玩家，以及该卡是否在手牌中/奇迹面板来计算得出
        /// </summary>
        [Obsolete]
        public bool CanPutBack;
        /// <summary>
        /// 这张卡被哪位玩家拿走了，-1表示未拿走
        /// </summary>
        public int TakenBy;
        
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
