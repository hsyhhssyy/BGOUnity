using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace Assets.CSharpCode.Network.Bgo
{
    public class BgoPageProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";
        
        private static TtaCivilopedia civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

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
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

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
                game.Version = "2.0";
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
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

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

            //var data = Encoding.UTF8.GetString(myPostData.data);

            www = new WWW(BgoBaseUrl, myPostData.data, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

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

            //var data = Encoding.UTF8.GetString(myPostData.data);

            var www = new WWW(BgoBaseUrl+ "index.php?cnt=202&pl="+game.GameId+"&nat="+game.Nat, null, cookieHeaders);

            yield return www;

            if (www.error != null)
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
            }

            var html = www.text;

           Assets.CSharpCode.UI.Util.LogRecorder.Log("Board page received");

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
        // ReSharper disable once FunctionComplexityOverflow
        internal static void FillGameBoard(String html,BgoGame game)
        {
            //分析用户面板

            game.PossibleActions=new List<PlayerAction>();

            //解出卡牌列
            var matches = BgoRegexpCollections.ExtractCardRow.Matches(html);

            game.CardRow=new List<CardRowCardInfo>();

            foreach(Match mc in matches)
            {
                var card = civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), mc.Groups[5].Value),
                    mc.Groups[6].Value);

                BgoCardRowCardInfo cardRowCardInfo = new BgoCardRowCardInfo
                {
                    Card = card,
                    CanPutBack = mc.Groups[4].Value.Contains("carteEnMain"),
                    CanTake = mc.Groups[2].Value!=""&& (!mc.Groups[4].Value.Contains("carteEnMain")),
                    CivilActionCost =
                        BgoRegexpCollections.ExtractGovenrmentAndActionPointsMissing.Matches(mc.Groups[8].Value).Count
                };

                //LogRecorder.Log(card.CardName + mc.Groups[2].Value);

                BgoPlayerAction pa = new BgoPlayerAction
                {
                    ActionType = PlayerActionType.TakeCardFromCardRow
                };
                pa.Data[0] = cardRowCardInfo;
                pa.Data[1] = game.CardRow.Count; //Card Row Pos
                pa.Data[2] = mc.Groups[3].Value;//idNote
                pa.Data[3] = mc.Groups[2].Value;//PostUrl

                //能拿能放回去，才能有Action
                if (cardRowCardInfo.CanPutBack || cardRowCardInfo.CanTake)
                {
                    game.PossibleActions.Add(pa);
                }

                game.CardRow.Add(cardRowCardInfo);
            }

            //当前事件
            var matchCurrentEvent = BgoRegexpCollections.ExtractCurrentEvent.Match(html);
            if (matchCurrentEvent.Groups[3].Value == "")
            {
                game.CurrentEventAge = (Age) Enum.Parse(typeof (Age), matchCurrentEvent.Groups[5].Value);
                game.CurrentEventCard =
                    civilopedia.GetCardInfoByName(game.CurrentEventAge, matchCurrentEvent.Groups[6].Value);
                game.CurrentEventCount = matchCurrentEvent.Groups[7].Value;
            }
            else
            {
                game.CurrentEventAge = (Age)Enum.Parse(typeof(Age), matchCurrentEvent.Groups[3].Value);
                game.CurrentEventCard = null;
                game.CurrentEventCount = matchCurrentEvent.Groups[7].Value;
            }

            //未来事件
            var matchFutureEvent = BgoRegexpCollections.ExtractFutureEvent.Match(html);
            if (matchFutureEvent.Groups[1].Value.Length > 4)
            {
                game.FutureEventAge = Age.A;
                game.FutureEventCount = "0";
            }
            else
            {
                game.FutureEventAge = (Age)Enum.Parse(typeof(Age), matchFutureEvent.Groups[1].Value);
                game.FutureEventCount = matchFutureEvent.Groups[2].Value; 
            }

            //卡牌剩余
            var matchCivilRemain = BgoRegexpCollections.ExtractCivilCardRemains.Match(html);
            game.CivilCardsRemain = Convert.ToInt32(matchCivilRemain.Groups[2].Value);
            var matchMilitaryRemain = BgoRegexpCollections.ExtractMilitryCardRemains.Match(html);
            game.MilitaryCardsRemain = Convert.ToInt32(matchMilitaryRemain.Groups[2].Value);

            //当前阶段
            var matchPhase = BgoRegexpCollections.ExtractGamePhase.Match(html);
            var phase = matchPhase.Groups[1].Value;
            switch (phase.Trim())
            {
                case "Political Phase":
                    game.CurrentPhase = TtaPhase.PoliticalPhase;
                    break;
                case "Action Phase":
                    game.CurrentPhase = TtaPhase.ActionPhase;
                    break;
                case "Event Resolution":
                    game.CurrentPhase = TtaPhase.EventResolution;
                    break;
                case "Discard Phase":
                    game.CurrentPhase = TtaPhase.DiscardPhase;
                    break;
                default:
                    game.CurrentPhase = TtaPhase.OtherPhase;
                    break;
            }

            //时代和回合
            var matchAgeAndRound = BgoRegexpCollections.ExtractAgeAndRound.Match(html);
            game.CurrentAge = (Age)Enum.Parse(typeof (Age), matchAgeAndRound.Groups[1].Value);
            game.CurrentRound = Convert.ToInt32(matchAgeAndRound.Groups[2].Value);

            //可抄袭阵型
            var sharedTacticsMatch = BgoRegexpCollections.ExtractSharedTactics.Match(html);
            game.SharedTactics=new List<CardInfo>();
            foreach (Match m in BgoRegexpCollections.ExtractSharedTacticsItem.Matches(sharedTacticsMatch.Groups[1].Value))
            {
                if (m.Groups[1].Value == "&nbsp;")
                {
                    continue;
                }
                game.SharedTactics.Add(civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), m.Groups[1].Value),
                    m.Groups[2].Value));
            }

            //拆开玩家面板

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

            #region 校准名字和资源

            ExtractPlayerNameAndResource(html, game);

            #endregion


            //可用行动
            var subDropdown = BgoRegexpCollections.ExtractSubDropDown("action").Match(html);
            if (subDropdown.Success)
            {
                matches = BgoRegexpCollections.ExtractActions.Matches(subDropdown.Groups[1].Value);
                foreach (Match mc in matches)
                {
                    BgoPlayerAction pa = new BgoPlayerAction {ActionType = PlayerActionType.Unknown};
                    pa.Data[0] = mc.Groups[2].Value;
                    pa.Data[1] = mc.Groups[1].Value;

                    game.PossibleActions.Add(pa);
                }

                Match mSubmitForm = BgoRegexpCollections.ExtractSubmitForm.Match(html);
                game.ActionForm = new Dictionary<string, string>();

                if (mSubmitForm.Success)
                {
                    game.ActionFormSubmitUrl = mSubmitForm.Groups[1].Value;
                    matches = BgoRegexpCollections.ExtractSubmitFormDetail.Matches(mSubmitForm.Groups[2].Value);
                    foreach (Match mc in matches)
                    {
                        if (mc.Groups[2].Value != "")
                        {
                            game.ActionForm[mc.Groups[2].Value] = "";
                        }
                        else
                        {
                            game.ActionForm[mc.Groups[4].Value] = mc.Groups[5].Value;
                        }
                    }
                }
            }

            BgoActionFormater.Format((BgoGame)game,html);
        }

        // ReSharper disable once FunctionComplexityOverflow
        private static void ExtractPlayerNameAndResource(string html, BgoGame game)
        {
            var myNameMatch= BgoRegexpCollections.ExtractMyName.Match(html);
            var myName = myNameMatch.Groups[1].Value.Replace("&nbsp;"," ");

            var matches = BgoRegexpCollections.ExtractPlayerNameAndResource.Matches(html);

            for (var i = 0; i < matches.Count; i++)
            {
                Match mc = matches[i];

                var board = game.Boards[i];

                board.PlayerName = mc.Groups[1].Value;

                if (myName.ToLower() == board.PlayerName.ToLower())
                {
                    game.MyPlayerIndex = i;
                }

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
                            int incr = cm.Groups[3].Value == "-" ? 0 : Convert.ToInt32(cm.Groups[3].Value);
                            board.Resource[ResourceType.Culture] = curr;
                            board.Resource[ResourceType.CultureIncrement] = incr;
                        }
                            break;
                        case "Science":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceSpecial.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int mcurr = cm.Groups[2].Value == "" ? 0 : Convert.ToInt32(cm.Groups[2].Value);
                            int incr = cm.Groups[4].Value == "-" ? 0 : Convert.ToInt32(cm.Groups[4].Value);
                            board.Resource[ResourceType.Science] = curr;
                            board.Resource[ResourceType.ScienceForMilitary] = mcurr;
                            board.Resource[ResourceType.ScienceIncrement] = incr;
                        }
                            break;
                        case "Puissance":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            board.Resource[ResourceType.MilitaryForce] = curr;
                        }
                            break;
                        case "Exploration":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = cm.Groups[1].Value == "" ? 0 : Convert.ToInt32(cm.Groups[1].Value);
                            board.Resource[ResourceType.Exploration] = curr;
                        }
                            break;
                        case "HF":
                        {
                            var cms = BgoRegexpCollections.ExtractPlayerNameAndResourceHappy.Matches(rmc.Groups[2].Value);
                            board.Resource[ResourceType.HappyFace] = cms.Count;
                            //cms = BgoRegexpCollections.ExtractPlayerNameAndResourceUnhappy.Matches(rmc.Groups[2].Value);
                            //board.ResourceQuantity[ResourceType.UnhappyFace] = cms.Count;
                        }
                            break;
                        case "Nourriture":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceNormal.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int incr = cm.Groups[3].Value == "-" ? 0 : Convert.ToInt32(cm.Groups[3].Value);
                            board.Resource[ResourceType.Food] = curr;
                            board.Resource[ResourceType.FoodIncrement] = incr;
                        }
                            break;
                        case "Ressources":
                        {
                            var cm = BgoRegexpCollections.ExtractPlayerNameAndResourceSpecial.Match(rmc.Groups[2].Value);
                            int curr = Convert.ToInt32(cm.Groups[1].Value);
                            int mcurr = cm.Groups[2].Value == "" ? 0 : Convert.ToInt32(cm.Groups[2].Value);
                            int incr = cm.Groups[4].Value == "" ? 0 : Convert.ToInt32(cm.Groups[4].Value);
                            board.Resource[ResourceType.Resource] = curr;
                            board.Resource[ResourceType.ResourceForMilitary] = mcurr;
                            board.Resource[ResourceType.ResourceIncrement] = incr;
                        }
                            break;

                        default:
                           Assets.CSharpCode.UI.Util.LogRecorder.Log("UnknownRMC:"+rmc.Groups[1].Value);
                            break;
                    }
                }
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        private static void FillPlayerBoard(TtaBoard board, String htmlShade)
        {
            #region 分析建筑面板

            ExtractBuildingBoard(board, htmlShade);

            #endregion

            //蓝点
            var blueMarkerHtml = BgoRegexpCollections.ExtractBlueMarker.Match(htmlShade).Groups[1].Value;
            board.Resource[ResourceType.BlueMarker] = BgoRegexpCollections.ExtractBlueMarkerCounter.Matches(blueMarkerHtml).Count;

            //黄点
            var yellowMarkerHtml = BgoRegexpCollections.ExtractYellowMarker.Match(htmlShade).Groups[1].Value;
            board.Resource[ResourceType.YellowMarker] = BgoRegexpCollections.ExtractYellowMarkerCounter.Matches(yellowMarkerHtml).Count;

            //红白点
            var matchCivilMilitaryActions = BgoRegexpCollections.ExtractGovenrmentAndActionPoints.Match(htmlShade);
            board.Government = civilopedia.GetCardInfoByName(matchCivilMilitaryActions.Groups[1].Value);

            int markerCount= BgoRegexpCollections.ExtractGovenrmentAndActionPointsCama.Matches(
                    matchCivilMilitaryActions.Groups[2].Value).Count;
            int missingCount =BgoRegexpCollections.ExtractGovenrmentAndActionPointsMissing.Matches(
                    matchCivilMilitaryActions.Groups[2].Value).Count;
            board.Resource[ResourceType.WhiteMarker] = markerCount;
            board.Resource[ResourceType.WhiteMarkerMax] = markerCount + missingCount;

            //board.Resource[ResourceType.ExtraWhiteMarker] = 0;
            //board.ResourceFluctuation[ResourceType.ExtraWhiteMarker] = 0;

            markerCount =
               BgoRegexpCollections.ExtractGovenrmentAndActionPointsCama.Matches(
                   matchCivilMilitaryActions.Groups[3].Value).Count;
            missingCount =
                BgoRegexpCollections.ExtractGovenrmentAndActionPointsMissing.Matches(
                    matchCivilMilitaryActions.Groups[3].Value).Count;
            board.Resource[ResourceType.RedMarker] = markerCount;
            board.Resource[ResourceType.RedMarkerMax] = markerCount + missingCount;

            //领袖
            var matchLeader = BgoRegexpCollections.ExtractLeader.Match(htmlShade);
            board.Leader = matchLeader.Success ? civilopedia.GetCardInfoByName(matchLeader.Groups[1].Value) : null;

            //闲置工人
            var matchWorkerPool = BgoRegexpCollections.ExtractWorkerPool.Match(htmlShade);
            board.Resource[ResourceType.WorkerPool] =
                BgoRegexpCollections.ExtractPlayerNameAndResourceHappy.Matches(matchWorkerPool.Groups[1].Value).Count
                +
                BgoRegexpCollections.ExtractPlayerNameAndResourceUnhappy.Matches(matchWorkerPool.Groups[1].Value).Count;

            //奇迹
            var matchWonder = BgoRegexpCollections.ExtractWonder.Match(htmlShade);
            board.CompletedWonders = new List<CardInfo>();
            foreach (Match m in BgoRegexpCollections.ExtractWondeName.Matches(matchWonder.Groups[1].Value))
            {
                if (m.Groups[3].Value != "")
                {
                    board.CompletedWonders.Add(civilopedia.GetCardInfoByName(m.Groups[3].Value));
                }
                else
                {
                    board.ConstructingWonder = civilopedia.GetCardInfoByName(m.Groups[5].Value);
                    board.ConstructingWonderSteps =new List<string>();
                    foreach (Match mBuild in BgoRegexpCollections.ExtractWondeBuildStatus.Matches(m.Groups[6].Value))
                    {
                        board.ConstructingWonderSteps.Add(mBuild.Groups[1].Value.Length > 4
                            ? "X"
                            : mBuild.Groups[1].Value);
                    }
                }
            }

            //殖民地
            var colonyMatch = BgoRegexpCollections.ExtractColonyBox.Match(htmlShade);
            board.Colonies = new List<CardInfo>();
            foreach (Match m in BgoRegexpCollections.FindCardInfoFromUnorderedList.Matches(colonyMatch.Groups[1].Value))
            {
                var card=civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), m.Groups[1].Value), m.Groups[2].Value);
                board.Colonies.Add(card);
            }

            //特殊科技
            var matchSpecialTech = BgoRegexpCollections.ExtractSpecialTech.Match(htmlShade);
            String tech=matchSpecialTech.Groups[1].Value;
            var matchesSpecialTech = BgoRegexpCollections.ExtractSpecialTechName.Matches(tech);
            board.SpecialTechs=new List<CardInfo>();
            foreach (Match match in matchesSpecialTech)
            {
                CardInfo info = civilopedia.GetCardInfoByName(match.Groups[1].Value);
                board.SpecialTechs.Add(info);
            }

            //手牌
            var matchHandCivilCard = BgoRegexpCollections.ExtractHandCivilCard.Match(htmlShade);
            board.CivilCards=new List<CardInfo>();
            foreach (Match m in BgoRegexpCollections.ExtractHandCardName.Matches(matchHandCivilCard.Groups[1].Value))
            {
                board.CivilCards.Add(civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), m.Groups[1].Value) , m.Groups[2].Value));
            }

            var matchHandMilitaryCard = BgoRegexpCollections.ExtractHandMilitaryCard.Match(htmlShade);
            board.MilitaryCards = new List<CardInfo>();
            foreach (Match m in BgoRegexpCollections.ExtractHandCardName.Matches(matchHandMilitaryCard.Groups[1].Value))
            {
                board.MilitaryCards.Add(civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), m.Groups[1].Value) ,m.Groups[2].Value));
            }

            //警告
            var warningMatch = BgoRegexpCollections.ExtractWarning.Match(htmlShade);
            board.Warnings=new List<Warning>();
            int corruptionValue = 0;
            foreach (Match m in BgoRegexpCollections.ExtractWarningItem.Matches(warningMatch.Groups[1].Value))
            {
                var playerColors = new[] { "Unknown", "Orange", "Purple", "Green", "Grey" };
                var warningStr = m.Groups[1].Value;
                if (warningStr.StartsWith("War"))
                {
                    //War over Territory against Purple
                    var warName = "WarOver" + warningStr.CutBetween("over ", " against");
                    WarningType t = (WarningType) Enum.Parse(typeof (WarningType), warName);
                    Warning w = new Warning
                    {
                        Type = t,
                        Data = Array.IndexOf(playerColors, warningStr.CutAfter("against ")).ToString()
                    };
                    board.Warnings.Add(w);
                }else if (warningStr.StartsWith("Corruption"))
                {
                    corruptionValue++;
                }
                else if (warningStr.StartsWith("Famine"))
                {
                    Warning w = new Warning
                    {
                        Type = WarningType.Famine,
                    };
                    board.Warnings.Add(w);
                }
                else if (warningStr.StartsWith("Civil disorder"))
                {
                    Warning w = new Warning
                    {
                        Type = WarningType.CivilDisorder,
                    };
                    board.Warnings.Add(w);
                }
            }
            if (corruptionValue > 0)
            {
                Warning w = new Warning
                {
                    Type = WarningType.Corruption,
                    Data = corruptionValue.ToString()
                };
                board.Warnings.Add(w);
            }

            //阵型
            var matchTactic = BgoRegexpCollections.ExtractTactics.Match(htmlShade);
            board.Tactic = matchTactic.Success
                ? civilopedia.GetCardInfoByName((Age) Enum.Parse(typeof (Age), matchTactic.Groups[1].Value),
                                          matchTactic.Groups[2].Value)
                : null;

            board.CurrentEventPlayed = new List<CardInfo>();
            board.FutureEventPlayed = new List<CardInfo>();

            //已打出的事件(未知内容）,success就表示有，那么就肯定没有已知内容
            var matchPlayedEvents = BgoRegexpCollections.ExtractPlayedEvents.Match(htmlShade);
            if (matchPlayedEvents.Success)
            {
                string eventStr = matchPlayedEvents.Groups[1].Value;
                var collection = board.CurrentEventPlayed;
                foreach (Match m in BgoRegexpCollections.ExtractPlayedEventsItem.Matches(eventStr))
                {
                    var ageStr = m.Groups[3].Value;
                    if (ageStr == "")
                    {
                        collection = board.FutureEventPlayed;
                        continue;
                    }
                    var unknownCard = CardInfo.UnknownMilitaryCard((Age) Enum.Parse(typeof (Age), ageStr));
                    collection.Add(unknownCard);
                }
            }else if (
                (matchPlayedEvents = BgoRegexpCollections.ExtractPlayedEventsVisible.Match(htmlShade)).Success)
                //已打出的事件(已知内容）,success就表示有
            {
                string currentEventStr = matchPlayedEvents.Groups[1].Value;
                string futureEventStr = matchPlayedEvents.Groups[2].Value;
                
                foreach (Match m in BgoRegexpCollections.FindCardInfoFromUnorderedList.Matches(currentEventStr))
                {
                    var card =
                        civilopedia.GetCardInfoByName((Age) Enum.Parse(typeof (Age), m.Groups[1].Value) ,m.Groups[2].Value);
                    board.CurrentEventPlayed.Add(card);
                }

                foreach (Match m in BgoRegexpCollections.FindCardInfoFromUnorderedList.Matches(futureEventStr))
                {
                    var card =
                        civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), m.Groups[1].Value) ,
                                                m.Groups[2].Value);
                    board.FutureEventPlayed.Add(card);
                }
            }
        }

        private static void ExtractBuildingBoard(TtaBoard board, string htmlShade)
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
                    cell.Card = civilopedia.GetCardInfoByName(buildingName);

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
