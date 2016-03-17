using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Helper
{
    public static class ExtensionMethods
    {
        private const string _newline = "\r\n";

        public static string WordWrap2(this string theString, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return theString;

            // Parse each line of text
            for (pos = 0; pos < theString.Length; pos = next)
            {
                // Find end of line
                int eol = theString.IndexOf(_newline, pos);

                if (eol == -1)
                    next = eol = theString.Length;
                else
                    next = eol + _newline.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > width)
                            len = BreakLine(theString, pos, width);

                        sb.Append(theString, pos, len);
                        sb.Append(_newline);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && Char.IsWhiteSpace(theString[pos]))
                            pos++;

                    } while (eol > pos);
                }
                else sb.Append(_newline); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max; // No whitespace found; break at maximum length
                            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;
            // Return length of text before whitespace
            return i + 1;
        }

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
    }
}
