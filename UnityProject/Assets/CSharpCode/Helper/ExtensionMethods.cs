using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.CSharpCode.Helper
{
    public static class ExtensionMethods
    {
        #region 中英文混排自动折行
        public static String WordWrap(this String strToConvert, int length)
        {
            if (strToConvert == null)
            {
                return null;
            }

                StringBuilder xmlString = new StringBuilder();
                string trimedSourceStr = strToConvert.Trim();
                if ((trimedSourceStr.Length / length + 1) > 1)
                {
                    int startIndex = 0;
                    int subLen = length;
                    int subCount = trimedSourceStr.Length / length + 1;
                    for (int i = 0; i < subCount; i++)
                    {
                        string oneOfVal = SubStrLenth(trimedSourceStr, startIndex, subLen * 2);
                        if (oneOfVal.Trim() != "")
                        {
                            if (oneOfVal.Contains("\n"))
                            {
                                int index = oneOfVal.IndexOf("\n");
                                xmlString.AppendLine(oneOfVal.Substring(0, index+1).TrimEnd());
                                startIndex += GetStrByteLength(oneOfVal.Substring(0, index + 1));
                            }
                            else
                            {
                                xmlString.AppendLine(oneOfVal);
                                startIndex += GetStrByteLength(oneOfVal);
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                else
                {
                    xmlString.AppendLine( trimedSourceStr);
                }


            return xmlString.ToString().Trim();
            
        }
        
        private static int GetStrByteLength(string str)
        {
            return System.Text.Encoding.Default.GetByteCount(str);
        }
        private static string SubStrLenth(string str, int startIndex, int length)
        {
            int strlen = GetStrByteLength(str);
            if (startIndex + 1 > strlen)
            {
                return "";
            }
            int j = 0;//记录遍历的字节数
            int L = 0;//记录每次截取开始，遍历到开始的字节位，才开始记字节数
            bool b = false;//当每次截取时，遍历到开始截取的位置才为true
            string restr = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                char C = str[i];
                var strW = System.Text.Encoding.Default.GetByteCount(new char[] {C});//IsChinese(C) ? 2 : 1;//字符宽度
                if ((L == length - 1) && (L + strW > length))
                {
                    b = false;
                    break;
                }
                if (j >= startIndex)
                {
                    restr += C;
                    b = true;
                }

                j += strW;

                if (b)
                {
                    L += strW;
                    if (((L + 1) > length))
                    {
                        b = false;
                        break;
                    }
                }

            }
            return restr;
        }

        #endregion

        #region 字符串裁切

        [UsedImplicitly]
        public static String CutBetween(this String origin, String prefix, String suffix)
        {
            var index1 = origin.IndexOf(prefix, StringComparison.Ordinal);
            var index2= origin.IndexOf(suffix, StringComparison.Ordinal);
            if (index1 == -1|| index2==-1)
            {
                return origin.Substring(index1==-1?0: index1 + prefix.Length, index2==-1? origin .Length- index1 - prefix.Length : index2 - prefix.Length);
            }
            var cutPiece=origin.Substring(index1 + prefix.Length, index2 - index1 - prefix.Length);
            return cutPiece;
        }
        [UsedImplicitly]
        public static String CutAfter(this String origin, String prefix)
        {
            var index1 = origin.IndexOf(prefix, StringComparison.Ordinal);
            if (index1 == -1)
            {
                return origin;
            }
            var cutPiece = origin.Substring(index1 + prefix.Length, origin.Length-index1-prefix.Length);
            return cutPiece;
        }
        /// <summary>
        /// 返回给定模板之前的内容，如对abcdef调用并传入def，返回abc
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        [UsedImplicitly]
        public static String CutBefore(this String origin, String suffix)
        {
            var index2 = origin.IndexOf(suffix, StringComparison.Ordinal);
            if (index2 == -1)
            {
                return origin;
            }
            var cutPiece = origin.Substring(0, index2);
            return cutPiece;
        }

        #endregion

        public static GameObject FindObject(this GameObject parent, string name)
        {
            Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
            return (from Transform t in trs where t.name == name select t.gameObject).FirstOrDefault();
        }

        #region Scene之间交换数据

        private static readonly Dictionary<String, object> Storage=new Dictionary<string, object>(); 

        public static void LoadScene<T>(String name, params T[] parameter )
        {
            Storage[name] = new List<T>(parameter);
            SceneManager.LoadScene(name);
        }

        public static List<T> GetPassedData<T>(this MonoBehaviour go)
        {
            var scene = SceneManager.GetActiveScene();
            var path = scene.path.Substring("Assets/".Length, scene.path.Length - ("Assets/"+".unity").Length);
            return Storage.ContainsKey(path) ? (List<T>) Storage[path] : new List<T>();
        }

        #endregion

        #region 缩放Sprite

        public static Sprite CloneResize(this Sprite sp, Vector2 pivot,float scale)
        {
            var sprite = Sprite.Create(sp.texture, sp.rect, pivot, sp.pixelsPerUnit / scale);
            return sprite;
        }
        #endregion

        #region Collider2D对指针的检测

        public static bool IsMouseHitting(this Collider2D collider)
        {
            Collider2D[] cols = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition));

            foreach (var col in cols)
            {
                if (col == collider)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
