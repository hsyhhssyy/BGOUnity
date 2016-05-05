using Assets.CSharpCode.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PCBoradBehavior : MonoBehaviour
    {

        public int CurrentPlayerNo;

        public GameObject LoadingGo;

        public PlayerBoardDisplayBehavior PlayerBoardDisplay;

        // Use this for initialization
        void Start()
        {
            ExceptionHandle.SetupExceptionHandling();

            if (SceneTransporter.CurrentGame == null)
            {
                SceneManager.LoadScene("Scene/TestScene");
                return;
            }

            LoadingGo.SetActive(true);


            StartCoroutine(RefreshBoard());
        }

        private IEnumerator RefreshBoard()
        {
            return SceneTransporter.Server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                LoadingGo.SetActive(false);
                PlayerBoardDisplay.Refresh();

            });
        }

        internal void SwitchBoard(int No)
        {
            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                return;
            }

            if (SceneTransporter.CurrentGame.Boards.Count <= No)
            {
                return;
            }

            CurrentPlayerNo = No;
            PlayerBoardDisplay.Refresh();
        }

        // Update is called once per frame
        void Update () {
	
        }
    }
}
