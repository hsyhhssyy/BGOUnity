using System;
using Assets.CSharpCode.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhaseDialog;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class PCBoardBehavior : MonoBehaviour
    {

        public int CurrentPlayerBoardIndex;

        public GameObject LoadingGo;

        public PCBoardDisplayBehavior BoardDisplay;

        public PCBoardActionBinder ActionBinder;
        public PCBoardActionTriggerController ActionTriggerController;


        /// <summary>
        /// 用于执行两段式命令，触发两段式命令的方式就是先写入interAction
        /// 然后执行BindAction（不需要Unbind）
        /// Binder因为interAction不同，而Band到不同的Action
        /// 执行完以后，记得要把interAction设为空
        /// </summary>
        public PlayerAction InterAction;

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
                CurrentPlayerBoardIndex = SceneTransporter.CurrentGame.MyPlayerIndex;
                BoardDisplay.Refresh();
                ActionBinder.BindAction(SceneTransporter.CurrentGame.PossibleActions,this);

                LoadingGo.SetActive(false);
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

            CurrentPlayerBoardIndex = no;

            InterAction = null;
            ActionBinder.Unbind();
            BoardDisplay.Refresh();
            ActionBinder.BindAction(SceneTransporter.CurrentGame.PossibleActions,this);
        }

        public void TakeAction(PlayerAction action,Action<List<PlayerAction>> internalActionCallback)
        {
            if (BackgroundRefreshing == true)
            {
                return;
            }

            LoadingGo.SetActive(true);
            if (action.Internal == false)
            {
                InterAction = null;
                StartCoroutine(SceneTransporter.Server.TakeAction(SceneTransporter.CurrentGame, action,
                    () =>
                    {
                        BoardDisplay.Refresh();
                        ActionBinder.BindAction(SceneTransporter.CurrentGame.PossibleActions,this);
                        LoadingGo.SetActive(false);
                        BackgroundRefreshing = false;
                    }));
            }
            else
            {
                InterAction = action;
                StartCoroutine(SceneTransporter.Server.TakeInternalAction(SceneTransporter.CurrentGame, action,
                    (actions) =>
                    {
                        LoadingGo.SetActive(false);
                        if (internalActionCallback != null)
                        {
                            internalActionCallback(actions);
                        }
                    }));
            }
        }
        
    }
}
