using Assets.CSharpCode.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.CSharpCode.Entity;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class PCBoardBehavior : MonoBehaviour
    {

        public int CurrentPlayerNo;

        public GameObject LoadingGo;
        public PlayerBoardDisplayBehavior PlayerBoardDisplay;
        public PCBoardActionBinder actionBinder;

        /// <summary>
        /// 用于执行两段式命令，触发两段式命令的方式就是先写入interAction
        /// 然后执行BindAction
        /// Binder因为interAction不同，而Band到不同的Action
        /// 执行完以后，记得要把interAction设为空
        /// </summary>
        public PlayerAction interAction;

        public bool BackgroundRefreshing=false;

       [UsedImplicitly]
        void Start()
        {
            ExceptionHandle.SetupExceptionHandling();

            if (SceneTransporter.CurrentGame == null)
            {
                SceneManager.LoadScene("Scene/TestScene");
                return;
            }

            LoadingGo.SetActive(true);

            StartCoroutine(SceneTransporter.Server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                CurrentPlayerNo = SceneTransporter.CurrentGame.MyPlayerIndex;
                PlayerBoardDisplay.Refresh();
                actionBinder.BindAction();

                LoadingGo.SetActive(false);
            }));
        }

        private void BackgroundRefresh()
        {
            BackgroundRefreshing = true;
            StartCoroutine(SceneTransporter.Server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                LoadingGo.SetActive(true);
                PlayerBoardDisplay.Refresh();
                actionBinder.BindAction();
                LoadingGo.SetActive(false);
                BackgroundRefreshing = false;
            }));
        }

        internal void SwitchBoard(int no)
        {
            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                return;
            }

            if (SceneTransporter.CurrentGame.Boards.Count <= no)
            {
                return;
            }

            CurrentPlayerNo = no;
            PlayerBoardDisplay.Refresh();
        }

        public void TakeAction(PlayerAction action)
        {
            if (BackgroundRefreshing == true)
            {
                return;
            }

            LoadingGo.SetActive(true);
            StartCoroutine(SceneTransporter.Server.TakeAction(SceneTransporter.CurrentGame, action,
                BackgroundRefresh));

        }
        
    }
}
