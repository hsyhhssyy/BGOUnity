using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoServer : IServerAdapter
    {
        private String phpSession="";
        private String identifiant = "";
        private String motDePasse = "";

        public IEnumerator LogIn(String username, String password, Action callback)
        {
            return BgoPageProvider.HomePage(username, password, (session,mot) =>
            {
                phpSession = session;
                identifiant = username;
                motDePasse = mot;
                if (callback != null)
                {
                    callback();
                }
            });
        }
        
        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            return BgoPageProvider.GameLists(phpSession, bgoGames =>
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

            return BgoPageProvider.RefreshBoard(phpSession, bgoGame, () =>
             {
                 if (callback != null)
                 {
                     callback();
                 }
             });
        }
    }
}
