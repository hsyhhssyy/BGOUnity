using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using UnityEngine;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoPostProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";
        
        public static IEnumerator PostAction(BgoSessionObject sessionObject, BgoGame game,BgoPlayerAction action,Action callback)
        {
            switch (action.ActionType)
            {
                //Special function
                case PlayerActionType.TakeCardFromCardRow:
                case PlayerActionType.PutBackCard:
                    return SendTakePutBackCardPostMessage(sessionObject, game, action.Data[3].ToString(),
                        action.Data[2].ToString(), callback);
                //----optvalue is [1]
                case PlayerActionType.ResetActionPhase:
                case PlayerActionType.PlayActionCard:
                case PlayerActionType.ElectLeader:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(), callback);
                //----optvalue is [2]
                case PlayerActionType.IncreasePopulation:
                case PlayerActionType.BuildBuilding:
                case PlayerActionType.Revolution:
                case PlayerActionType.DevelopTechCard:
                    return PerformAction(sessionObject, game, action.Data[2].ToString(), callback);
                //----optvalue is [3]
                case PlayerActionType.UpgradeBuilding:
                case PlayerActionType.BuildWonder:
                    return PerformAction(sessionObject, game, action.Data[3].ToString(), callback);
                //----has additional form
                case PlayerActionType.Destory:
                case PlayerActionType.Disband:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(),
                        new Dictionary<String, String>
                        {
                            {"unite", action.Data[2].ToString()}
                        },
                        callback);
                //----Unknown
                case PlayerActionType.Unknown:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(), callback);
                default:
                    return null;
            }
        }

        private static IEnumerator PerformAction(BgoSessionObject sessionObject, BgoGame game, String actionValue,Dictionary<String,String> overrideData, Action callback)
        {
            var postUrl = RemoveCharacterEntities(game.ActionFormSubmitUrl);

            WWWForm myPostData = new WWWForm();
            foreach (var pair in game.ActionForm)
            {
                if (pair.Key == "action")
                {
                    myPostData.AddField("action", actionValue);
                }
                else
                {
                    if (overrideData.ContainsKey(pair.Key))
                    {
                        myPostData.AddField(pair.Key, overrideData[pair.Key]);
                    }
                    else
                    {
                        myPostData.AddField(pair.Key, pair.Value);
                    }
                }
            }

            var cookieHeaders = myPostData.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + sessionObject._phpSession + "; identifiant=" + sessionObject._identifiant + "; mot_de_passe=" + sessionObject._motDePasse);

            //var data = Encoding.UTF8.GetString(myPostData.data);

            var www = new WWW(BgoBaseUrl + postUrl, myPostData.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
                Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
            }

            var responseText = www.text;

            BgoPageProvider.FillGameBoard(responseText, game);

            if (callback != null)
            {
                callback();
            }
        }

        private static IEnumerator PerformAction(BgoSessionObject sessionObject, BgoGame game, String actionValue, Action callback)
        {
            var postUrl = RemoveCharacterEntities(game.ActionFormSubmitUrl);

            WWWForm myPostData = new WWWForm();
            foreach (var pair in game.ActionForm)
            {
                if (pair.Key == "action")
                {
                    myPostData.AddField("action", actionValue);
                }
                else
                {
                    myPostData.AddField(pair.Key, pair.Value);
                }
            }

            var cookieHeaders = myPostData.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + sessionObject._phpSession + "; identifiant=" + sessionObject._identifiant + "; mot_de_passe=" + sessionObject._motDePasse);

            //var data = Encoding.UTF8.GetString(myPostData.data);

            var www = new WWW(BgoBaseUrl + postUrl, myPostData.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
            }

            var responseText = www.text;

            BgoPageProvider.FillGameBoard(responseText, game);

            if (callback != null)
            {
                callback();
            }
        }

        private static IEnumerator SendTakePutBackCardPostMessage(BgoSessionObject sessionObject, BgoGame game, String postUrl,String idNote,Action callback)
        {
            postUrl = RemoveCharacterEntities(postUrl);

            WWWForm myPostData = new WWWForm();
            myPostData.AddField("idNote", idNote);
            myPostData.AddField("idMsgChat", "");

            var cookieHeaders = myPostData.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + sessionObject._phpSession +"; identifiant=" + sessionObject._identifiant+"; mot_de_passe=" + sessionObject._motDePasse);

            //var data = Encoding.UTF8.GetString(myPostData.data);

            var www = new WWW(BgoBaseUrl+ postUrl, myPostData.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
            }

            var responseText = www.text;

            BgoPageProvider.FillGameBoard(responseText,game);

            if (callback != null)
            {
                callback();
            }
        }
        
        private static String RemoveCharacterEntities(String src)
        {
            src = src.Replace("&amp;", "&");
            src = src.Replace("&nbsp;", " ");

            return src;
        }
    }
}
