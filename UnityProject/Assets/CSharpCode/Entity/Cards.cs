using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.Entity
{
    public class Card
    {
        /// <summary>
        /// 0-Frugality
        /// 1-Frederick Barbarossa
        /// 2-Napoleon Bonaparte
        /// 3-Impact of Architecture
        /// </summary>
        public String InternalId;

    }

    public class BuildingCell
    {
        public CardInfo Card;

        public int Worker;
        public int Storage;
        
    }
}
