using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia
{
    public class CardInfo
    {
        /// <summary>
        /// 0-Frugality
        /// 1-Frederick Barbarossa
        /// 2-Napoleon Bonaparte
        /// 3-Impact of Architecture
        /// </summary>
        public String InternalId;

        public Age CardAge;
        public String CardName;
        public CardType CardType;

        //Arts
        public String SmallImage;

        //ServerData
        public Dictionary<String,Object> ServerData=new Dictionary<string, object>();



        public CardInfo Clone()
        {
            CardInfo clone = (CardInfo)MemberwiseClone();
            clone.ServerData = new Dictionary<string, object>();
            return clone;
        }
    }
}
