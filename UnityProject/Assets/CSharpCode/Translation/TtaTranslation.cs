using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.Translation
{
    public static class TtaTranslation
    {
        private static Dictionary<String, String> _dictionary; 

        public static void LoadDictionary()
        {
            if (_dictionary != null)
            {
                return;
            }

            TextAsset textAsset= Resources.Load<TextAsset>("Localization/dict_zh-cn");

            var dictStr= textAsset.text;

            _dictionary=new Dictionary<string, string>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length > 1)
                {
                    _dictionary[sp[0]] = sp[1];
                }
            }
        }

        public static String GetTranslatedText(String text)
        {
            if (_dictionary.ContainsKey(text))
            {
                return _dictionary[text];
            }

            return text;
        }
    }
}
