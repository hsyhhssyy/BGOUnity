using UnityEngine;
using System.Collections;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.Network.Bgo;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenUIBehaviour : MonoBehaviour
{

    public GameObject LoginButton;

	// Use this for initialization
	void Start () {
        TtaTranslation.LoadDictionary();

        Debug.Log("App Loaded");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoginButton_Clicked()
    {
        Debug.Log("LoginButton!");
        //LoginButton

        LoginButton.GetComponent<Button>().interactable = false;

        SceneTransporter.server = new BgoServer();
        StartCoroutine(SceneTransporter.server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Debug.Log("Logged in!");


            SceneManager.LoadScene("Scene/LobbyScene");
        }));
    }
}
