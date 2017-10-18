using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic;
using Assets.CSharpCode.GameLogic.Actions;
using Assets.CSharpCode.Network.Wcf.Entities;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpScripts.Helper;

namespace Assets.CSharpCode.Network.Wcf
{
    public class WcfServer: IServerAdapter
    {
        private String Session;
        private int Uid;

        public ServerType ServerType { get { return ServerType.PassiveServer2Sec; } }

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
                    list.Select<JSONObject,WcfGame>(WcfJsonPageProvider.ParseTtaGameWithRoomJson).Cast<TtaGame>().ToList();
                callback(games);
                yield break;
            }
        }
        

        public IEnumerator RefreshBoard(TtaGame game, Action<String> callback)
        {
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.RefreshBoard(Session,(WcfGame) game),
                WcfServiceProvider.CastCallback((json) =>
                {
                    if (json != null && json.IsNull == false)
                    {
                        WcfJsonPageProvider.ParseTtaGameWithGameJson((WcfGame) game, json);
                        callback(null);
                        return;
                    }
                    callback("NullReference");
                }));
        }

        public IEnumerator TakeAction(TtaGame game, PlayerAction action, Action<ActionResponse> callback)
        {
            WcfGame wcfGame= game as WcfGame;
            if (wcfGame == null)
            {
                callback(null);
                return new WcfServiceProvider.EmptyEnumrator();
            }

            var gameLogicResponse=GameLogicManager.CurrentManager.TakeAction(action);
            if (gameLogicResponse != null)
            {
                callback(gameLogicResponse);
                return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.TakeAction(Session, wcfGame.RoomId,action, gameLogicResponse),
                WcfServiceProvider.CastCallback((jsonPayload) =>
                {
                    var serverResponse=WcfJsonPageProvider.ParseActionResponse(jsonPayload);
                    var suppress=GameLogicManager.CurrentManager.ProcessServerCallback(gameLogicResponse, serverResponse);
                    if (!suppress)
                    {
                        callback(serverResponse);
                    }
                }));
            }
            //否则直接请求网络，并把传入的callback作为网络通信的结果
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.TakeAction(Session, wcfGame.RoomId, action, null),
                WcfServiceProvider.CastCallback((jsonPayload) =>
                {
                        var serverResponse = WcfJsonPageProvider.ParseActionResponse(jsonPayload);
                        callback(serverResponse);
                }));
        }

        public IEnumerator TakeInternalAction(TtaGame game, PlayerAction action,Action<ActionResponse,List<PlayerAction>> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator CheckRankedMatch(Action<TtaGame> callback)
        {
            return WcfServiceProvider.ExecuteWcfService(WcfServiceProvider.CheckRankedMatch(Session),
                WcfServiceProvider.CastCallback((game) =>
                {
                    callback(game != null&&game.IsNull==false ? WcfJsonPageProvider.ParseTtaGameWithRoomJson(game) : null);
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
