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

            Civilopedias =new Dictionary<string, TtaCivilopedia>();

            var civilopedia = new TtaCivilopedia();
            civilopedia.cardInfos=new Dictionary<string, CardInfo>();

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
                info.CardType = (CardType)Enum.Parse(typeof (CardType), sp[1]);
                info.CardName = TtaTranslation.GetTranslatedText(info.InternalId.Split("-".ToCharArray(), 2).Last()).WordWrap(7).Trim();
                info.CardAge = (Age)Convert.ToInt32(info.InternalId.Split("-".ToCharArray(), 2)[0]);
                info.SmallImage = sp[2];

                civilopedia.cardInfos[info.InternalId] = info;
                
            }

            Civilopedias.Add("2.0", civilopedia);
        }


        //-------------------------------

        private Dictionary<String, CardInfo> cardInfos;

        public CardInfo GetCardInfo(String internalId)
        {
            var cardInfo = (new[] {"", "0-", "1-", "2-", "3-"})
                .Select(x => x + internalId)
                .Intersect(cardInfos.Keys)
                .Select(x => cardInfos[x])
                .FirstOrDefault();
            

            if (cardInfo == null)
            {
                Debug.Log("Unknown internal id "+ internalId);
                cardInfo = new CardInfo
                {
                    CardName =
                        TtaTranslation.GetTranslatedText(internalId.Split("-".ToCharArray(), 2).Last())
                            .WordWrap(7)
                            .Trim(),
                    InternalId = internalId,
                    CardType = CardType.Action,
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
