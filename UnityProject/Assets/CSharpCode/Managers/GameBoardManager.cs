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
        private readonly GameEventChannel _channel;

        public NetworkManager NetworkManager;

        public GameBoardManager()
        {
            _channel = SceneTransporter.CurrentChannel;
            _channel.GameEvent += DestoryNullReference;
            
            NetworkManager = new NetworkManager(this, _channel);

        }

        #region 接受和分发消息的部分

        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        private void DestoryNullReference(System.Object sender, GameUIEventArgs args)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this == null)
            {
                _channel.GameEvent -= DestoryNullReference;
            }
            else
            {
                OnSubscribedUIControllerEvents(sender, args);
            }
        }

        public void Regiseter(TtaUIControllerMonoBehaviour controller)
        {
            controller.Channel = _channel;

            LogRecorder.Log(controller.name + " Registed");
        }

        private void OnSubscribedUIControllerEvents(System.Object sender, GameUIEventArgs args)
        {
            switch (State)
            {
                case GameManagerState.PoliticalPhaseIdle:
                    ProcessPoliticalPhaseIdleStateEvents(sender, args);
                    break;
                case GameManagerState.ActionPhaseIdle:
                    ProcessActionPhaseIdleStateEvents(sender, args);
                    break;
                case GameManagerState.ResolveEvent:
                    ProcessResolveEventStateEvents(sender, args);
                    break;
                case GameManagerState.OutOfMyTurn:
                    ProcessOutOfMyTurnStateEvents(sender, args);
                    break;
                    case GameManagerState.ActionPhaseChooseTarget:
                    ProcessActionPhaseChooseTargetStateEvents(sender, args);
                    break;
                case GameManagerState.Unknown:
                default:
                    //只有在刷新时才会出现，否则定时请求刷新
                    if (args.EventType == GameUIEventType.Refresh)
                    {
                        VerifyState();
                    }
                    break;
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
            _channel.Flush();
        }

        public void Update()
        {
            switch (State)
            {
                case GameManagerState.OutOfMyTurn:
                    UpdateOnOutOfMyTurnState();
                    break;
                case GameManagerState.Unknown:
                default:
                    break;
            }
        }

        private IEnumerator Init()
        {
            yield return 0;
            _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
            yield break;
        }

        #endregion

        #region 一系列复杂的用于维护当前游戏状态机的变量

        public TtaGame CurrentGame
        {
            get { return SceneTransporter.CurrentGame; }
        }

        public int CurrentDisplayingBoardNo = 0;

        public GameManagerState State;
        public Dictionary<String, System.Object> StateData = new Dictionary<string, object>();

        #endregion

        #region 决定游戏状态
        
        public void VerifyState()
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
                        newState = GameManagerState.Unknown;
                        break;
                }
            }

            if (newState != State)
            {
                State = newState;
                switch (State)
                {
                    case GameManagerState.ActionPhaseChooseTarget:
                        EnteringActionPhaseChooseTargetState();
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
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    //看有没有相关的Action，没有就不让选
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板（要看人数）
                    var playerNo = (int) args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("HandCivilCard")
                    )
                {
                    //手牌（要看手牌内容）
                    var card = args.AttachedData["Card"] as CardInfo;
                    if (CurrentGame.PossibleActions.FirstOrDefault(
                        action => (
                            action.ActionType == PlayerActionType.DevelopTechCard ||
                            action.ActionType == PlayerActionType.PlayActionCard ||
                            action.ActionType == PlayerActionType.Revolution ||
                            action.ActionType == PlayerActionType.ElectLeader) && (action.Data[0] as CardInfo == card)) !=
                        null)
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }else if (args.UIKey.Contains("HandMilitaryCard"))
                {
                    var card = args.AttachedData["Card"] as CardInfo;
                    if (CurrentGame.PossibleActions.FirstOrDefault(
                        action =>
                            action.ActionType == PlayerActionType.SetupTactic && action.Data[0] as CardInfo == card) !=
                        null)
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //按钮
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            if (args.EventType == GameUIEventType.Selected)
            {
                //如果玩家在空闲状态下，选中不同的目标
                //这里省略判断，默认合法
                if (args.UIKey.Contains("CardRow"))
                {
                    //卡牌列上的卡

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
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);
                    _channel.Broadcast(msg);
                }
                else if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）

                    var action =
                        CurrentGame.PossibleActions.FirstOrDefault(
                            a => a.ActionType == PlayerActionType.IncreasePopulation);

                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);
                    _channel.Broadcast(msg);

                }
                else if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    var card = (CardInfo) args.AttachedData["Card"];
                    //看有没有相关的Action，没有就不让选
                    List<PlayerAction> acceptedActions = new List<PlayerAction>();

                    foreach (var action in CurrentGame.PossibleActions)
                    {
                        if (action.ActionType == PlayerActionType.BuildBuilding &&
                            ((CardInfo) action.Data[0]).InternalId == card.InternalId)
                        {
                            acceptedActions.Add(action);
                        }

                        if (action.ActionType == PlayerActionType.UpgradeBuilding)
                        {
                            if (((CardInfo) action.Data[0]).InternalId == card.InternalId ||
                                ((CardInfo) action.Data[1]).InternalId == card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }
                        if (action.ActionType == PlayerActionType.Disband ||
                            action.ActionType == PlayerActionType.Destory)
                        {
                            if (((CardInfo) action.Data[0]).InternalId == card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }


                    }
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.PopupMenu, args.UIKey);
                    msg.AttachedData.Add("Actions", acceptedActions);
                    _channel.Broadcast(msg);
                }
                else if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板
                    var playerNo = (int) args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {

                        CurrentDisplayingBoardNo = playerNo;
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
                    }
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //按钮
                    if (args.UIKey.Contains("ResetActionPhase"))
                    {
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.ResetActionPhase);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                        msg.AttachedData.Add("PlayerAction", action);
                        _channel.Broadcast(msg);
                    }
                    else if (args.UIKey.Contains("EndActionPhase"))
                    {
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.EndActionPhase);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                        msg.AttachedData.Add("PlayerAction", action);
                        _channel.Broadcast(msg);
                    }
                }
                else if (args.UIKey.Contains("HandCivilCard"))
                {
                    //手牌（要看手牌内容）
                    var handCardActions = CurrentGame.PossibleActions.Where(
                        a =>
                            a.ActionType == PlayerActionType.DevelopTechCard ||
                            a.ActionType == PlayerActionType.PlayActionCard ||
                            a.ActionType == PlayerActionType.Revolution ||
                            a.ActionType == PlayerActionType.ElectLeader).ToList();
                    var action = handCardActions.FirstOrDefault(
                        a => a.Data[0] as CardInfo == args.AttachedData["Card"] as CardInfo);

                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);

                    if (action == null)
                    {
                        return;
                    }

                    if (action.Internal)
                    {
                        LogRecorder.Log("Go ActionPhaseChooseTarget");
                        State = GameManagerState.ActionPhaseChooseTarget;
                        StateData["ActionPhaseChooseTarget.TriggerCard"] = args.AttachedData["Card"] as CardInfo;
                        StateData["ActionPhaseChooseTarget.TriggerAction"] = action;
                        EnteringActionPhaseChooseTargetState();
                    }
                    else
                    {
                        _channel.Broadcast(msg);
                    }
                }

            }
        }

        #endregion

        #region ActionPhase Choose Action Target

        private void EnteringActionPhaseChooseTargetState()
        {
            //进入该事件
            var board = CurrentGame.Boards[CurrentGame.MyPlayerIndex];

            //确定可选组件

            var triggerCard = StateData["ActionPhaseChooseTarget.TriggerCard"] as CardInfo;
            if (triggerCard == null)
            {
                LogRecorder.Log("Crash!");
                return;
            }

            var targetElements = new Dictionary<String, List<CardInfo>>();
            if (triggerCard.ImmediateEffects.FirstOrDefault(e=> e.FunctionId== CardEffectType.DevelopATechThenChangeYofResourceX)!=null)
            {
                //1. 突破(研发一项科技后返还X科技）
                var cards = new List<CardInfo>();
                foreach (var card in board.CivilCards)
                {
                    if (card.ResearchCost != null && card.ResearchCost.Count > 0)
                    {
                        if (card.ResearchCost[0] <= board.Resource[ResourceType.Science])
                        {
                            cards.Add(card);
                        }
                    }
                }

                cards.Add(triggerCard);
                targetElements["HandCivilCard"] = cards;
            }
            

            //
            StateData["HighlightElement"] = targetElements;
        }

        private void ProcessActionPhaseChooseTargetStateEvents(System.Object sender, GameUIEventArgs args)
        {
            var triggerCard = StateData["ActionPhaseChooseTarget.TriggerCard"] as CardInfo;
            var highlightElement=StateData["HighlightElement"] as Dictionary<String, List<CardInfo>>;
            if (highlightElement == null)
            {
                return;
            }

            if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                }
                else if (args.UIKey.Contains("HandCivilCard"))
                {
                    //手牌（要看手牌内容）
                    if (highlightElement.ContainsKey("HandCivilCard") &&
                        highlightElement["HandCivilCard"].Contains(args.AttachedData["Card"] as CardInfo))
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("HandCivilCard"))
                {
                    //手牌（要看手牌内容）
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    
                    //如果是点击了同样的卡
                    if (args.AttachedData["Card"] as CardInfo == triggerCard)
                    {
                        State= GameManagerState.ActionPhaseIdle;
                        VerifyState();
                        return;
                    }
                    
                    //Action来自于TriggeringCard


                    var action = StateData["ActionPhaseChooseTarget.TriggerAction"] as PlayerAction;
                    msg.AttachedData.Add("PlayerAction", action);
                    msg.AttachedData.Add("AdditionalCard1", args.AttachedData["Card"]);
                    _channel.Broadcast(msg);
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
            if (args.EventType == GameUIEventType.TrySelect)
            {
                //内政Idle中，能选中的Dialog就能点击
                if (args.UIKey.Contains("Card"))
                {
                    //点的是卡
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("Player"))
                {
                    //是玩家名字
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //是按钮
                    //这里不一定能按
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Card"))
                {
                    //点的是卡
                    if (StateData.ContainsKey("PoliticalIdle-CardSelected"))
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionDeactive,
                            ((GameUIEventArgs) StateData["PoliticalIdle-CardSelected"]).UIKey));
                    }
                    StateData["PoliticalIdle-CardSelected"] = args;
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionActive, args.UIKey));
                }
                else if (args.UIKey.Contains("Player"))
                {
                    //是玩家名字
                    if (StateData.ContainsKey("PoliticalIdle-Player"))
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionDeactive,
                            ((GameUIEventArgs) StateData["PoliticalIdle-Player"]).UIKey));
                    }
                    StateData["PoliticalIdle-Player"] = args;
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionActive, args.UIKey));
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //是按钮
                    //这里不一定能按
                    //看是什么Button
                    if (args.UIKey.Contains("PassPoliticalPhase"))
                    {
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.PassPoliticalPhase);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                        msg.AttachedData.Add("PlayerAction", action);
                        _channel.Broadcast(msg);
                    }
                    else if (args.UIKey.Contains("Confirm"))
                    {
                        var card =
                            (CardInfo) ((GameUIEventArgs) StateData["PoliticalIdle-CardSelected"]).AttachedData["Card"];
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.Aggression && card == (CardInfo) a.Data[0]);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");

                        if (StateData.ContainsKey("PoliticalIdle-Player"))
                        {
                            var playerNo =
                                (int) ((GameUIEventArgs) StateData["PoliticalIdle-Player"]).AttachedData["PlayerNo"];
                            action =
                                CurrentGame.PossibleActions.FirstOrDefault(
                                    a =>
                                        String.Equals(a.Data[1].ToString(), CurrentGame.Boards[playerNo].PlayerName,
                                            StringComparison.CurrentCultureIgnoreCase));
                            if (action == null)
                            {
                                LogRecorder.Log("No Such Aggression");
                                return;
                            }

                        }
                        msg.AttachedData.Add("PlayerAction", action);
                        _channel.Broadcast(msg);
                    }
                }
            }
        }

        #endregion

        #region Resolve Event State

        private void ProcessResolveEventStateEvents(System.Object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains("ResolveEventDialog"))
            {
                return;
            }
            if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("Option"))
                {
                    //点的是选项
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));

                    //当然可以啦
                }
            }else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Option"))
                {
                    var action =
                               CurrentGame.PossibleActions.FirstOrDefault(
                                   a =>
                                      a.Data[1]== args.AttachedData["Data"]);
                    //点的是选项
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);
                    _channel.Broadcast(msg);
                }
            }
        }

        #endregion

        #region OutOfMyTurn
        
        private void UpdateOnOutOfMyTurnState()
        {
            if(!StateData.ContainsKey("Toggle")||(bool)StateData["Toggle"] == false)
            {
                return;
            }

            float refreshIntervel = (float)StateData["Progress"];

            refreshIntervel -= Time.deltaTime;

            if (refreshIntervel < 0)
            {
                refreshIntervel = 0;
            }

            StateData["Progress"] = refreshIntervel;

            if (refreshIntervel <= 0)
            {
                StateData["Toggle"] = false;
                //Refresh
                _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
            }
        }

        private void ProcessOutOfMyTurnStateEvents(System.Object sender, GameUIEventArgs args)
        {
            //Refresh以后仍然处于这个状态
            if (args.EventType == GameUIEventType.Refresh)
            {
                StateData["Progress"] = 30f;
                StateData["Toggle"] = true;
            }
            else if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板（要看人数）
                    var playerNo = (int) args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if(args.UIKey.Contains("ForceRefreshButton"))
                {
                  _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板
                    var playerNo = (int) args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        CurrentDisplayingBoardNo = playerNo;
                        _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
                    }
                }else if (args.UIKey.Contains("ForceRefreshButton"))
                {
                    StateData["Toggle"] = false;
                    //Refresh
                    _channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
                }
            }
        }

        #endregion

        }


    public enum GameManagerState
    {
        Unknown,
        PoliticalPhaseIdle,
        ResolveEvent, DefendAggression,Colonize,
        ActionPhaseIdle, ActionPhaseChooseTarget,
        DiscardPhaseIdle,
        OutOfMyTurn,
    }
}
