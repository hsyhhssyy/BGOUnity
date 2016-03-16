using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.Translation
{
    public class TtaTranslation
    {
        private static Dictionary<String, String> dictionary; 

        public static void LoadDictionary()
        {
            TextAsset textAsset= Resources.Load<TextAsset>("Localization/dict_zh-cn");

            var dictStr= textAsset.text;

            dictionary=new Dictionary<string, string>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length > 1)
                {
                    dictionary[sp[0]] = sp[1];
                }
            }
        }

        public static String GetTranslatedText(String text)
        {
            if (dictionary.ContainsKey(text))
            {
                return dictionary[text];
            }

            return text;
        }
    }
}
