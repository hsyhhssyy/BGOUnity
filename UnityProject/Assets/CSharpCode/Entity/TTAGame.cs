using System;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network
{
    public abstract class TtaGame
    {
        public String Name;

        public List<String> Players; 
        public List<TtaBoard> Boards;

        public List<Card> CardRow;
    }
}
