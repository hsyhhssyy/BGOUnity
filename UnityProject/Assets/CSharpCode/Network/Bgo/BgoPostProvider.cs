﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network.Bgo.DataStructure;
using UnityEngine;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoPostProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";

        public static IEnumerator PostAction(BgoSessionObject sessionObject, BgoGame game, BgoPlayerAction action,
            Action callback)
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
                case PlayerActionType.PassPoliticalPhase:
                case PlayerActionType.PlayActionCard:
                case PlayerActionType.ElectLeader:
                case PlayerActionType.SetupTactic:
                case PlayerActionType.AdoptTactic:
                case PlayerActionType.PlayColony:
                case PlayerActionType.PlayEvent:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(), callback);
                //----optvalue is [2]
                case PlayerActionType.IncreasePopulation:
                case PlayerActionType.BuildBuilding:
                case PlayerActionType.Revolution:
                case PlayerActionType.ColonizeBid:
                case PlayerActionType.ResolveEventOption:
                case PlayerActionType.DevelopTechCard:
                    return PerformAction(sessionObject, game, action.Data[2].ToString(), AttachIdCarte(3, action),
                        callback);
                //----optvalue is [3]
                case PlayerActionType.UpgradeBuilding:
                    return PerformAction(sessionObject, game, action.Data[3].ToString(), AttachIdCarte(4, action), callback);
                //----optvalue is [4]
                case PlayerActionType.BuildWonder:
                    return PerformAction(sessionObject, game, action.Data[4].ToString(), AttachIdCarte(4, action), callback);
                //----has additional form
                case PlayerActionType.Destory:
                case PlayerActionType.Disband:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(),
                        new Dictionary<String, String>
                        {
                            {"unite", action.Data[2].ToString()}
                        },
                        callback);
                case PlayerActionType.DeclareWar:
                case PlayerActionType.Aggression:
                    return PerformAction(sessionObject, game, action.Data[3].ToString(),
                        new Dictionary<String, String>
                        {
                            {action.Data[4].ToString(), action.Data[5].ToString()}
                        },
                        callback);
                case PlayerActionType.EndActionPhase:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(),
                        new Dictionary<String, String>
                        {
                            {"confirmEndTurn", "on"}
                        },
                        callback);
                //---has additional method
                case PlayerActionType.SendColonists:
                    return SendSendColonistsPostMessage(sessionObject, game, action, callback);
                //----Unknown
                case PlayerActionType.Unknown:
                    return PerformAction(sessionObject, game, action.Data[1].ToString(), callback);
                default:
                    return null;
            }
        }



        public static IEnumerator PostInternalAction(BgoSessionObject sessionObject, BgoGame game, BgoPlayerAction action,
             Action<List<PlayerAction>> callback)
        {
            switch (action.ActionType)
            {
                case PlayerActionType.PlayActionCard:
                    return Perform2StepAction(sessionObject, game, action.Data[1].ToString(), callback);
                case PlayerActionType.BuildWonder:
                    return Perform2StepAction(sessionObject, game, action.Data[4].ToString(), callback);
                    
                default:
                    return null;
            }
        }

        private static Dictionary<String, String> AttachIdCarte(int pos, BgoPlayerAction action, Dictionary<String, String> otherData = null)
        {
            if (otherData == null)
            {
                otherData=new Dictionary<string, string>();
            }

            if (action.Data.Count > pos)
            {
                otherData.Add("idCarte", action.Data[pos].ToString());
            }

            return otherData;
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
                    if (overrideData!=null&&overrideData.ContainsKey(pair.Key))
                    {
                        myPostData.AddField(pair.Key, overrideData[pair.Key]);
                        overrideData.Remove(pair.Key);
                    }
                    else
                    {
                        myPostData.AddField(pair.Key, pair.Value);
                    }
                }
            }
            if (overrideData != null)
            {
                foreach (var pair in overrideData)
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

        private static IEnumerator Perform2StepAction(BgoSessionObject sessionObject, BgoGame game, String actionValue,
            Action<List<PlayerAction>> callback)
        {
            var postUrl = RemoveCharacterEntities(game.ActionFormSubmitUrl);

            WWWForm myPostData = new WWWForm();

            var actionSp = actionValue.Split(";".ToCharArray());
            var idCarte = "0";
            foreach (var pair in game.ActionForm)
            {
                if (pair.Key == "action")
                {
                    myPostData.AddField("action", actionValue);
                }
                else if (pair.Key == "idCarteAction")
                {
                    if (actionSp.Length > 1 && actionSp[0] == "12")
                    {
                        idCarte = actionSp[1];
                        myPostData.AddField("idCarteAction", actionSp[1]);
                    }
                    else
                    {
                        myPostData.AddField("idCarteAction", pair.Value);
                    }
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

            //Step1 Complete

            var labelMatches = BgoRegexpCollections.ExtractActionChoice.Matches(responseText);
            if (labelMatches.Count <= 0)
            {
                callback(new List<PlayerAction>());
                yield break;
            }
            var actions = new List<PlayerAction>();
            foreach (Match m in labelMatches)
            {
                var msg = m.Groups[2].Value;
                var optValue = m.Groups[1].Value;

                BgoPlayerAction pa = new BgoPlayerAction { ActionType = PlayerActionType.Unknown };
                pa.Data[0] = msg;
                pa.Data[1] = optValue;
                pa.Data[2] = idCarte;
                actions.Add(pa);
            }
            BgoActionFormater.FormatInternalAction(actions,responseText);

            if (callback != null)
            {
                callback(actions);
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

        private static IEnumerator SendSendColonistsPostMessage(BgoSessionObject sessionObject, BgoGame game,
            BgoPlayerAction action, Action callback)
        {
            var form = new Dictionary<String, String>
            {
                {"confirmEndTurn", "on"}
            };

            var tupleList=new List<BgoAdditionalFormTuple>();
            tupleList.AddRange(action.Data[5] as List<BgoAdditionalFormTuple>);

            foreach (var card in action.Data[2] as List<CardInfo>)
            {
                BgoAdditionalFormTuple theTuple=null;
                foreach (var tuple in tupleList)
                {
                    if (tuple.card == card)
                    {
                        theTuple = tuple;
                        break;
                    }
                }
                if (theTuple == null)
                {
                    throw new InvalidOperationException("找不到合适的用于牺牲的部队");
                }

                tupleList.Remove(theTuple);
                form.Add(theTuple.formName, ToCharacterEntities(theTuple.formValue));
            }

            return PerformAction(sessionObject, game, action.Data[1].ToString(),
                        form,
                        callback);
        }

        private static String RemoveCharacterEntities(String src)
        {
            src = src.Replace("&amp;", "&");
            src = src.Replace("&nbsp;", " ");

            return src;
        }

        private static String ToCharacterEntities(String src)
        {
            src = WWW.EscapeURL(src);

            return src;
        }
    }
}
