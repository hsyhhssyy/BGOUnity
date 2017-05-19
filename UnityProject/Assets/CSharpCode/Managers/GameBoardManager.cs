using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers.GameBoardStateHandlers;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Assets.CSharpCode.Managers
{
    public class GameBoardManager : MonoBehaviour
    {
        #region 一系列复杂的用于维护当前游戏状态机的变量

        public TtaGame CurrentGame
        {
            get { return SceneTransporter.CurrentGame; }
        }

        public int CurrentDisplayingBoardNo = 0;

        public GameManagerState State;
        public Dictionary<String, System.Object> StateData = new Dictionary<string, object>();

        public Dictionary<GameManagerState, List<GameBoardStateHandler>> Handlers; 

        public GameEventChannel Channel { get; private set; }

        public NetworkManager NetworkManager;


        #endregion

        public GameBoardManager()
        {
            Channel = SceneTransporter.CurrentChannel;
            Channel.GameEvent += DestoryNullReference;
            
            NetworkManager = new NetworkManager(this, Channel);

            Handlers = new Dictionary<GameManagerState, List<GameBoardStateHandler>>
            {
                {
                    GameManagerState.ActionPhaseIdle, new List<GameBoardStateHandler>
                    {
                        new ActionPhaseIdleStateHandler(this)
                    }
                },
                {
                    GameManagerState.ActionPhaseInternalQuery, new List<GameBoardStateHandler>
                    {
                        new ActionPhaseInternalQueryHandler(this)
                    }
                },
                {
                    GameManagerState.ActionPhaseChooseTarget, new List<GameBoardStateHandler>
                    {
                        new ActionPhaseChooseTargetStateHandler(this)
                    }
                },
                {
                    GameManagerState.OutOfMyTurn, new List<GameBoardStateHandler>
                    {
                        new OutOfMyTurnStateHandler(this)
                    }
                }
            };
        }

        #region 接受和分发消息的部分

        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        private void DestoryNullReference(System.Object sender, GameUIEventArgs args)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this == null)
            {
                Channel.GameEvent -= DestoryNullReference;
            }
            else
            {
                OnSubscribedUiControllerEvents(sender, args);
            }
        }

        public void Regiseter(TtaUIControllerMonoBehaviour controller)
        {
            controller.Channel = Channel;

            LogRecorder.Log(controller.name + " Registed");
        }

        private void OnSubscribedUiControllerEvents(System.Object sender, GameUIEventArgs args)
        {
            //只有在刷新时才会出现，否则定时请求刷新
            if (args.EventType == GameUIEventType.Refresh)
            {
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "PCBoard"));
                VerifyState();
            }

            if (Handlers.ContainsKey(State))
            {
                var hand = Handlers[State];
                if (hand != null)
                {
                    foreach (var handler in hand)
                    {
                        handler.ProcessGameEvents(sender,args);
                    }
                }
            }

        }

        #endregion

        #region 程序初始化的最开始一步

        [UsedImplicitly]
        private void Start()
        {
            ExceptionHandle.SetupExceptionHandling();

            if (SceneTransporter.CurrentGame == null)
            {
                SceneManager.LoadScene("Scene/TestScene");
                return;
            }


            State = GameManagerState.Unknown;

            StartCoroutine(Init());
        }

        public void LateUpdate()
        {
            Channel.Flush();
        }

        public void Update()
        {
            if (Handlers.ContainsKey(State))
            {
                var hand = Handlers[State];
                if (hand != null)
                {
                    foreach (var handler in hand)
                    {
                        handler.OnUnityUpdate();
                    }
                }
            }
        }

        private IEnumerator Init()
        {
            yield return 0;
            Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
            yield break;
        }

        #endregion
        
        public bool SwitchDisplayingBoardNo(int no)
        {
            CurrentDisplayingBoardNo = no;
            Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
            return true;
        }

        #region 决定游戏状态
        
        /// <summary>
        /// 尝试针对当前面板内容确定当前状态
        /// </summary>
        private void VerifyState()
        {
            GameManagerState newState;

            if (CurrentGame.PossibleActions.Count == 0)
            {
                newState = GameManagerState.OutOfMyTurn;
            }
            else
            {
                switch (CurrentGame.CurrentPhase)
                {
                    case TtaPhase.PoliticalPhase:
                        newState = GameManagerState.PoliticalPhaseIdle;
                        break;
                    case TtaPhase.ActionPhase:
                        newState = GameManagerState.ActionPhaseIdle;
                        break;
                    case TtaPhase.EventResolution:
                        newState = GameManagerState.ResolveEvent;
                        break;
                    case TtaPhase.DiscardPhase:

                    default:
                        newState = State;
                        break;
                }
            }

            if (newState != State)
            {
                SwitchState(newState, null);
            }
        }

        #endregion
        
        
        public void SwitchState(GameManagerState newState, Dictionary<string, System.Object> stateData)
        {
            if (Handlers.ContainsKey(State))
            {
                var hand = Handlers[State];
                if (hand != null)
                {
                    foreach (var handler in hand)
                    {
                        handler.LeaveState();
                    }
                }
            }

            State = newState;

            StateData.Clear();
            if (stateData != null)
            {
            foreach (var o in stateData)
            {
                StateData.Add(o.Key,o.Value);
                }
            }

            if (Handlers.ContainsKey(State))
            {
                var hand = Handlers[State];
                if (hand != null)
                {
                    foreach (var handler in hand)
                    {
                        handler.EnteringState();
                    }
                }
            }
        }
    }


    public enum GameManagerState
    {
        Unknown,
        PoliticalPhaseIdle,
        ResolveEvent, DefendAggression,Colonize,
        ActionPhaseIdle, ActionPhaseChooseTarget, ActionPhaseInternalQuery,
        DiscardPhaseIdle,
        OutOfMyTurn,
    }
}
