using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using UnityEngine;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoTestServer:IServerAdapter
    {
        public String File;

        public IEnumerator LogIn(String username, String password, Action<String> callback)
        {
            if (callback != null)
            {
                callback(null);
            }
            
            yield break;
        }

        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            if (callback != null)
            {
                callback(new List<TtaGame> {new BgoGame {GameId = "7279176", Nat = "1", Name = "2.5尝鲜 8"}});
            }

            yield break;
        }

        public IEnumerator RefreshBoard(TtaGame game, Action<String> callback)
        {
            var html = Resources.Load<TextAsset>(File); ;

            BgoPageProvider.FillGameBoard(html.text, game as BgoGame);

            if (callback != null)
            {
                callback(null);
            }

            yield break;
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action, Action<String> callback)
        {
            if (callback != null)
            {
                callback(null);
            }

            yield break;
        }

        public IEnumerator TakeInternalAction(TtaGame game, PlayerAction action, Action<List<PlayerAction>>  callback)
        {
            if (callback != null)
            {
                callback(new List<PlayerAction>());
            }

            yield break;
        }

        public IEnumerator CheckRankedMatch(Action<TtaGame> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator StartRanking(string queueName, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator StopRanking(string queueName, Action<bool> callback)
        {
            throw new NotImplementedException();
        }
    }
}
