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

public class TestSceneUIBehaviour : MonoBehaviour
{
    public bool isDebug = false;

    public GameObject Canvas;
    public GameObject TestObject;


    [UsedImplicitly]
    private void Start()
    {
        Debug.Log("Started");

        var informationMesh = GameObject.Find("Information").GetComponent<TextMesh>();

        informationMesh.text = SceneTransporter.LastError.WordWrap(60);
        
        if (File.Exists(Application.persistentDataPath + "/UsernamePassword"))
        {
        
        var up = File.ReadAllText(Application.persistentDataPath + "/UsernamePassword").Split("|".ToCharArray());
            GameObject.Find("Canvas/Username").GetComponent<InputField>().text = up[0];
            GameObject.Find("Canvas/Password").GetComponent<InputField>().text = up[1];
        }

         var actionCard= new CardInfo
        {
            CardName =
                        "高效升级",
            InternalId = "Internal",
            Description = "升级一座农场、矿山或者城市建筑物，少花费4[R]。",
            CardType = CardType.Action,
            CardAge = Age.II
        };
        var urbanCard = new CardInfo
        {
            CardName =
                        "剧院",
            InternalId = "Internal",
            Description = "[F] [C]2",
            CardType = CardType.UrbanTechTheater,
            CardAge = Age.I,
            ResearchCost = {5},
            BuildCost = {6}

        };
        TestObject.GetComponent<PCBoardCardSmallDisplayBehaviour>().Bind(urbanCard);
    }

[UsedImplicitly]
    public void 登录并打开尝鲜8_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server=new BgoServer();

        Debug.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Debug.Log("Logged in!");

            BgoGame g = new BgoGame {GameId = "7279176", Nat = "1", Name = "2.5尝鲜 8"};

            SceneTransporter.CurrentGame = g;
            
            ExtensionMethods.LoadScene("Scene/BoardScene-PC",1);
        }));
    }
    [UsedImplicitly]
    public void 登录并打开尝鲜10_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Debug.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Debug.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "2.5尝鲜 10",Version="2.0" };

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

        Debug.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
            Debug.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "Text based Game" };

            SceneTransporter.CurrentGame = g;

            ExtensionMethods.LoadScene("Scene/BoardScene-PC", 1);
            
        }));
    }

    [UsedImplicitly]
    public void LoginButton_Clicked()
    {
        Debug.Log("LoginButton!");
        //LoginButton

        GameObject.Find("Canvas/Login").GetComponent<Button>().interactable = false;

        SceneTransporter.Server = new BgoServer();

        StartCoroutine(SceneTransporter.Server.LogIn(
            GameObject.Find("Canvas/Username").GetComponent<InputField>().text,
            GameObject.Find("Canvas/Password").GetComponent<InputField>().text, () =>
        {
            Debug.Log("Logged in!");

            
                var up = GameObject.Find("Canvas/Username").GetComponent<InputField>().text + "|" +
                         GameObject.Find("Canvas/Password").GetComponent<InputField>().text;
                File.WriteAllText(Application.persistentDataPath + "/UsernamePassword", up);
            

            StartCoroutine(SceneTransporter.Server.ListGames(
            gamesReturn =>
            {
                Debug.Log("List Get!");

                SceneTransporter.LastListedGames = gamesReturn;

                var dropDown = GameObject.Find("Canvas/MyGamesList").GetComponent<Dropdown>();
                dropDown.options = new List<Dropdown.OptionData>();
                foreach (var pa in gamesReturn)
                {
                    UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = pa.Name;
                    dropDown.options.Add(op);
                }

                if (dropDown.options.Count > 0)
                {
                    dropDown.value = 0; GameObject.Find("Canvas/GoButton").GetComponent<Button>().interactable = true;
                }
            }));
        }));
    }

    [UsedImplicitly]
    public void GoButton_Clicked()
    {
        var dropDown = GameObject.Find("Canvas/MyGamesList").GetComponent<Dropdown>();
        SceneTransporter.CurrentGame = SceneTransporter.LastListedGames[dropDown.value];
        SceneManager.LoadScene("Scene/PCBoardScene");
    }

}
