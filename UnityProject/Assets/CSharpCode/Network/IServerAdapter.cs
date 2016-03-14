using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using JetBrains.Annotations;

namespace Assets.CSharpCode.Network
{
    public interface IServerAdapter
    {
        IEnumerator ListGames(Action<List<TtaGame>> callback);
        IEnumerator RefreshBoard(TtaGame game,Action callback);
    }
}
