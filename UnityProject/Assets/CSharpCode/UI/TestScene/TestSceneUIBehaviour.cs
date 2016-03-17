using UnityEngine;
using System.Collections;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class TestSceneUIBehaviour : MonoBehaviour
{

    public GameObject Canvas;

    [UsedImplicitly]
    private void Start()
    {
        Debug.Log("Started");

        var informationMesh = GameObject.Find("Information").GetComponent<TextMesh>();

        informationMesh.text = "这是一段中英文混排的很长的字要换行".WordWrap(7);
    }

    [UsedImplicitly]
    public void 登录并打开尝鲜8_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.server=new BgoServer();

        StartCoroutine(SceneTransporter.server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Debug.Log("Logged in!");

            BgoGame g = new BgoGame {GameId = "7279176", Nat = "1", Name = "2.5尝鲜 8"};

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene");
        }));
    }

}
