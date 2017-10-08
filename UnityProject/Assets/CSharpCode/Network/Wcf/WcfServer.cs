using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpScripts.Helper;

namespace Assets.CSharpCode.Network.Wcf
{
    public class WcfServer: IServerAdapter
    {
        private String Session;
        private int Uid;

        public IEnumerator LogIn(string username, string password, Action<String> callback)
        {
            var enumrator=WcfServiceProvider.RequestSession();
            enumrator.MoveNext();
            yield return enumrator.Current;

            if (!enumrator.MoveNext())
            {
                yield break;
            }

            JSONObject obj=(JSONObject)enumrator.Current;

            Session = obj.GetField("GenerateSessionKeyResult").str;
            var md5Crpt = MD5.Create();
            String passwordMd5 =
                BitConverter.ToString(md5Crpt.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToUpper();
            String sessionPasswordMd5 =
                BitConverter.ToString(md5Crpt.ComputeHash(Encoding.UTF8.GetBytes(Session+ passwordMd5))).Replace("-", "").ToUpper();

            enumrator = WcfServiceProvider.Login(Session, username, sessionPasswordMd5);
            enumrator.MoveNext();
            yield return enumrator.Current;

            if (!enumrator.MoveNext())
            {
                callback("UnknownError");
                yield break;
            }

            obj=(JSONObject)enumrator.Current;
            if (obj.TryGetPath("LoginResult").GetField("Error").i != 0)
            {
                callback(obj.TryGetPath("LoginResult").GetField("Message").str);
                yield break;
            }
            else
            {
                Uid = (int) obj.TryGetPath("LoginResult").GetField("Payload").i;
                callback(null);
                yield break;
            }
        }

        public IEnumerator ListGames(Action<List<TtaGame>> callback)
        {
            LogRecorder.Log("ListGames:" + Session);
            var enumrator = WcfServiceProvider.ListGames(Session);
            enumrator.MoveNext();
            yield return enumrator.Current;

            if (!enumrator.MoveNext())
            {
                callback(null);
                yield break;
            }

            var obj = (JSONObject)enumrator.Current;
            if (obj.TryGetPath("ListMyGameResult").GetField("Error").i != 0)
            {
                callback(null);
                yield break;
            }
            else
            {
                var payloadJson = obj.TryGetPath("ListMyGameResult").GetField("Payload");
                List<TtaGame> games= payloadJson.
                    list.Select(o => WcfJsonPageProvider.CreateRoom(o)).ToList();
                callback(games);
                yield break;
            }
        }
        

        public IEnumerator RefreshBoard(TtaGame game, Action<String> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action, Action<String> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator TakeInternalAction(TtaGame game, PlayerAction action, Action<List<PlayerAction>> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator CheckRankedMatch(Action<TtaGame> callback)
        {
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.CheckRankedMatch(Session),
                WcfServiceProvider.CastCallback((game) =>
                {
                    callback(game != null&&game.IsNull==false ? WcfJsonPageProvider.CreateRoom(game) : null);
                }));
        }

        public IEnumerator StartRanking(string queueName, Action<bool> callback)
        {
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.StartRanking(Session),
                WcfServiceProvider.CastCallback((game) =>
                {
                    callback(game);
                }));
        }

        public IEnumerator StopRanking(string queueName, Action<bool> callback)
        {
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.StopRanking(Session),
                WcfServiceProvider.CastCallback((game) =>
                {
                    callback(game);
                }));
        }
    }
}
