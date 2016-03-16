using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoCardRowCard : Card
    {
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

    public class BgoGame:TtaGame
    {
        public String GameId;
        public String Nat;

    }
}
