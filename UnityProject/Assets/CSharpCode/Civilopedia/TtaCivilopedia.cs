﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Translation;

namespace Assets.CSharpCode.Civilopedia
{
    public class TtaCivilopedia
    {
        public static TtaCivilopedia GetCivilopedia(String gameVersion)
        {
            TtaTranslation.LoadDictionary();

            return new TtaCivilopedia();
        }

        public CardInfo getCardInfo(String internalId)
        {
            return null;
        }
    }
}