using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network.Wcf.Entities
{
    public class WcfPlayerAction:PlayerAction
    {
        public String Guid;
        public Dictionary<int,JSONObject> JsonData;
    }
}
