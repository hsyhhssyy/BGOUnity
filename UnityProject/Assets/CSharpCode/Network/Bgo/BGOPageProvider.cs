using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Entity;
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

            WWWForm myPostData = new WWWForm();
            var headers = myPostData.headers;
            headers.Add("Cookie", "PHPSESSID=" + phpSession);

            WWW www = new WWW(BgoBaseUrl, null, headers);

            //UnityWebRequest www = UnityWebRequest.Get(BgoBaseUrl+ "index.php?cnt=2");

            //www.SetRequestHeader("Cookie", "PHPSESSID=" + phpSession);
            
            yield return www;

            if (www.error!=null)
            {
                Debug.Log(www.error);

                yield break;
            }

            var responseText = www.text;

            List<BgoGame> games=new List<BgoGame>();

            //Some logic to extract games
            var matches = BgoRegexpCollections.ListGamesInMyGamePage.Matches(responseText);

            foreach(Match match in matches)
            {
                BgoGame game=new BgoGame();
                game.GameId = match.Groups[1].Value;
                game.Nat = match.Groups[3].Value;
                
                game.Name = UTF8Decoder(match.Groups[4].Value);
                games.Add(game);

            }

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
        public static IEnumerator HomePage(String username, String password,Action<String,String> callback)
        {
            //首先请求一个phpsession id
            WWW www = new WWW(BgoBaseUrl);

            yield return www;

            if (www.error!=null)
            {
                Debug.Log(www.error);

                yield break;
            }

            var sessionIdResponseHeaders = www.responseHeaders;
            var phpSession  = sessionIdResponseHeaders["SET-COOKIE"].Split(";".ToCharArray())[0].Split("=".ToCharArray())[1];

            //然后登陆

            WWWForm myPostData = new WWWForm();
            myPostData.AddField("identifiant",username);
            myPostData.AddField("mot_de_passe", password);
            myPostData.AddField("souvenir", "on");
            var cookieHeaders = myPostData.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + phpSession);

            var data = Encoding.UTF8.GetString(myPostData.data);

            www = new WWW(BgoBaseUrl, myPostData.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
                Debug.Log(www.error);

                yield break;
            }

            var motDePasseResponseHeaders = www.responseHeaders;
            var motDePasse = motDePasseResponseHeaders["SET-COOKIE"].Split(";".ToCharArray())[0].Split("=".ToCharArray())[1];

            
            if (callback != null)
            {
                callback(phpSession,motDePasse);
            }
        }


        public static IEnumerator RefreshBoard(String phpSession, BgoGame game,Action callback)
        {
            WWWForm myPostData = new WWWForm();
            var cookieHeaders = myPostData.headers;
            cookieHeaders.Add("Cookie", "PHPSESSID=" + phpSession);

            var data = Encoding.UTF8.GetString(myPostData.data);

            var www = new WWW(BgoBaseUrl+ "index.php?cnt=202&pl="+game.GameId+"&nat="+game.Nat, null, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
                Debug.Log(www.error);

                yield break;
            }

            var html = www.text;

            FillGameBoard(html, game);

            if (callback != null)
            {
                callback();
            }
        }

        private static void FillGameBoard(String html,BgoGame game)
        {
            //分析用户面板
            //第一步：解出卡牌列
            var matches = BgoRegexpCollections.ExtractCardRow.Matches(html);

            game.CardRow=new List<Card>();

            foreach(Match mc in matches)
            {
                BgoCardRowCard card=new BgoCardRowCard();
                card.InternalId = ((int) Enum.Parse(typeof(Age), mc.Groups[4].Value)).ToString() +"-"+ mc.Groups[5].Value;
                card.IdNote = mc.Groups[3].Value;
                card.PostUrl = mc.Groups[2].Value;
                game.CardRow.Add(card);
            }
        }

        public static String DiscardPile(String phpSession, String matchId)
        {
            return null; 
            
        }

        private static String UTF8Decoder(String str)
        {
            //&#26494;&#26472;&#32769;&#24072;8:6&#21021;&#38899;&#32769;&#24072;
            Regex reg=new Regex(@"&#(\d*?);");

            return reg.Replace(str, m => ((char) int.Parse(m.Groups[1].Value)).ToString());
        }

    }
}
