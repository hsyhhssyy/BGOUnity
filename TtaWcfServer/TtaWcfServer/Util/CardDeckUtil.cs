using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.Util
{
    public static class CardDeckUtil
    {
        public static List<T> Shuffle<T>(this List<T> item)
        {
            var cardsShuffled=new List<T>();
            cardsShuffled.AddRange(item);

            Random ran = new Random();
            for (int i = 0; i < cardsShuffled.Count; i++)
            {
                var index = ran.Next(0, cardsShuffled.Count - 1);
                if (index != i)
                {
                    var temp = cardsShuffled[i];
                    cardsShuffled[i] = cardsShuffled[index];
                    cardsShuffled[index] = temp;
                }
            }

            return cardsShuffled;
        }

        public static CardInfo DrawCard(this List<CardInfo> cards)
        {
            if (cards.Count == 0)
            {
                return null;
            }
            var card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        public static List<CardInfo> DrawOrShuffle(this List<CardInfo> cards,int amount,List<CardInfo> discardDeck)
        {
            if (cards.Count + discardDeck.Count < amount)
            {
                //出错，总张数不够
                return null;
            }

            List < CardInfo > drawCards=new List<CardInfo>();

            for (int index = 0; index < amount; index++)
            {
                var card = cards.DrawCard();
                if (card == null)
                {
                    cards.AddRange(discardDeck.Shuffle());
                    discardDeck.Clear();
                    card = cards.DrawCard();
                }
                drawCards.Add(card);
            }

            return drawCards;
        }

        public static CardInfo PeekCard(this List<CardInfo> cards)
        {
            if (cards.Count == 0)
            {
                return null;
            }
            var card = cards[0];
            return card;
        }
    }
}