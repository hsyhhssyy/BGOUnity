using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Experimental.Networking;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Network.Bgo;

//using UnityEngine.Experimental.Networking;

public class UIBehaviour : MonoBehaviour
{

    public Text ConsoleTextBox;


    public void LoadWebpageButtonClicked()
    {
        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
            MethodInfo info = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);

            if (info != null)
                Debug.Log(info.Invoke(null, null));
        }
        

        StartCoroutine(GetText());
    }

    private IEnumerator GetText()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        //UnityWebRequest request = UnityWebRequest.Post("http://boardgaming-online.com/index.php?cnt=202&msg=400&pl=7307514&nat=1", formData);
        UnityWebRequest request = UnityWebRequest.Get("http://boardgaming-online.com/");
        yield return request.Send();

        if (request.isError)
        {
            Debug.Log(request.error);
        }
        else
        {
            String str = Encoding.UTF8.GetString(request.downloadHandler.data);

            ConsoleTextBox.text = str;

            BoardAnalyzer.AnalyzeBoard(str);

            Debug.Log("Form upload complete!");
        }
    }
}
