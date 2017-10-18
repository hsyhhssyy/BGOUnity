using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TtaWcfServer.Util
{
    public static class WcfCastUtil
    {
        public static Dictionary<T1,T2> CastDictionary<T1, T2>(object data)
        {
            var dict = new Dictionary<T1, T2>();
            if (data is object[])
            {
                foreach (var o in (object[])data)
                {
                    if (o is KeyValuePair<T1, T2>)
                    {
                        var pair = (KeyValuePair<T1, T2>) o;
                        dict.Add(pair.Key,pair.Value);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }

            return dict;
        } 
    }
}