using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia.RuleBook;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.Civilopedia
{
    public class TtaCivilopedia
    {
        public const String TtaVersionBoardGamingOnline2 = "2.0";

        private static Dictionary<String, TtaCivilopedia> Civilopedias;

        public static TtaCivilopedia GetCivilopedia(String gameVersion)
        {
            TtaTranslation.LoadDictionary();
            LoadCiviloPedia();

            return Civilopedias[gameVersion];
        }

        public static void LoadCiviloPedia()
        {
            if (Civilopedias != null)
            {
                return;
            }

            TextAsset textAsset = Resources.Load<TextAsset>("Civilopedia/BGO.2.0");

            var dictStr = textAsset.text;

            Civilopedias = new Dictionary<string, TtaCivilopedia>();

            var civilopedia = new TtaCivilopedia();
            civilopedia.cardInfos = new Dictionary<string, CardInfo>();
            civilopedia.ruleBook=new TtaStandard0200RuleBook();

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
                        CardName = TtaTranslation.GetTranslatedText(csvRow[1]).Trim(),
                        CardType = (CardType) Convert.ToInt32(csvRow[2]),
                        CardAge = (Age) Convert.ToInt32(csvRow[3]),
                        Description = TtaTranslation.GetTranslatedText(csvRow[4]),
                        NormalImage = "SpriteTile/Cards/" + csvRow[5].Replace(".png", "")
                    };

                    info.SmallImage = info.NormalImage + "_small";
                    UnityResources.LazyLoadSprite(info.SmallImage,
                        () => UnityResources.ZoomSprite(info.NormalImage, new Vector2(0.5f, 0.5f), 70f/297f));

                    info.SpecialImage = "SpriteTile/Cards/" + csvRow[6].Replace(".png", "");

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

                    civilopedia.cardInfos[info.InternalId] = info;
                }
                catch (Exception e)
                {
                    LogRecorder.Log(e.Message+" "+row);
                }
            }

            Civilopedias.Add("2.0", civilopedia);
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
            var s=str.Split(",".ToCharArray());
            int id= Convert.ToInt32(s[0]);
            
            CardEffect e = new CardEffect {FunctionId = (CardEffectType) id};
            for (int i = 1; i < s.Length; i++)
            {
                e.Data.Add(Convert.ToInt32(s[i]));
            }

            return e;
        }

        //-------------------------------

        private Dictionary<String, CardInfo> cardInfos;
        private TtaRuleBook ruleBook;

        public List<CardInfo> GetAllCards()
        {
            return cardInfos.Values.ToList();
        } 

        public CardInfo GetCardInfoById(String internalId)
        {
            CardInfo cardInfo;
            if (!cardInfos.ContainsKey(internalId))
            {
                Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown internal id " + internalId);
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
                cardInfo = cardInfos[internalId];
                cardInfo = cardInfo.Clone();
            }

            return cardInfo;
        }

        public CardInfo GetCardInfoByName(Age age, String name)
        {
            var translatedName = TtaTranslation.GetTranslatedText(name);
            var cardInfo = cardInfos.FirstOrDefault(info => info.Value.CardAge == age && (info.Value.CardName == name || info.Value.CardName == translatedName)).Value;

            if (cardInfo == null)
            {
                Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown age and name pair:"+age+" " + name);
                cardInfo = new CardInfo
                {
                    CardName =
                        TtaTranslation.GetTranslatedText(name).Trim(),
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

        public CardInfo GetCardInfoByName(String name)
        {
            var translatedName = TtaTranslation.GetTranslatedText(name);
            var cardInfo =
                cardInfos.FirstOrDefault(info => info.Value.CardName == name || info.Value.CardName == translatedName)
                    .Value;

            if (cardInfo == null)
            {
                Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown name:" + translatedName);
                cardInfo = new CardInfo
                {
                    CardName =
                        TtaTranslation.GetTranslatedText(name).Trim(),
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
            return ruleBook;
        }
    }
}
