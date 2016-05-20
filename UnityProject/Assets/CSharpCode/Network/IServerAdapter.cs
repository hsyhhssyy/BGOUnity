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
        IEnumerator LogIn(String username,String password, Action callback);
        IEnumerator ListGames(Action<List<TtaGame>> callback);
        IEnumerator RefreshBoard(TtaGame game,Action callback);

        IEnumerator TakeAction(TtaGame game, PlayerAction action, Action callback);
        IEnumerator TakeInternalAction(TtaGame game, PlayerAction action, Action<List<PlayerAction>> callback);

    }

}
