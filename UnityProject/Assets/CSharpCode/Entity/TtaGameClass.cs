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

        public List<String> Players; 
        public List<TtaBoard> Boards;

        public List<CardRowCardInfo> CardRow;

        public List<PlayerAction> PossibleActions;
    }

    public class CardRowCardInfo
    {
        public CardInfo Card;
        public bool CanPutBack;
        public bool CanTake;
        public int CivilActionCost;
        
    }
}
