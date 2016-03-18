using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoPageProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";
        
        /// <summary>
        /// 获取正在进行的游戏列表，注意这里面也能取到最近完成的比赛
        /// </summary>
        /// <param name="phpSession"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据网页内容填充玩家面板
        /// </summary>
        /// <param name="phpSession"></param>
        /// <param name="game"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 填充面板的帮助方法
        /// </summary>
        /// <param name="html"></param>
        /// <param name="game"></param>
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

            //第二步，拆开玩家面板

            matches = BgoRegexpCollections.ExtractPlayerPlate.Matches(html);
            game.Boards=new List<TtaBoard>();

            int plateStart = -1;
            foreach (Match mc in matches)
            {
                if (plateStart != -1)
                {
                    String plate = html.Substring(plateStart, mc.Index - plateStart);

                    TtaBoard board = new TtaBoard();
                    game.Boards.Add(board);

                    FillPlayerBoard(board, plate);
                    
                }

                plateStart = mc.Index;
                

                if (mc.Groups[2].Value != "")
                {
                    break;
                }
            }

            #region 第三步，校准名字和资源

            ExtractPlayerNameAndResource(html, game);

            #endregion

        }

        // ReSharper disable once FunctionComplexityOverflow
        private static void ExtractPlayerNameAndResource(string html, BgoGame game)
        {
            var matches = BgoRegexpCollections.ExtractPlayerNameAndResource.Matches(html);

            for (var i = 0; i < matches.Count; i++)
            {
                Match mc = matches[i];

                var board = game.Boards[i];

                board.PlayerName = mc.Groups[1].Value;
                board.ResourceIncrement = new Dictionary<ResourceType, int>();
                board.ResourceTotal = new Dictionary<ResourceType, int>();

                var strCut = mc.Groups[0].Value;

                var resourceMatches = BgoRegexpCollections.ExtractPlayerNameAndResourceCutResourceOut.Matches(strCut);

                foreach (Match rmc in resourceMatches)
                {
                    switch (rmc.Groups[1].Value)
                    {
                        case "Culture":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int incr = cm.Groups[3].Value == "" ? 0 : Convert.ToInt32(cm.Groups[3].Value);
                            board.ResourceTotal[ResourceType.Culture] = curr;
                            board.ResourceIncrement[ResourceType.Culture] = incr;
                        }
                            break;
                        case "Science":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceSpecial.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int mcurr = cm.Groups[2].Value == "" ? 0 : Convert.ToInt32(cm.Groups[2].Value);
                            int incr = cm.Groups[4].Value == "" ? 0 : Convert.ToInt32(cm.Groups[4].Value);
                            board.ResourceTotal[ResourceType.Science] = curr;
                            board.ResourceTotal[ResourceType.ScienceForMilitary] = mcurr;
                            board.ResourceIncrement[ResourceType.Science] = incr;
                        }
                            break;
                        case "Puissance":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            board.ResourceTotal[ResourceType.MilitaryForce] = curr;
                        }
                            break;
                        case "Exploration":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = cm.Groups[1].Value == "" ? 0 : Convert.ToInt32(cm.Groups[1].Value);
                            board.ResourceTotal[ResourceType.Exploration] = curr;
                        }
                            break;
                        case "HF":
                        {
                            var cms = BgoRegexpCollections.ExtractPlayerNameAndResourceHappy.Matches(rmc.Groups[2].Value);
                            board.ResourceTotal[ResourceType.HappyFace] = cms.Count;
                            cms = BgoRegexpCollections.ExtractPlayerNameAndResourceUnhappy.Matches(rmc.Groups[2].Value);
                            board.ResourceTotal[ResourceType.UnhappyFace] = cms.Count;
                        }
                            break;
                        case "Nourriture":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int incr = cm.Groups[3].Value == "" ? 0 : Convert.ToInt32(cm.Groups[3].Value);
                            board.ResourceTotal[ResourceType.Food] = curr;
                            board.ResourceIncrement[ResourceType.Food] = incr;
                        }
                            break;
                        case "Ressources":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceSpecial.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int mcurr = cm.Groups[2].Value == "" ? 0 : Convert.ToInt32(cm.Groups[2].Value);
                            int incr = cm.Groups[4].Value == "" ? 0 : Convert.ToInt32(cm.Groups[4].Value);
                            board.ResourceTotal[ResourceType.Ore] = curr;
                            board.ResourceTotal[ResourceType.OreForMilitary] = mcurr;
                            board.ResourceIncrement[ResourceType.Ore] = incr;
                        }
                            break;

                        default:

                            break;
                    }
                }
            }
        }

        private static void FillPlayerBoard(TtaBoard board, String htmlShade)
        {
            TtaCivilopedia civilopedia = TtaCivilopedia.GetCivilopedia("2.0");
            #region 分析建筑面板

            ExtractBuildingBoard(board, htmlShade, civilopedia);

            #endregion
        }

        private static void ExtractBuildingBoard(TtaBoard board, string htmlShade, TtaCivilopedia civilopedia)
        {
            board.Buildings = new Dictionary<BuildingType, Dictionary<Age, BuildingCell>>();
            var titleMap = new List<BuildingType>();

            var buildingBoardTableHtml = BgoRegexpCollections.ExtractBuildingBoard.Match(htmlShade).Groups[1].Value;

            var mcsRow = BgoRegexpCollections.ExtractBuildingBoardRow.Matches(buildingBoardTableHtml);

            //分析表头
            var titleRowHtml = mcsRow[0].Groups[1].Value;
            foreach (Match mcTitle in BgoRegexpCollections.FindP.Matches(titleRowHtml))
            {
                BuildingType bType;
                switch (mcTitle.Groups[1].Value)
                {
                    case "Air Force":
                        bType = BuildingType.AirForce;
                        break;
                    case "&nbsp;":
                        bType = BuildingType.Unknown;
                        break;
                    default:
                        bType = (BuildingType) Enum.Parse(typeof (BuildingType), mcTitle.Groups[1].Value);
                        break;
                }

                if (bType != BuildingType.Unknown)
                {
                    titleMap.Add(bType);
                }
            }

            for (int rowIndex = 1; rowIndex < mcsRow.Count; rowIndex++)
            {
                //分析从第二行开始的内容
                var mcsColumn = BgoRegexpCollections.ExtractBuildingBoardCell.Matches(mcsRow[rowIndex].Groups[1].Value);

                //第一格是时代
                var strAge =
                    BgoRegexpCollections.FindP.Match(mcsColumn[0].Groups[1].Value).Groups[1].Value.Replace("Age&nbsp;",
                        "");
                var age = (Age) Enum.Parse(typeof (Age), strAge);

                //从第二格开始
                for (int colIndex = 1; colIndex < mcsColumn.Count; colIndex++)
                {
                    if (mcsColumn[colIndex].Groups[1].Value == "&nbsp;")
                    {
                        continue;
                    }

                    BuildingCell cell = new BuildingCell();

                    var cellHtml = mcsColumn[colIndex].Groups[1].Value;

                    var mcsNameAndCount = BgoRegexpCollections.FindP.Matches(cellHtml);
                    var buildingName = mcsNameAndCount[0].Groups[1].Value;
                    cell.Card = civilopedia.GetCardInfo(buildingName);

                    var workerCount = mcsNameAndCount[1].Groups[1].Value == "&nbsp;"
                        ? 0
                        : BgoRegexpCollections.ExtractBuildingBoardBuidingCount.Matches(mcsNameAndCount[1].Groups[1].Value).Count;
                    cell.Worker = workerCount;

                    var mcsResource = BgoRegexpCollections.ExtractBuildingBoardResourceCount.Matches(cellHtml);
                    cell.Storage = mcsResource.Count;

                    if (!board.Buildings.ContainsKey(titleMap[colIndex-1]))
                    {
                        board.Buildings[titleMap[colIndex-1]] = new Dictionary<Age, BuildingCell>();
                    }
                    board.Buildings[titleMap[colIndex-1]].Add(age, cell);
                }
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
