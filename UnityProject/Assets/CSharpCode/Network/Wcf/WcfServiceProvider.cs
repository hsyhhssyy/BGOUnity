using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpScripts.Helper;
using UnityEngine;

namespace Assets.CSharpCode.Network.Wcf
{
    public static class WcfServiceProvider
    {
        public static String ServerUrlBase = "http://www.hsyhhssyy.net:8888/Service/";

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
    }
}
