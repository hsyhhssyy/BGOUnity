using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoPageProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";
        
        public static IEnumerator GameLists(String phpSession,Action<List<BgoGame>> callback)
        {
            //http://boardgaming-online.com/index.php?cnt=2

            UnityWebRequest www = UnityWebRequest.Get(BgoBaseUrl+ "index.php?cnt=2");

            www.SetRequestHeader("Cookie", "PHPSESSID=" + phpSession);

            yield return www.Send();

            if (www.isError)
            {
                Debug.Log(www.error);

                yield break;
            }

            var responseText = www.downloadHandler.data;

            List<BgoGame> games=new List<BgoGame>();

            //Some logic to extract games

            if (callback != null)
            {
                callback(games);
            }
        }

        /// <summary>
        /// 打开homepage然后登陆，确认到玩家名称和session id才视为成功
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator HomePage(String username, String password,Action<String> callback)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("identifiant="+username+"&mot_de_passe="+password+"&souvenir=on")
            };

            UnityWebRequest www = UnityWebRequest.Post(BgoBaseUrl, formData);

            var asyncResult=www.Send();
            yield return asyncResult;

            if (www.isError)
            {
                Debug.Log(www.error);

                yield break;
            }

            var headers = www.GetResponseHeaders();
            var phpSessionId = headers["Set-Cookie"].Split(";".ToCharArray())[0].Split("=".ToCharArray())[1];

            //var responseText = www.downloadHandler.data;

            if (callback != null)
            {
                callback(phpSessionId);
            }
        }


        public static String MatchPage(String phpSession, String matchId,String nat)
        {
            return null;
        }

        public static String DiscardPile(String phpSession, String matchId)
        {
            return null; 
            
        }


    }
}
