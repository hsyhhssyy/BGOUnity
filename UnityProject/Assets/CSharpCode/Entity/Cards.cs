using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Entity
{
    public class CardRowCard
    {
        /// <summary>
        /// 0-Frugality
        /// 1-Frederick Barbarossa
        /// 2-Napoleon Bonaparte
        /// 3-Impact of Architecture
        /// </summary>
        public String InternalId;
        /// <summary>
        /// Could be null(Means can't take)
        /// </summary>
        public String PostUrl;
        /// <summary>
        /// &lt;input type="hidden" name="idNote" value="68"&gt;
        /// </summary>
        public String IdNote;
        /// <summary>
        /// &lt;input type="hidden" name="idMsgChat" value=""&gt;
        /// </summary>
        public String IdMsgChat;
    }

    public class BuildingCard
    {
        public int Age;
        public int Name;

        public int Worker;
        public int Storage;

        //public int Cost;

    }
}
