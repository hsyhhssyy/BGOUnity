using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.Entity
{
    public abstract class TtaGame
    {
        /// <summary>
        /// 用于显示的游戏名称
        /// </summary>
        public String Name;


        public Age CurrentAge;
        public int CurrentRound;

        /// <summary>
        /// 从0开始的当前玩家号码
        /// </summary>
        public int CurrentPlayer;
        /// <summary>
        /// 客户端所代表的游戏玩家的号码
        /// </summary>
        public int MyPlayerIndex = -1;

        /// <summary>
        /// 当前马上要翻开的事件的Age
        /// </summary>
        public Age CurrentEventAge;
        /// <summary>
        /// 显示当前最上面的一张EventCard的内容（如果你是贞德）
        /// </summary>
        public CardInfo CurrentEventCard;
        /// <summary>
        /// 当前事件的时代分布（四个数字），用加号相连，比如0+1+2+3
        /// </summary>
        public String CurrentEventCount;

        /// <summary>
        /// 未来事件最上面一张的时代（最后放的那张的时代）
        /// </summary>
        public Age FutureEventAge;
        /// <summary>
        /// 未来事件的时代分布（四个数字），用加号相连，比如0+1+2+3
        /// </summary>
        public String FutureEventCount;

        /// <summary>
        /// 剩余的内政牌数量
        /// </summary>
        public int CivilCardsRemain;

        /// <summary>
        /// 剩余的军事牌数量
        /// </summary>
        public int MilitaryCardsRemain;
        
        /// <summary>
        /// 当前阶段
        /// </summary>
        public TtaPhase CurrentPhase;

        /// <summary>
        /// 
        /// </summary>
        public List<TtaBoard> Boards;

        public List<CardInfo> SharedTactics; 

        public List<CardRowInfo> CardRow;

        public List<PlayerAction> PossibleActions=new List<PlayerAction>();

        public String Version = "2.0";

        public List<GameJournalEntry> Journal;
    }

    [DataContract]
    public class HandCardInfo
    {
        public const int TurnUnknownPlayable = -1;
        public const int TurnUnknownIllegal = 200;
        [DataMember]
        public CardInfo Card;
        [DataMember]
        public int TurnTaken;

        public HandCardInfo()
        {
            
        }

        public HandCardInfo(CardInfo card, int turn)
        {
            Card = card;
            TurnTaken = turn;
        }
    }

    public class Warning
    {
        public WarningType Type;
        public String Data;
    }

    [DataContract]
    public class CardRowInfo
    {
        [DataMember]
        public CardInfo Card;
        public bool CanPutBack;
        public bool CanTake;
        public int CivilActionCost;
        
    }
}
