using System;
using System.Collections;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoServer : IServerAdapter
    {
        private BgoSessionObject sessionObject;

        public IEnumerator LogIn(String username, String password, Action<String> callback)
        {
            return BgoPageProvider.HomePage(username, password, (session,mot) =>
            {
                sessionObject=new BgoSessionObject();
                sessionObject._phpSession = session;
                sessionObject._identifiant = username;
                sessionObject._motDePasse = mot;
                if (callback != null)
                {
                    callback(null);
                }
            });
        }
        
        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            return BgoPageProvider.GameLists(sessionObject._phpSession, bgoGames =>
            {
                List<TtaGame> games=new List<TtaGame>();
                bgoGames.ForEach(game=>games.Add(game));
                if (callback != null)
                {
                    callback(games);
                }
            });
        }

        public IEnumerator RefreshBoard(TtaGame game, Action<String> callback)
        {
            if (!(game is BgoGame))
            {
                return null;
            }

            var bgoGame = (BgoGame)game;

            return BgoPageProvider.RefreshBoard(sessionObject._phpSession, bgoGame, () =>
             {
                 if (callback != null)
                 {
                     callback(null);
                 }
             });
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action,Action<String> callback)
        {
            BgoGame bgoGame = game as BgoGame;
            BgoPlayerAction bgoAction = action as BgoPlayerAction;
            Action callbackDelegate = () =>
            {
                if (callback != null)
                {
                    callback(null);
                }
            };

            if (bgoAction == null)
            {
                LogRecorder.Log("Null Action!");
                return null;
            }
            
            return BgoPostProvider.PostAction(sessionObject, bgoGame, bgoAction, callbackDelegate);
            
        }

        public IEnumerator TakeInternalAction(TtaGame game, PlayerAction action, Action<List<PlayerAction>>  callback)
        {
            BgoGame bgoGame = game as BgoGame;
            BgoPlayerAction bgoAction = action as BgoPlayerAction;
            Action<List<PlayerAction>> callbackDelegate = (actions) =>
            {
                if (callback != null)
                {
                    callback(actions);
                }
            };

            if (bgoAction == null)
            {
                LogRecorder.Log("Null Action!");
                return null;
            }

            return BgoPostProvider.PostInternalAction(sessionObject, bgoGame, bgoAction, callbackDelegate);

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
