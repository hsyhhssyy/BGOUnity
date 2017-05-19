using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class ActionPhaseIdleStateHandler: GameBoardStateHandler
    {
        public override void EnteringState()
        {
            
        }

        public override void LeaveState()
        {
            
        }

        public override void OnUnityUpdate()
        {
            
        }

        public override void ProcessGameEvents(System.Object sender, GameUIEventArgs args)
        {
            #region ProcessActionPhaseIdleStateEvents - TrySelect
            if (args.EventType == GameUIEventType.TrySelect)
            {
                //在空闲状态，允许点击的UI元素如下
                if (args.UIKey.Contains("CardRow"))
                {
                    //卡牌列上的卡
                    //要看是什么张卡，白点够不够
                    var action =
                        CurrentGame.PossibleActions.FirstOrDefault(a => (
                            a.ActionType == PlayerActionType.TakeCardFromCardRow ||
                            a.ActionType == PlayerActionType.PutBackCard) && (
                                Convert.ToInt32(a.Data[1]) ==
                                (int)
                                    args.AttachedData[
                                        "Position"]));

                    if (action != null)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    //看有没有相关的Action，没有就不让选
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("BuildingsPopupMenuItem"))
                {
                    //Building的MenuItem都是可选的Action
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("ConstructingWonderBox"))
                {
                    //在建造中的奇迹
                    List<PlayerAction> acceptedActions =
                    CurrentGame.PossibleActions.Where(
                        action =>
                            action.ActionType == PlayerActionType.BuildWonder).ToList();

                    acceptedActions.Sort((a, b) => ((int)a.Data[1]).CompareTo(b.Data[0]));

                    if (acceptedActions.Count > 0)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("ConstructingWonderMenuItem"))
                {
                    //Wonder的MenuItem只要拉得出来都是可选的Action
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板（要看人数）
                    var playerNo = (int)args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
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
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("HandMilitaryCard"))
                {
                    var card = args.AttachedData["Card"] as CardInfo;
                    if (CurrentGame.PossibleActions.FirstOrDefault(
                        action =>
                            action.ActionType == PlayerActionType.SetupTactic && action.Data[0] as CardInfo == card) !=
                        null)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //按钮
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            #endregion

            #region ProcessActionPhaseIdleStateEvents - Selected
            if (args.EventType == GameUIEventType.Selected)
            {
                //如果玩家在空闲状态下，选中不同的目标
                //TODO 这里省略判断，默认合法（理论上上面TrySelect的内容应该留在这里判断）
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
                    Channel.Broadcast(msg);
                }
                else if (args.UIKey.Contains("WorkerBank"))
                {
                    //人力库（要看粮食）

                    var action =
                        CurrentGame.PossibleActions.FirstOrDefault(
                            a => a.ActionType == PlayerActionType.IncreasePopulation);

                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);
                    Channel.Broadcast(msg);

                }
                else if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    var card = (CardInfo)args.AttachedData["Card"];
                    //看有没有相关的Action，没有就不让选
                    List<PlayerAction> acceptedActions = new List<PlayerAction>();

                    foreach (var action in CurrentGame.PossibleActions)
                    {
                        if (action.ActionType == PlayerActionType.BuildBuilding &&
                            ((CardInfo)action.Data[0]).InternalId == card.InternalId)
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
                    Channel.Broadcast(msg);
                }
                else if (args.UIKey.Contains("BuildingsPopupMenuItem"))
                {
                    var action = args.AttachedData["Action"] as PlayerAction;
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);

                    if (action == null)
                    {
                        return;
                    }

                    if (action.Internal)
                    {
                        //出错了，建筑面板不可能有InternalAction
                        Debug.Log("Error: BuildingsPopupMenuItem has Internal Action");
                    }
                    else
                    {
                        Channel.Broadcast(msg);
                    }
                }
                else if (args.UIKey.Contains("ConstructingWonderBox"))
                {
                    //在建造中的奇迹
                    List<PlayerAction> acceptedActions =
                    CurrentGame.PossibleActions.Where(
                        action =>
                            action.ActionType == PlayerActionType.BuildWonder).ToList();

                    acceptedActions.Sort((a, b) => ((int)a.Data[1]).CompareTo(b.Data[0]));

                    if (acceptedActions.Count > 0)
                    {
                        var msg = new ManagerGameUIEventArgs(GameUIEventType.PopupMenu, args.UIKey);
                        msg.AttachedData.Add("Actions", acceptedActions);
                        Channel.Broadcast(msg);
                    }
                }
                else if (args.UIKey.Contains("ConstructingWonderMenuItem"))
                {
                    var action = args.AttachedData["Action"] as PlayerAction;
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);

                    if (action == null)
                    {
                        return;
                    }

                    if (action.Internal)
                    {
                        //出错了，Wonder不可能有InternalAction
                        Debug.Log("Error: BuildingsPopupMenuItem has Internal Action");
                    }
                    else
                    {
                        Channel.Broadcast(msg);
                    }
                }
                else if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板
                    var playerNo = (int)args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {

                        Manager.SwitchDisplayingBoardNo(playerNo);
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
                        Channel.Broadcast(msg);
                    }
                    else if (args.UIKey.Contains("EndActionPhase"))
                    {
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.EndActionPhase);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                        msg.AttachedData.Add("PlayerAction", action);
                        Channel.Broadcast(msg);
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
                        Manager.SwitchState(GameManagerState.ActionPhaseInternalQuery,
                            new Dictionary<String, System.Object>()
                            {
                                {ActionPhaseChooseTargetStateHandler.StateNameTriggerCard, args.AttachedData["Card"] as CardInfo},
                                {ActionPhaseChooseTargetStateHandler.StateNameTriggerAction, action}
                            });
                    }
                    else
                    {
                        Channel.Broadcast(msg);
                    }
                }
                else if (args.UIKey.Contains("HandMilitaryCard"))
                {
                    var card = args.AttachedData["Card"] as CardInfo;
                    var cardAction = CurrentGame.PossibleActions.FirstOrDefault(
                        action =>
                            action.ActionType == PlayerActionType.SetupTactic && action.Data[0] as CardInfo == card);

                    if (cardAction == null)
                    {
                        return;
                    }

                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", cardAction);

                    if (cardAction.Internal)
                    {
                        LogRecorder.Log("Error HandMilitaryCard Internal Action");
                    }
                    else
                    {
                        Channel.Broadcast(msg);
                    }
                }

            }
            #endregion
        }


        public ActionPhaseIdleStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
