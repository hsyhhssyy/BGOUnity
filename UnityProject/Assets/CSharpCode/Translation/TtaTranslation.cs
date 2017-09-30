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
        private static Dictionary<String, String> _sourceLegend;
        private static Dictionary<String, String> _destLegend;

        public static void LoadDictionary()
        {
            if (_dictionary != null)
            {
                return;
            }

            LoadSourceLegend();
            LoadDestLegend();
            LoadTranslation();
            
        }

        private static void LoadSourceLegend()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Localization/legend_zh-cn_source");
            var dictStr = textAsset.text;

            _sourceLegend = new Dictionary<string, string>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length > 1)
                {
                    var key = sp[0];
                    var value = sp[1];
                    _sourceLegend.Add(key,value);
                   Debug.Log("Load Legend "+_sourceLegend[key]);
                }
            }
        }
        private static void LoadDestLegend()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Localization/legend_zh-cn_dest");
            var dictStr = textAsset.text;

            _destLegend = new Dictionary<string, string>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length > 1)
                {
                    var key = sp[0];
                    var value = sp[1];
                    _destLegend.Add(key, value);
                    Debug.Log("Load Legend " + _destLegend[key]);
                }
            }
        }

        private static void LoadTranslation()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Localization/dict_zh-cn");

            var dictStr = textAsset.text;

            _dictionary = new Dictionary<string, string>();

            var rows = dictStr.Split("\n".ToCharArray());

            foreach (var row in rows)
            {
                var sp = row.Trim().Split("|".ToCharArray());
                if (sp.Length > 1)
                {
                    _dictionary[ReplaceSourceLegend(sp[0])] = sp[1];
                }
            }
        }

        private static String ReplaceSourceLegend(String source)
        {
            foreach (var pair in _sourceLegend)
            {
                if (source.Contains(pair.Key))
                {
                    source=source.Replace(pair.Key, pair.Value);
                }
            }
            return source;
        }

        private static String ReplaceDestLegend(String source)
        {
            foreach (var pair in _destLegend)
            {
                if (source.Contains(pair.Key))
                {
                    source = source.Replace(pair.Key, pair.Value);
                }
            }
            return source;
        }

        public static String GetTranslatedText(String text)
        {
            if (_dictionary.ContainsKey(text))
            {
                return ReplaceDestLegend(_dictionary[text]);
            }

            return text;
        }
    }
}
