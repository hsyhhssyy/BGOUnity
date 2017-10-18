using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook.RuleBooks;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Logging;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.Civilpedia
{
    public class TtaCivilopedia
    {
        private static Dictionary<String, TtaCivilopedia> Civilopedias;

        public static TtaCivilopedia GetCivilopedia(String gameVersion)
        {
            LoadCivilopedias();

            return Civilopedias[gameVersion];
        }

        public static void LoadCivilopedias()
        {
            if (Civilopedias != null)
            {
                return;
            }

            Civilopedias = new Dictionary<string, TtaCivilopedia>();

            //CivilopediaList
            GameVersion ver=new GameVersion();
            ver.Name = "Original-TTA2.0";
            if (!Directory.Exists("C:/TtaUploads/"))
            {
                Directory.CreateDirectory("C:/TtaUploads/");
            }
            ver.CivilopediaPath = "C:/TtaUploads/Civilopedia/Original-TTA2.0/Civilopedia.csv";
            ver.RuleBookPath= "C:/TtaUploads/Civilopedia/Original-TTA2.0/RuleBook.cs";
            LoadCivilopedia(ver);
        }

        private static void LoadCivilopedia(GameVersion gameVersion)
        {
            var civilopedia = new TtaCivilopedia();
            civilopedia._cardInfos = new Dictionary<string, CardInfo>();
            civilopedia._ruleBook = new OriginalTta0200();

            StreamReader sr=new StreamReader(new FileStream(gameVersion.CivilopediaPath, FileMode.Open));
            string dictStr = sr.ReadToEnd();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var csvRow = CsvUtil.SplitRow(row.Trim());
                if (csvRow.Count != 20)
                {
                    continue;
                }

                try
                {
                    CardInfo info = new CardInfo
                    {
                        InternalId = csvRow[0],
                        CardName = csvRow[1].Trim(),
                        CardType = (CardType)Convert.ToInt32(csvRow[2]),
                        CardAge = (Age)Convert.ToInt32(csvRow[3]),
                        Description = csvRow[4],
                        FromSerialization = false,
                    };

                    //csv5,6,7是三个image，客户端才需要，服务器不需要也没有这些资源
                    
                    info.Package = csvRow[7];

                    info.ResearchCost = ToIntList(csvRow[8], "/");
                    info.BuildCost = ToIntList(csvRow[9], ",");
                    info.RedMarkerCost = ToIntList(csvRow[10], "|");

                    info.ImmediateEffects = new List<CardEffect>();
                    info.ImmediateEffects.AddRange(CreateEffects(csvRow[11])); //使用效果（ActionCard专用）
                    info.ImmediateEffects.AddRange(CreateEffects(csvRow[12])); //一次性效果（ColonyCard专用）

                    info.SustainedEffects = CreateEffects(csvRow[13]); //持续效果

                    info.AffectedTrget = ToIntList(csvRow[14], ","); //6月6日新加：影响对象

                    info.WinnerEffects = CreateEffects(csvRow[15]);
                    info.LoserEffects = CreateEffects(csvRow[16]);

                    info.TacticComposition = ToIntList(csvRow[17], ",");
                    info.TacticValue = string.IsNullOrEmpty(csvRow[18])
                        ? new List<int>()
                        : csvRow[18].Split("/".ToCharArray())
                            .Select(a => Convert.ToInt32(a.Split(",".ToCharArray())[2]))
                            .ToList();

                    info.ImmediateEffects.AddRange(CreateEffects(csvRow[19])); //领袖技能主动使用效果（LeaderCard专用）

                    civilopedia._cardInfos[info.InternalId] = info;
                }
                catch (Exception e)
                {
                    LogRecorder.Log(e.Message + " " + row);
                }
            }

            Civilopedias.Add(gameVersion.Name, civilopedia);
        }


        private static List<int> ToIntList(String str, String spliter)
        {
            return string.IsNullOrEmpty(str) ? new List<int>() : str.Split(spliter.ToCharArray()).Select(a => Convert.ToInt32(a)).ToList();
        }

        private static List<CardEffect> CreateEffects(String str)
        {
            str = str.Trim();
            if (string.IsNullOrEmpty(str))
            {
                return new List<CardEffect>();
            }
            List<CardEffect> result = new List<CardEffect>();
            var splites = str.Split("|".ToCharArray());
            foreach (var s in splites)
            {
                if (s.Contains("/"))
                {
                    ChooseOneCardEffect che = new ChooseOneCardEffect();
                    var orSplite = s.Split("/".ToCharArray());
                    foreach (var sOr in orSplite)
                    {
                        var e = CreateEffect(sOr);
                        che.Candidate.Add(e);
                    }
                    result.Add(che);
                }
                else
                {
                    result.Add(CreateEffect(s));
                }
            }

            return result;
        }

        private static CardEffect CreateEffect(String str)
        {
            var s = str.Split(",".ToCharArray());
            int id = Convert.ToInt32(s[0]);

            CardEffect e = new CardEffect { FunctionId = (CardEffectType)id };
            for (int i = 1; i < s.Length; i++)
            {
                e.Data.Add(Convert.ToInt32(s[i]));
            }

            return e;
        }

        //------------------------------

        private Dictionary<String, CardInfo> _cardInfos;
        private TtaRuleBook _ruleBook;

        public List<CardInfo> GetAllCards()
        {
            return _cardInfos.Values.ToList();
        }

        public CardInfo GetCardInfoById(String internalId)
        {
            CardInfo cardInfo;
            if (!_cardInfos.ContainsKey(internalId))
            {
                LogRecorder.Log("Unknown internal id " + internalId);
                cardInfo = new CardInfo
                {
                    CardName = "Unknown Card " + internalId,
                    InternalId = internalId,
                    CardType = CardType.Unknown,
                    CardAge = Age.A
                };
            }
            else
            {
                cardInfo = _cardInfos[internalId];
                cardInfo = cardInfo.Clone();
            }

            return cardInfo;
        }

        [Obsolete]
        public CardInfo GetCardInfoByName(Age age, String name)
        {
            var cardInfo = _cardInfos.FirstOrDefault(
                info => info.Value.CardAge == age && (info.Value.CardName == name
                )).Value;

            if (cardInfo == null)
            {
                LogRecorder.Log("Unknown age and name pair:" + age + " " + name);
                cardInfo = new CardInfo
                {
                    CardName =
                        name.Trim(),
                    InternalId = CardInfo.UnknownInternalId,
                    CardType = CardType.Unknown,
                    CardAge = age
                };
            }
            else
            {
                cardInfo = cardInfo.Clone();
            }

            return cardInfo;
        }

        [Obsolete]
        public CardInfo GetCardInfoByName(String name)
        {
            var translatedName =name;
            var cardInfo =
                _cardInfos.FirstOrDefault(info => info.Value.CardName == name || info.Value.CardName == translatedName)
                    .Value;

            if (cardInfo == null)
            {
                LogRecorder.Log("Unknown name:" + translatedName);
                cardInfo = new CardInfo
                {
                    CardName =
                        name.Trim(),
                    InternalId = CardInfo.UnknownInternalId,
                    CardType = CardType.Unknown,
                    CardAge = Age.A
                };
            }
            else
            {
                cardInfo = cardInfo.Clone();
            }

            return cardInfo;
        }

        public TtaRuleBook GetRuleBook()
        {
            return _ruleBook;
        }
    }
}