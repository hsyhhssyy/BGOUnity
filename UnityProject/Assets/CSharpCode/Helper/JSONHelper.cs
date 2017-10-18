using System;

namespace Assets.CSharpScripts.Helper
{
    public static class JsonHelper
    {
        /// <summary>
        /// 支持按照中括号和点来进行扫描如xxx[yyy].zzz
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JSONObject TryGetPath(this JSONObject obj, String path)
        {

            String currentPath = path.Trim();
            JSONObject currentObject = obj;

            if (String.IsNullOrEmpty(currentPath)||obj==null)
            {
                return null;
            }

            if (currentPath.StartsWith(".") )
            {
                return currentObject.TryGetPath(currentPath.Substring(1));
            }
            else if ( currentPath.StartsWith("["))
            {
                var delSplit = currentPath.Split("]".ToCharArray(), 2);
                if (delSplit.Length == 1)
                {
                    return currentObject.TryGetPath(delSplit[0]);
                }
                else
                {
                    return currentObject.TryGetPath(delSplit[0]+delSplit[1]);
                }
            }
            else
            {
                var split = currentPath.Split(".[".ToCharArray(), 2);
                if (split.Length == 1)
                {
                    return currentObject.TryGetField(split[0]);
                }
                else
                {
                    return currentObject.TryGetField(split[0]).TryGetPath(currentPath.Substring(split[0].Length));
                }
            }
        }

        public static String TryGetPathString(this JSONObject obj, String path)
        {
            var objJson = obj.TryGetPath(path);
            if (objJson == null)
            {
                return null;
            }
            else
            {
                if (objJson.IsObject || objJson.isContainer|| objJson.IsArray)
                {
                    return null;
                }
                if (objJson.IsBool)
                {
                    return objJson.b.ToString();
                }
                if (objJson.IsString)
                {
                    return objJson.str;
                }
                return objJson.ToString();
            }
        }

        public static String GetPathString(this JSONObject obj, String path)
        {
            var objJson = TryGetPathString(obj,path);
            return objJson ?? "";
        }

        public static JSONObject TryGetField(this JSONObject obj, String name)
        {
            if (obj.HasField(name))
            {
                return obj.GetField(name);
            }
            else
            {
                return null;
            }
        }
    }
}
