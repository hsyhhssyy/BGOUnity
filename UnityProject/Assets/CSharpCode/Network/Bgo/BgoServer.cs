using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoServer : IServerAdapter
    {
        private String _phpSession="";
        private String _identifiant = "";
        private String _motDePasse = "";

        public IEnumerator LogIn(String username, String password, Action callback)
        {
            return BgoPageProvider.HomePage(username, password, (session,mot) =>
            {
                _phpSession = session;
                _identifiant = username;
                _motDePasse = mot;
                if (callback != null)
                {
                    callback();
                }
            });
        }
        
        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            return BgoPageProvider.GameLists(_phpSession, bgoGames =>
            {
                List<TtaGame> games=new List<TtaGame>();
                bgoGames.ForEach(game=>games.Add(game));
                if (callback != null)
                {
                    callback(games);
                }
            });
        }

        public IEnumerator RefreshBoard(TtaGame game, Action callback)
        {
            if (!(game is BgoGame))
            {
                return null;
            }

            var bgoGame = (BgoGame)game;

            return BgoPageProvider.RefreshBoard(_phpSession, bgoGame, () =>
             {
                 if (callback != null)
                 {
                     callback();
                 }
             });
        }
    }
}
