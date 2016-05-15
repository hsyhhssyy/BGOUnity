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
                case PlayerActionType.TakeCardFromCardRow:
                case PlayerActionType.PutBackCard:
                    return SendTakePutBackCardPostMessage(sessionObject, game,action.Data[3].ToString(),action.Data[2].ToString(),callback);
                case PlayerActionType.IncreasePopulation:
                    return UnknownAction(sessionObject, game, action.Data[2].ToString(), callback);
                case PlayerActionType.BuildBuilding:
                    return UnknownAction(sessionObject, game, action.Data[2].ToString(), callback);
                    case PlayerActionType.UpgradeBuilding:
                    return UnknownAction(sessionObject, game, action.Data[3].ToString(), callback);
                    case PlayerActionType.Destory:
                        case PlayerActionType.Disband:
                    return UnknownAction(sessionObject, game, action.Data[1].ToString(),
                        new Dictionary<String, String>
                        {
                            {"unite", action.Data[2].ToString()}
                        },
                        callback);
                case PlayerActionType.Unknown:
                    return UnknownAction(sessionObject, game, action.Data[1].ToString(),  callback);
                default:
                    return null;
            }
        }

        private static IEnumerator UnknownAction(BgoSessionObject sessionObject, BgoGame game, String actionValue,Dictionary<String,String> overrideData, Action callback)
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

        private static IEnumerator UnknownAction(BgoSessionObject sessionObject, BgoGame game, String actionValue, Action callback)
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

        private static IEnumerator PostData(String url,WWWForm form, BgoSessionObject sessionObject)
        {
            var cookieHeaders = form.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + sessionObject._phpSession + "; identifiant=" + sessionObject._identifiant + "; mot_de_passe=" + sessionObject._motDePasse);

            var www = new WWW(url, form.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
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
