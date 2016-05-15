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

           Assets.CSharpCode.UI.Util.LogRecorder.Log("App Loaded");
        }

        [UsedImplicitly] 
        public void LoginButton_Clicked()
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("LoginButton!");
            //LoginButton

            LoginButton.GetComponent<Button>().interactable = false;

            SceneTransporter.Server = new BgoServer();
            StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");


                SceneManager.LoadScene("Scene/LobbyScene");
            }));
        }

        [UsedImplicitly]
        public void EngineerModeButton_Clicked()
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Engineer!");
           
            SceneManager.LoadScene("Scene/TestScene");
            
        }
    }
}
