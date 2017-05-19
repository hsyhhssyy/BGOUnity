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
    public class ActionPhaseChooseTargetStateHandler:GameBoardStateHandler
    {
        public const String StateNameTriggerCard = "ActionPhaseChooseTarget.TriggerCard";
        public const String StateNameTriggerAction = "ActionPhaseChooseTarget.TriggerAction";
        public const String StateNameAvailableActions = "ActionPhaseChooseTarget.AvailableActions";
        
        public override void EnteringState()
        {
            //确定返回的事件
            var actions = StateData[StateNameAvailableActions] as List<PlayerAction>;
            var triggerCard = StateData[StateNameTriggerCard] as CardInfo;

            if (triggerCard == null|| actions==null)
            {
                LogRecorder.Log("Crash!");
                return;
            }
            
            var board = CurrentGame.Boards[CurrentGame.MyPlayerIndex];
            
            var targetElements = new Dictionary<String, List<CardInfo>>();
            targetElements["HandCivilCard"] = new List<CardInfo>();
            targetElements["BuildingCell"] = new List<CardInfo>();

            targetElements["HandCivilCard"].Add(triggerCard);
            

            //1. 突破(研究科技后改变[0]类型的属性[1]点）
            var cards =
                board.CivilCards.Where(
                    card =>
                        actions.Exists(
                            action =>
                                action.ActionType == PlayerActionType.DevelopTechCard &&
                                (action.Data[0] as CardInfo).InternalId == card.InternalId)).ToList();

            targetElements["HandCivilCard"].AddRange(cards);
            
            
            //建造资源建筑减少资源点消耗[0]
            //建造城市建筑减少资源点消耗[0]
            cards =
                actions.Where(action => action.ActionType == PlayerActionType.BuildBuilding)
                    .Select(act => act.Data[0] as CardInfo)
                    .ToList();
            targetElements["BuildingCell"] .AddRange(cards);
            
            //建造资源建筑减少资源点消耗[0]
            //建造城市建筑减少资源点消耗[0]
            cards =
                actions.Where(action => action.ActionType == PlayerActionType.UpgradeBuilding)
                    .Select(act => act.Data[0] as CardInfo)
                    .ToList();
            targetElements["BuildingCell"].AddRange(cards);
            

            StateData["HighlightElement"] = targetElements;
        }

        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
        {
            var actions = StateData[StateNameAvailableActions] as List<PlayerAction>;
            var triggerCard = StateData["ActionPhaseChooseTarget.TriggerCard"] as CardInfo;
            var highlightElement = StateData["HighlightElement"] as Dictionary<String, List<CardInfo>>;
            if (highlightElement == null)
            {
                return;
            }

            if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    if (highlightElement["BuildingCell"].Contains(args.AttachedData["Card"] as CardInfo))
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("BuildingsPopupMenuItem"))
                {
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("HandCivilCard"))
                {
                    //手牌（要看手牌内容）
                    //注意这里总是包括原卡牌，表示取消操作
                    if (highlightElement["HandCivilCard"].Contains(args.AttachedData["Card"] as CardInfo))
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("BuildingCell"))
                {
                    //建筑列表
                    List<PlayerAction> acceptedActions = new List<PlayerAction>();
                    var card = args.AttachedData["Card"] as CardInfo;

                    foreach (var action in actions)
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

                    if (acceptedActions.Count >0)
                    {
                        var msg = new ManagerGameUIEventArgs(GameUIEventType.PopupMenu, args.UIKey);
                        msg.AttachedData.Add("Actions", acceptedActions);
                        Channel.Broadcast(msg);
                    }
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
                else if (args.UIKey.Contains("HandCivilCard"))
                {
                    //手牌（要看手牌内容）
                    var card = args.AttachedData["Card"] as CardInfo;
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");

                    //如果是点击了原卡牌
                    if (args.AttachedData["Card"] as CardInfo == triggerCard)
                    {
                        Manager.SwitchState(GameManagerState.ActionPhaseIdle, null);
                        return;
                    }
                    
                    var action = actions.FirstOrDefault(act => act.ActionType == PlayerActionType.DevelopTechCard &&
                                                               (act.Data[0] as CardInfo).InternalId == card.InternalId);
                    msg.AttachedData.Add("PlayerAction", action);
                    Channel.Broadcast(msg);
                }
            }
        }

        

        public override void LeaveState()
        {
        }
        

        public override void OnUnityUpdate()
        {
        }

        public ActionPhaseChooseTargetStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
