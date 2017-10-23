using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Network.Wcf.Json;
using UnityEngine;
using Object = System.Object;

namespace Assets.CSharpCode.UI.TestScene
{
    public class MonoPlayground:MonoBehaviour
    {
        public void Play()
        {
            JsonSerializer serial=new JsonSerializer();
            JSONObject obj = JSONObject.Create("[{\"Key\": 0,\"Value\": 2}]");
            //var ser=serial.DeserializeIDictionary(obj, typeof(Dictionary<Int32, Object>), typeof(Int32), typeof(object));
        }
    }
}
