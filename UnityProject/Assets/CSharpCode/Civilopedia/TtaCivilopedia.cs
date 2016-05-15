using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Translation;
using UnityEngine;

namespace Assets.CSharpCode.Civilopedia
{
    public class TtaCivilopedia
    {
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

            TextAsset textAsset = Resources.Load<TextAsset>("Civilopedia/TTA7");

            var dictStr = textAsset.text;

            Civilopedias = new Dictionary<string, TtaCivilopedia>();

            var civilopedia = new TtaCivilopedia();
            civilopedia.cardInfos = new Dictionary<string, CardInfo>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length < 2)
                {
                    continue;
                }
                CardInfo info = new CardInfo();
                info.InternalId = sp[0];
                info.CardType = (CardType)Enum.Parse(typeof(CardType), sp[1]);
                info.CardName = TtaTranslation.GetTranslatedText(info.InternalId.Split("-".ToCharArray(), 2).Last()).Trim();
                info.CardAge = (Age)Convert.ToInt32(info.InternalId.Split("-".ToCharArray(), 2)[0]);

                info.SmallImage = sp[3].Replace(".png","")+"_small";
                info.NormalImage = sp[3].Replace(".png", "");

                info.ResearchCost = new List<int> { 1,1 };
                info.BuildCost = new List<int> { 1 };
                info.RedMarkerCost = new List<int> { 3};

                info.TacticComposition = new List<int> {1,2, 3 };
                info.TacticValue = new List<int> { 1, 2 };

                info.Description = "Test";

                civilopedia.cardInfos[info.InternalId] = info;

            }

            Civilopedias.Add("2.0", civilopedia);
        }


        //-------------------------------

        private Dictionary<String, CardInfo> cardInfos;

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
                Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown age and name pair:" + name);
                cardInfo = new CardInfo
                {
                    CardName =
                        TtaTranslation.GetTranslatedText(name).Trim(),
                    InternalId = "Unknown!",
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
                Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown age and name pair:" + name);
                cardInfo = new CardInfo
                {
                    CardName =
                        TtaTranslation.GetTranslatedText(name).Trim(),
                    InternalId = "Unknown!",
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
    }
}
