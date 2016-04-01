using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using UnityEngine;

namespace Assets.CSharpCode.Helper
{
    public static class ExtensionMethods
    {
        private const string _newline = "\r\n";
        
        #region 中英文混排自动折行
        public static String WordWrap(this String strToConvert, int length)
        {
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
                        if (oneOfVal != "")
                        {
                            xmlString.AppendLine(oneOfVal);
                            startIndex += GetStrByteLength(oneOfVal);
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

        public static bool IsChinese(char c)
        {
            System.Text.Encoding.Default.GetByteCount(new char[]{c});
            return (int)c >= 0x4E00 && (int)c <= 0x9FA5;
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

        public static GameObject FindObject(this GameObject parent, string name)
        {
            Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
            foreach (Transform t in trs)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }
            return null;
        }

        public static List<PlayerAction> FindAction(this List<PlayerAction> actions)
        {
            return null;
        }
    }
}
