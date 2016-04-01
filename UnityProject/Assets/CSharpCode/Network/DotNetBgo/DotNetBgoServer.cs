using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network.DotNetBgo
{
    public class DotNetBgoServer
    {
        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            yield break;
        }

        public IEnumerator RefreshBoard(TtaGame game, Action callback)
        {
            yield break;
        }
    }
}
