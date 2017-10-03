using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaWcfServer.InGameLogic.Civilpedia;

namespace TtaWcfServer.Util
{
    public static class GameUtil
    {
        public static List<T> Shuffle<T>(this List<T> item)
        {
            var cardsShuffled=new List<T>();
            cardsShuffled.AddRange(item);

            return cardsShuffled;
        }

        public static CardInfo DrawCard(this List<CardInfo> cards)
        {
            var card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}