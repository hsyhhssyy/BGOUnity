using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Assets.CSharpCode.Managers
{
    public class GameBoardManager : MonoBehaviour
    {
        public event EventHandler<GameUIEventArgs> GameBoardManagerEvent;
        
        public void Regiseter(TtaUIControllerMonoBehaviour controller)
        {
            controller.GameBoardControllerEvent += OnSubscribedGameEvents;
        }

        private void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            switch (State)
            {
                case GameManagerState.PoliticalPhaseIdle:
                    ProcessPoliticalPhaseIdleStateEvents(sender, args);
                    break;
                case GameManagerState.ActionPhaseIdle:
                    ProcessActionPhaseIdleStateEvents(sender, args);
                    break;
                case GameManagerState.Unknown:
                default:
                    ProcessUnknownStateEvents(sender, args);
                    break;
            }
        }

        private void Broadcast(GameUIEventArgs args)
        {
            if (GameBoardManagerEvent != null)
            {
                GameBoardManagerEvent(this, args);
            }
        }

        [UsedImplicitly]
        void Start()
        {
            ExceptionHandle.SetupExceptionHandling();

            if (SceneTransporter.CurrentGame == null)
            {
                SceneManager.LoadScene("Scene/TestScene");
                return;
            }

            State = GameManagerState.Unknown;
            StartCoroutine(RefreshRoutine());
        }

        private IEnumerator RefreshRoutine()
        {
            yield return 0;
            Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "PCBoard"));

            StartCoroutine(SceneTransporter.Server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                CurrentDisplayingBoardNo = SceneTransporter.CurrentGame.MyPlayerIndex;

                var msg = new ManagerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard");
                Broadcast(msg);
            }));
            yield break;
        }

        #region 一系列复杂的用于维护当前游戏状态机的变量

        public TtaGame CurrentGame { get { return SceneTransporter.CurrentGame; } }
        public int CurrentDisplayingBoardNo = 0;

        public GameManagerState State;
        public Dictionary<String, System.Object> StateData;

        #endregion

        #region 决定游戏初始状态

        private void ProcessUnknownStateEvents(System.Object sender, GameUIEventArgs args)
        {
            //只有在刷新时才会出现，否则定时请求刷新
            if (args.EventType == GameUIEventType.Refresh)
            {
                switch (CurrentGame.CurrentPhase)
                {
                        case TtaPhase.PoliticalPhase:
                        State=GameManagerState.PoliticalPhaseIdle;
                        break;
                        case TtaPhase.ActionPhase:
                        State = GameManagerState.ActionPhaseIdle;
                        break;
                        case TtaPhase.DiscardPhase:

                    default:
                        State= GameManagerState.Unknown;
                        break;
                }
            }
        }

        #endregion

        #region ActionPhase Idle State

        private void ProcessActionPhaseIdleStateEvents(System.Object sender, GameUIEventArgs args)
        {
            if (args.EventType == GameUIEventType.TrySelect)
            {
                //在空闲状态，允许点击的UI元素如下
                if (args.UIKey.Contains("CardRow"))
                {
                    //卡牌列上的卡
                    //要看是什么张卡，白点够不够
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect,args.UIKey));
                }else
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    //看有没有相关的Action，没有就不让选
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }else
                if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else
                if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板（要看人数）
                    var playerNo = (int) args.AttachedData["PlayerNo"];
                    if(CurrentGame.Boards.Count> playerNo)
                    {
                        Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else
                if (args.UIKey.Contains("HandCivilCard") ||
                    args.UIKey.Contains("HandMilitaryCard")
                    )
                {
                    //手牌（要看手牌内容）
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            if (args.EventType == GameUIEventType.Selected)
            {
                //如果玩家在空闲状态下，选中不同的目标
                //这里省略判断，默认合法
                if (args.UIKey.Contains("CardRow"))
                {
                    //卡牌列上的卡
                    //通知界面暗转等待网络
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "AllComponent"));

                    //第一步，把要的action找出来

                    var action =
                        CurrentGame.PossibleActions.FirstOrDefault(a => (
                            a.ActionType == PlayerActionType.TakeCardFromCardRow ||
                            a.ActionType == PlayerActionType.PutBackCard) && (
                                Convert.ToInt32(a.Data[1]) ==
                                (int)
                                    args.AttachedData[
                                        "Position"]));

                    //第二步，发消息给server，take action
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "TtaServer");
                    msg.AttachedData.Add("PlayerAction",action);
                    Broadcast(msg);
                }
                else
                if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）
                    Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "AllComponent"));

                    var action = CurrentGame.PossibleActions.FirstOrDefault(a => a.ActionType == PlayerActionType.IncreasePopulation);

                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "TtaServer");
                    msg.AttachedData.Add("PlayerAction", action);
                    Broadcast(msg);

                }else
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    var card = (CardInfo) args.AttachedData["Card"];
                    //看有没有相关的Action，没有就不让选
                    List<PlayerAction> acceptedActions = new List<PlayerAction>();

                    foreach (var action in CurrentGame.PossibleActions)
                    {
                        if (action.ActionType == PlayerActionType.BuildBuilding &&
                            ((CardInfo)action.Data[0]).InternalId ==card.InternalId)
                        {
                            acceptedActions.Add(action);
                        }

                        if (action.ActionType == PlayerActionType.UpgradeBuilding)
                        {
                            if (((CardInfo)action.Data[0]).InternalId == card.InternalId ||
                                ((CardInfo)action.Data[1]).InternalId == card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }
                        if (action.ActionType == PlayerActionType.Disband ||
                            action.ActionType == PlayerActionType.Destory)
                        {
                            if (((CardInfo)action.Data[0]).InternalId == card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }


                    }
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.PopupMenu, args.UIKey);
                    msg.AttachedData.Add("Actions", acceptedActions);
                    Broadcast(msg);
                }
                else
                if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板
                    var playerNo = (int)args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {

                        CurrentDisplayingBoardNo = playerNo;
                        Broadcast(new ManagerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
                    }
                }

            }
        }

        #endregion

        #region PoliticalPhase Idle State

        private void ProcessPoliticalPhaseIdleStateEvents(System.Object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains("PoliticalDialog"))
            {
                return;
            }

            //内政Idle中，能选中的Dialog就能点击
            //
        }

        #endregion

    }


    public enum GameManagerState
    {
        Unknown,
        PoliticalPhaseIdle,ResolveAction,DefendAggression,Colonize,
        ActionPhaseIdle,
        DiscardPhaseIdle,
    }
}
