using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.Network.Wcf
{
    public static class WcfPostOperator
    {
        public static IEnumerator PostJson(String uri, JSONObject json)
        {
            WWWForm myPostData = new WWWForm();
            var headers = myPostData.headers;
            headers["Content-Type"] = "application/json";
            headers["Accept"] = "application/json";

            var str = json.ToString(true);
            byte[] rawData = Encoding.UTF8.GetBytes(str);

            var www = new WWW(uri,
                rawData, headers);

            yield return www;

            if (www.error != null)
            {
                Assets.CSharpCode.UI.Util.LogRecorder.Log(www.error);

                yield break;
            }
            
            var html = www.text;

            Assets.CSharpCode.UI.Util.LogRecorder.Log("Json received"+html);
            yield return new JSONObject(html);
        }

        
    }
}
