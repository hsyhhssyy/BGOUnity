using UnityEngine;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using System.Collections.Generic;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.UI.PCBoardScene;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;

public class TestSceneUIBehaviour : MonoBehaviour
{
    public bool isDebug = false;

    public GameObject Canvas;
    public GameObject CardSmall;
    public GameObject CardNormal;

    private List<CardInfo> TestCards;

    [UsedImplicitly]
    private void Start()
    {
       Assets.CSharpCode.UI.Util.LogRecorder.Log("Started");

        var informationMesh = GameObject.Find("Information").GetComponent<TextMesh>();

        informationMesh.text = SceneTransporter.LastError.WordWrap(60);

        if (File.Exists(Application.persistentDataPath + "/UsernamePassword"))
        {

            var up = File.ReadAllText(Application.persistentDataPath + "/UsernamePassword").Split("|".ToCharArray());
            GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text = up[0];
            GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text = up[1];
        }

        LoadTestCards();
    }



    #region 几个直接打开特定页面的按钮

    [UsedImplicitly]
    public void 登录并打开尝鲜8_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server=new BgoServer();

       Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame {GameId = "7279176", Nat = "1", Name = "2.5尝鲜 8"};

            SceneTransporter.CurrentGame = g;
            
            ExtensionMethods.LoadScene("Scene/BoardScene-PC",1);
        }));
    }
    [UsedImplicitly]
    public void 登录并打开尝鲜9_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279177", Nat = "1", Name = "2.5尝鲜 9", Version = "2.0" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene-PC");
        }));
    }
    [UsedImplicitly]
    public void 登录并打开尝鲜10_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "2.5尝鲜 10",Version="2.0" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene-PC");
        }));
    }
    [UsedImplicitly]
    public void 登录并打开测试用局01_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7397386", Nat = "1", Name = "TTA开发内测用局01", Version = "2.0" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene-PC");
        }));
    }

    [UsedImplicitly]
    public void 根据测试文本展示页面_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoTestServer();
        (SceneTransporter.Server as BgoTestServer).File = "Test/TestPage1";

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "Text based Game" };

            SceneTransporter.CurrentGame = g;

            ExtensionMethods.LoadScene("Scene/BoardScene-PC", 1);
            
        }));
    }

    [UsedImplicitly]
    public void 根据测试文本展示页面2_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoTestServer();
        (SceneTransporter.Server as BgoTestServer).File = "Test/TestPage2";
        

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
            Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "Text based Game" };

            SceneTransporter.CurrentGame = g;

            ExtensionMethods.LoadScene("Scene/BoardScene-PC", 1);

        }));
    }

    #endregion

    #region 登录BGO的相关代码

    [UsedImplicitly]
    public void LoginButton_Clicked()
    {
       Assets.CSharpCode.UI.Util.LogRecorder.Log("LoginButton!");
        //LoginButton

        GameObject.Find("Canvas/Login/Login").GetComponent<Button>().interactable = false;

        SceneTransporter.Server = new BgoServer();

        StartCoroutine(SceneTransporter.Server.LogIn(
            GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text,
            GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text, () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            
                var up = GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text + "|" +
                         GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text;
                File.WriteAllText(Application.persistentDataPath + "/UsernamePassword", up);
            

            StartCoroutine(SceneTransporter.Server.ListGames(
            gamesReturn =>
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log("List Get!");

                SceneTransporter.LastListedGames = gamesReturn;

                var dropDown = GameObject.Find("Canvas/Login/MyGamesList").GetComponent<Dropdown>();
                dropDown.options = new List<Dropdown.OptionData>();
                foreach (var pa in gamesReturn)
                {
                    UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = pa.Name;
                    dropDown.options.Add(op);
                }

                if (dropDown.options.Count > 0)
                {
                    dropDown.value = 0; GameObject.Find("Canvas/Login/GoButton").GetComponent<Button>().interactable = true;
                }
            }));
        }));
    }

    [UsedImplicitly]
    public void GoButton_Clicked()
    {
        var dropDown = GameObject.Find("Canvas/Login/MyGamesList").GetComponent<Dropdown>();
        SceneTransporter.CurrentGame = SceneTransporter.LastListedGames[dropDown.value];
        SceneManager.LoadScene("Scene/BoardScene-PC");
    }

    #endregion

    #region 测试CardNormal和CardSmall绘制的相关代码

    private void LoadTestCards()
    {
        TestCards=TtaCivilopedia.GetCivilopedia(TtaCivilopedia.TtaVersionBoardGamingOnline2).GetAllCards();
        
        var dropDown = GameObject.Find("Canvas/CardSelect").GetComponent<Dropdown>();
        dropDown.options = new List<Dropdown.OptionData>();
        foreach (var ca in TestCards)
        {
            UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
            op.text = ca.CardName;
            dropDown.options.Add(op);
        }


        CardSmall.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[0]);
        CardNormal.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[0]);
    }

    [UsedImplicitly]
    public void CardSelect_ValueChanged()
    {
        var dropDown = GameObject.Find("Canvas/CardSelect").GetComponent<Dropdown>();
        CardSmall.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[dropDown.value]);
        CardNormal.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[dropDown.value]);
    }

    #endregion
}
