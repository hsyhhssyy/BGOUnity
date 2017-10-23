using System;
using UnityEngine;
using System.Collections;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Wcf.Json;
using Assets.CSharpCode.UI;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace Assets.CSharpCode.UI.ErrorScene
{
    public class ErrorScenUiBehaviour : MonoBehaviour
    {
        public InputField ErrorText;

        [UsedImplicitly]
        void Start()
        {
            var errorStr = "------Error-----";
            errorStr += Environment.NewLine + SceneTransporter.LastError;

            if (SceneTransporter.CurrentGame != null)
            {
                errorStr += Environment.NewLine + "------Game:" + SceneTransporter.CurrentGame.Name + "-----";
                errorStr += Environment.NewLine;
                JsonSerializer serializer = new JsonSerializer();
                try
                {
                    var obj=serializer.Serialize(SceneTransporter.CurrentGame,true);
                    errorStr+=obj.ToString();
                }
                catch (Exception e)
                {
                    errorStr += "Error Serialize CurrentGame:" + e.Message;
                }
            }

            if (SceneTransporter.Server != null)
            {
                errorStr += Environment.NewLine + "------Server:" + SceneTransporter.Server.GetType().Name + "-----";
                errorStr += Environment.NewLine;
                JsonSerializer serializer = new JsonSerializer();
                try
                {
                    var obj=serializer.Serialize(SceneTransporter.Server, true);
                    errorStr += obj.ToString();
                }
                catch (Exception e)
                {
                    errorStr += "Error Serialize Server:" + e.Message;
                }
            }

            ErrorText.text = errorStr;
        }

        [UsedImplicitly]
        void Update()
        {

        }

        public void CopyToClipBoard()
        {
            TextEditor te = new TextEditor();
            te.text = ErrorText.text;
            te.SelectAll();
            te.Copy();
        }
    }
}
