using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.CSharpCode.UI.StartScreen
{
    [UsedImplicitly]
    public class StartScreenUIBehaviour : MonoBehaviour
    {

        public GameObject LoginButton;

        [UsedImplicitly]
        void Start () {
            TtaTranslation.LoadDictionary();

            Debug.Log("App Loaded");
        }

        [UsedImplicitly] 
        public void LoginButton_Clicked()
        {
            Debug.Log("LoginButton!");
            //LoginButton

            LoginButton.GetComponent<Button>().interactable = false;

            SceneTransporter.Server = new BgoServer();
            StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
            {
                Debug.Log("Logged in!");


                SceneManager.LoadScene("Scene/LobbyScene");
            }));
        }

        [UsedImplicitly]
        public void EngineerModeButton_Clicked()
        {
            Debug.Log("Engineer!");
           
            SceneManager.LoadScene("Scene/TestScene");
            
        }
    }
}
