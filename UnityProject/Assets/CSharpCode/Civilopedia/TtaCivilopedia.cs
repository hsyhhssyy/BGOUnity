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
                if (sp.Length > 1)
                {
                   CardInfo info=new CardInfo();
                    info.InternalId = sp[0];
                    info.CardType = (CardType)Enum.Parse(typeof (CardType), sp[1]);
                    info.CardName = TtaTranslation.GetTranslatedText(info.InternalId.Split("-".ToCharArray(), 2).Last()).WordWrap(7).Trim();
                    civilopedia.cardInfos[info.InternalId] = info;
                }
            }

            Civilopedias.Add("2.0", civilopedia);
        }


        //-------------------------------

        private Dictionary<String, CardInfo> cardInfos; 

        public CardInfo GetCardInfo(String internalId)
        {
            if (cardInfos.ContainsKey(internalId))
            {
                return cardInfos[internalId];
            }

            CardInfo info = new CardInfo();
            info.CardName =
                TtaTranslation.GetTranslatedText(internalId.Split("-".ToCharArray(), 2).Last()).WordWrap(7).Trim();
            info.InternalId = internalId;
            info.CardType = CardType.Action;
            return info;
        }
    }
}
