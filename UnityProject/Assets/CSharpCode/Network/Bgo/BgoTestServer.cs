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
        public IEnumerator LogIn(String username, String password, Action callback)
        {
            if (callback != null)
            {
                callback();
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

        public IEnumerator RefreshBoard(TtaGame game, Action callback)
        {
            var html = Resources.Load<TextAsset>("Test/TestPage1"); ;

            BgoPageProvider.FillGameBoard(html.text, game as BgoGame);

            if (callback != null)
            {
                callback();
            }

            yield break;
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action, Action callback)
        {
            if (callback != null)
            {
                callback();
            }

            yield break;
        }
    }
}
