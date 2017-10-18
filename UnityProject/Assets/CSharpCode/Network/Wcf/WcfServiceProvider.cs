using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network.Wcf.Entities;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpScripts.Helper;
using UnityEngine;
using Assets.CSharpCode.GameLogic.Actions;
using Assets.CSharpCode.Network.Wcf.Json;

namespace Assets.CSharpCode.Network.Wcf
{
    /// <summary>
    /// 本类用于指明网络服务的地址，接受原始数据，将其转化为Json打包并发送出去
    /// </summary>
    public static class WcfServiceProvider
    {
        public static String ServerUrlBase = "http://www.hsyhhssyy.net:8888/Service/";

        public static JsonSerializer Serializer=new JsonSerializer();

        static WcfServiceProvider()
        {
            Serializer.ReplaceAttributes.Add(typeof (CardRowCardInfo),
                new DataContractAttribute
                {
                    IsReference = true,
                    Name = "CardRowInfo",
                    Namespace = "TtaWcfServer.InGameLogic.TtaEntities"
                });
            Serializer.ReplaceAttributes.Add(typeof(CardInfo),
                new DataContractAttribute
                {
                    IsReference = true,
                    Name = "CardInfo",
                    Namespace = "TtaWcfServer.InGameLogic.Civilpedia"
                });
            Serializer.ReplaceAttributes.Add(typeof(KeyValuePair<CardInfo,int>),
                new DataContractAttribute
                {
                    IsReference = true,
                    Name = "KeyValuePairOfCardInfointc81yGVyJ",
                    Namespace = "System.Collections.Generic"
                });
        }

        public static IEnumerator Enumerate(params IEnumerator[] enums)
        {
            foreach (var enumerator in enums)
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            yield break;
        }

        public static Func<JSONObject, IEnumerator> CastCallback(Action<JSONObject> callback)
        {
            return (data) =>
            {
                //TODO 把执行移动到MoveNext上而不是创建时
                callback(data);
                return new EmptyEnumrator();
            };
        }

        public class EmptyEnumrator:IEnumerator
        {

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {

            }

            public object Current { get; private set; }
        }

        /// <summary>
        /// 执行一个IEnumerator的WcfService，转换成Json并拿出其中的Payload传递给Callback
        /// </summary>
        /// <param name="service"></param>
        /// <param name="callback">分析和处理结果的回调函数，接受JsonObject（Payload）作为从参数，返回IEnumerator提供异步能力</param>
        /// <returns></returns>
        public static IEnumerator ExecuteWcfService(IEnumerator service, Func<JSONObject, IEnumerator> callback)
        {
            //Service的第一步都是WWW
            service.MoveNext();
            if (service.Current is WWW)
            {
                yield return service.Current;
            }

            //Service的第二步是结果
            if (!service.MoveNext())
            {
                LogRecorder.Log("MoveNext false");
                var enumrator = callback(null);
                while (enumrator.MoveNext())
                {
                    yield return enumrator.Current;
                }
                yield break;
            }
            else
            {
                var obj = (JSONObject) service.Current;
                if (obj == null)
                {
                    var enumrator = callback(null);
                    while (enumrator.MoveNext())
                    {
                        yield return enumrator.Current;
                    }
                    yield break;
                }
                //结果一定叫XXXResult
                JSONObject payload = null;
                foreach (var key in obj.keys)
                {
                    if (key.EndsWith("Result"))
                    {
                        payload = obj.TryGetPath(key);
                    }
                }
                if (payload == null || payload.TryGetField("Error").i != 0)
                {
                    LogRecorder.Log("No Payload");
                    var enumrator = callback(null);
                    while (enumrator.MoveNext())
                    {
                        yield return enumrator.Current;
                    }
                    yield break;
                }
                else
                {
                    var result = payload.GetField("Payload");
                    var enumrator = callback(result);
                    while (enumrator.MoveNext())
                    {
                        yield return enumrator.Current;
                    }
                    yield break;
                }
            }
        }

        public static IEnumerator RequestSession()
        {
            return WcfPostOperator.PostJson(ServerUrlBase + "Login/UserLoginService.svc/GenerateSessionKey",new JSONObject());
        }

        public static IEnumerator Login(String session,String username,String sessionPasswordMd5)
        {
            JSONObject obj=new JSONObject();
            obj.SetField("session", session);
            obj.SetField("username", username);
            obj.SetField("sessionPasswordMd5", sessionPasswordMd5);
            return WcfPostOperator.PostJson(ServerUrlBase + "Login/UserLoginService.svc/Login", obj);
        }

        public static IEnumerator ListGames(String session)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            return WcfPostOperator.PostJson(ServerUrlBase + "LobbyService/LobbyMainService.svc/ListMyGame", obj);
        }

        public static IEnumerator CheckRankedMatch(String session)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            return WcfPostOperator.PostJson(ServerUrlBase + "LobbyService/LobbyMainService.svc/RankingStatus", obj);
        }

        public static IEnumerator StartRanking(String session)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            return WcfPostOperator.PostJson(ServerUrlBase + "LobbyService/LobbyMainService.svc/StartRanking", obj);
        }

        public static IEnumerator StopRanking(String session)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            return WcfPostOperator.PostJson(ServerUrlBase + "LobbyService/LobbyMainService.svc/StopRanking", obj);
        }

        //------------------
        public static IEnumerator RefreshBoard(String session,WcfGame game)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            obj.SetField("roomId", game.RoomId);
            return WcfPostOperator.PostJson(ServerUrlBase + "GameService/GameMainService.svc/QueryGameBoard", obj);
        }

        //------------------
        public static IEnumerator TakeAction(String session, int roomId,PlayerAction action,ActionResponse clientResponse)
        {
            JSONObject obj = new JSONObject();
            obj.SetField("sessionString", session);
            obj.SetField("roomId", roomId);
            obj.SetField("action", Serializer.Serialize(action));
            obj.SetField("data",(String)null);
            obj.SetField("clientResponse", Serializer.Serialize(clientResponse));
            return WcfPostOperator.PostJson(ServerUrlBase + "GameService/GameMainService.svc/PerformAction", obj);
        }

        public static JSONObject PackAction(PlayerAction action)
        {
            JSONObject obj = new JSONObject();
            JSONObject dataField = new JSONObject();
            foreach (var pair in action.Data)
            {
                PackDataItem(dataField,pair);
            }
            obj.AddField("Data",dataField);

            obj.AddField("PlayerActionType", (int)action.ActionType);
            obj.AddField("Internal", action.Internal);

            return obj;
        }

        public static void PackDataItem(JSONObject dataField, KeyValuePair<int, object> item)
        {

            JSONObject pairObject = new JSONObject();
            pairObject.AddField("Key", item.Key);
            pairObject.AddField("Value", Serializer.Serialize(item.Value));
            dataField.Add(pairObject);
        }
        
        public static JSONObject PackResponse(ActionResponse response)
        {
            if (response == null)
            {
                return null;
            }
            JSONObject obj = new JSONObject();
            JSONObject dataField = new JSONObject();
            foreach (var move in response.Changes)
            {
                JSONObject gamemove = new JSONObject();
                gamemove.AddField("Type", (int)move.Type);
                JSONObject changesDataField = new JSONObject();
                foreach (var movePair in move.Data)
                {
                    PackDataItem(changesDataField, movePair);
                }
                gamemove.AddField("Data", changesDataField);
                dataField.Add(gamemove);
            }
            obj.AddField("Changes", dataField);

            obj.AddField("Type", (int)response.Type);

            return obj;
        }
    }
}
