using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.Helper
{
    public class CsvUtil
    {
        public static List<String> SplitRow(String str)
        {
            List<String> Columns = new List<string>();

            String lastEntry = null;
            String col = "";
            foreach (char t in str)
            {
                switch (lastEntry)
                {
                    case null:
                    case ",":
                        switch (t)
                        {
                            case '"':
                                lastEntry = "(";
                                break;
                            case ',':
                                Columns.Add(col);
                                col = "";
                                lastEntry = ",";
                                break;
                            default:
                                col += t;
                                break;
                        }
                        break;
                    case "(":
                        if (t == '"')
                        {
                            lastEntry = ")";
                        }
                        else
                        {
                            col += t;
                        }
                        break;
                    case ")":
                        if (t == ',')
                        {
                            Columns.Add(col);
                            col = "";
                            lastEntry = ",";
                        }
                        else
                        {
                            LogRecorder.Log(str);
                            return null;
                        }
                        break;
                }
            }

            Columns.Add(col);
            return Columns;
        }
    }
}
