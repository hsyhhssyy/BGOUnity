using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Actions;
using JetBrains.Annotations;

namespace Assets.CSharpCode.Network
{
    public interface IServerAdapter
    {
        ServerType ServerType { get; }

        IEnumerator LogIn(String username,String password, Action<String> callback);
        IEnumerator ListGames(Action<List<TtaGame>> callback);
        IEnumerator RefreshBoard(TtaGame game,Action<String> callback);

        IEnumerator TakeAction(TtaGame game, PlayerAction action, Action<ActionResponse> callback);
        IEnumerator TakeInternalAction(TtaGame game, PlayerAction action, Action<ActionResponse,List<PlayerAction>> callback);

        IEnumerator CheckRankedMatch(Action<TtaGame> callback);
        IEnumerator StartRanking(String queueName,Action<bool> callback);
        IEnumerator StopRanking(String queueName, Action<bool> callback);
    }

    public enum ServerType
    {
        PassiveServer2Sec, PassiveServer30Sec,ActiveServer

    }
}
