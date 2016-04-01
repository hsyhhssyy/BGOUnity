using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class TestSceneUIBehaviour : MonoBehaviour
{
    public bool isDebug = false;

    public GameObject Canvas;

    [UsedImplicitly]
    private void Start()
    {
        Debug.Log("Started");

        var informationMesh = GameObject.Find("Information").GetComponent<TextMesh>();

        informationMesh.text = SceneTransporter.LastError.WordWrap(60);
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

            SceneManager.LoadScene("Scene/BoardScene");
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

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "2.5尝鲜 10" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene");
        }));
    }

    public void 根据测试文本展示页面_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoTestServer();

        Debug.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
            Debug.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "2.5尝鲜 10" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene");
        }));
    }
}
