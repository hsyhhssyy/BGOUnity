using System.IO;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Network.Wcf;
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
        private static string PasswordFile;
        public GameObject LoginButton;
        public InputField Username;
        public InputField Password;
        public Dropdown ServerList;

        [UsedImplicitly]
        void Start () {
            TtaTranslation.LoadDictionary();

           Assets.CSharpCode.UI.Util.LogRecorder.Log("App Loaded");
            PasswordFile = Application.persistentDataPath + "//password.txt";
            if (File.Exists(PasswordFile))
            {
                StreamReader fs = new StreamReader(new FileStream(PasswordFile, FileMode.Open));
                string usr = fs.ReadLine();
                string pas = fs.ReadLine();
                Username.text = usr;
                Password.text = pas;
                fs.Close();
            }
        }

        [UsedImplicitly] 
        public void LoginButton_Clicked()
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("LoginButton!");
            //LoginButton

            LoginButton.GetComponent<Button>().interactable = false;

            var server = ServerList.value;

            switch (server)
            {
                case 1:
                    SceneTransporter.Server = new WcfServer();
                    break;
                case 2:
                    SceneTransporter.Server = new WcfServer();
                    WcfServiceProvider.ServerUrlBase = "http://localhost:50487/Service/";
                    break;
                default:
                    SceneTransporter.Server = new BgoServer();
                    break;
            }

            StartCoroutine(SceneTransporter.Server.LogIn(Username.text, Password.text, (error) =>
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");
                StreamWriter fs;
                if (!File.Exists(PasswordFile))
                {
                    fs=File.CreateText(PasswordFile);
                }
                else { 
                    fs = new StreamWriter(new FileStream(PasswordFile, FileMode.Truncate));
                }

                fs.WriteLine(Username.text);
                fs.WriteLine(Password.text);
                fs.Close();

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
