﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoServer : IServerAdapter
    {
        private BgoSessionObject sessionObject;

        public IEnumerator LogIn(String username, String password, Action callback)
        {
            return BgoPageProvider.HomePage(username, password, (session,mot) =>
            {
                sessionObject=new BgoSessionObject();
                sessionObject._phpSession = session;
                sessionObject._identifiant = username;
                sessionObject._motDePasse = mot;
                if (callback != null)
                {
                    callback();
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

        public IEnumerator RefreshBoard(TtaGame game, Action callback)
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
                     callback();
                 }
             });
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action,Action callback)
        {
            BgoGame bgoGame = game as BgoGame;
            BgoPlayerAction bgoAction = action as BgoPlayerAction;
            Action callbackDelegate = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };
            
            return BgoPostProvider.PostAction(sessionObject, bgoGame, bgoAction, callbackDelegate);
            
        }
    }
}
