using System;
using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.Entity
{
    public abstract class TtaGame
    {
        public String Name;

        public Age CurrentAge;
        public int CurrentRound;

        public Age CurrentEventAge;
        public CardInfo CurrentEventCard;
        public String CurrentEventCount;

        public Age FutureEventAge;
        public String FutureEventCount;

        public int CivilCardsRemain;
        public int MilitaryCardsRemain;

        public int MyPlayerIndex = -1;

        public TtaPhase CurrentPhase;

        public List<TtaBoard> Boards;

        public List<CardInfo> SharedTactics; 

        public List<CardRowCardInfo> CardRow;

        public List<PlayerAction> PossibleActions;

        public String Version = "2.0";
    }

    public class Warning
    {
        public WarningType Type;
        public String Data;
    }

    public class CardRowCardInfo
    {
        public CardInfo Card;
        public bool CanPutBack;
        public bool CanTake;
        public int CivilActionCost;
        
    }
}
