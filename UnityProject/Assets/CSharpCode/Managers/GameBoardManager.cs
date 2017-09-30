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
using Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists;
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
        /// <summary>
        /// 表示当前程序激活的Manager（或激活的多个Manager中的随机一个）
        /// </summary>
        public static GameBoardManager ActiveManager { get; private set; }

        public int CurrentDisplayingBoardNo = 0;

        public GameManagerState State { get; private set; }
        public Dictionary<String, System.Object> StateData { get; private set; }

        public Dictionary<GameManagerState, List<GameBoardStateHandler>> Handlers; 

        public GameEventChannel Channel { get; private set; }

        public NetworkManager NetworkManager;


        #endregion

        public GameBoardManager()
        {
            StateData = new Dictionary<string, object>();

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
                },

                {
                    GameManagerState.PoliticalPhaseIdle, new List<GameBoardStateHandler>
                    {
                        new PoliticalPhaseIdleStateHandler(this)
                    }
                },

                {
                    GameManagerState.ResolveEvent, new List<GameBoardStateHandler>
                    {
                        new ResolveEventStateHandler(this)
                    }
                },
                {
                    GameManagerState.Colonize, new List<GameBoardStateHandler>
                    {
                        new ResolveEventStateHandler(this)
                    }
                },
                {
                    GameManagerState.SendColonists, new List<GameBoardStateHandler>
                    {
                        new SendColonistsStateHandler(this)
                    }
                },
                {
                    GameManagerState.SelectTargetPlayer, new List<GameBoardStateHandler>
                    {
                        new SelectTargetPlayerStateHandler(this)
                    }
                },
            };

            if (ActiveManager != null)
            {
                //销毁上一个Manager
            }

            ActiveManager = this;
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
                    case TtaPhase.Colonize:
                        newState = GameManagerState.Colonize;
                        break;
                    case TtaPhase.SendColonists:
                        newState = GameManagerState.SendColonists;
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
        
        /// <summary>
        /// 外部程序调用该方法，即可立刻切换当前状态。[注意]如果触发刷新页面操作，状态可能会被重置。
        /// stateDate参数内的内容，会被新的状态全部继承。
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="stateData"></param>
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

            StateData=new Dictionary<string, object>();
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
        ResolveEvent, DefendAggression,Colonize,SendColonists,
        ActionPhaseIdle, ActionPhaseChooseTarget, ActionPhaseInternalQuery,
        DiscardPhaseIdle,
        OutOfMyTurn,
        SelectTargetPlayer
    }
}
