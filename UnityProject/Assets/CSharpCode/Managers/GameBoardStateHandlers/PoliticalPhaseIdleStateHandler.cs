using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    /// <summary>
    /// 进入政治行动，玩家可以自由进行选择时的Controller
    /// </summary>
    public class PoliticalPhaseIdleStateHandler:GameBoardStateHandler
    {
        public override void EnteringState()
        {
            
        }

        public override void LeaveState()
        {
        }

        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
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
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("Player"))
                {
                    //是玩家名字
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
                else if (args.UIKey.Contains("DialogButton"))
                {
                    //是按钮
                    //这里不一定能按
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Card"))
                {
                    //点的是卡
                    if (StateData.ContainsKey("PoliticalIdle-CardSelected"))
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionDeactive,
                            ((GameUIEventArgs)StateData["PoliticalIdle-CardSelected"]).UIKey));
                    }
                    //根据选的是什么卡来决定是否要亮起玩家选择面板
                    StateData["PoliticalIdle-CardSelected"] = args;
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionActive, args.UIKey));
                }
                else if (args.UIKey.Contains("Player"))
                {
                    //现在不会有玩家名字了，这个改为独立弹窗
                    //是玩家名字
                    if (StateData.ContainsKey("PoliticalIdle-Player"))
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionDeactive,
                            ((GameUIEventArgs)StateData["PoliticalIdle-Player"]).UIKey));
                    }
                    StateData["PoliticalIdle-Player"] = args;
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionActive, args.UIKey));
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
                        Channel.Broadcast(msg);
                    }
                    else if (args.UIKey.Contains("Confirm"))
                    {
                        var card =
                            (CardInfo)((GameUIEventArgs)StateData["PoliticalIdle-CardSelected"]).AttachedData["Card"];
                        
                        //检查卡牌类型，如果是War，或者Aggression，或者Pact，要有独特的处理手段
                        if (card.CardType == CardType.Aggression || card.CardType == CardType.War)
                        {
                            Manager.SwitchState(GameManagerState.SelectTargetPlayer, new Dictionary<string, object>()
                            {
                                {SelectTargetPlayerStateHandler.SourceCardStateDataKey,card }
                            });
                        }
                        else
                        {
                            var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");

                            PlayerAction action = null;
                                CurrentGame.PossibleActions.FirstOrDefault(
                               a => (a.ActionType == PlayerActionType.PlayColony || a.ActionType == PlayerActionType.PlayEvent)
                               && card == (CardInfo)a.Data[0]);

                            msg.AttachedData.Add("PlayerAction", action);
                            Channel.Broadcast(msg);
                        }
                        /*
                        if (card.CardType== CardType.Aggression)
                        {
                            var playerNo =
                                (int) ((GameUIEventArgs) StateData["PoliticalIdle-Player"]).AttachedData["PlayerNo"];
                            action =
                                CurrentGame.PossibleActions.FirstOrDefault(
                                    a =>a.ActionType== PlayerActionType.Aggression&&
                                        String.Equals(a.Data[1].ToString(), CurrentGame.Boards[playerNo].PlayerName,
                                            StringComparison.CurrentCultureIgnoreCase));
                            if (action == null)
                            {
                                LogRecorder.Log("No Such Aggression");
                                return;
                            }

                        }
                        else
                        {
                           
                        }*/

                    }
                }
            }
        }

        public override void OnUnityUpdate()
        {
        }

        public PoliticalPhaseIdleStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
