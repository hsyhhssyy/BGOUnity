using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
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
                    StateData["PoliticalIdle-CardSelected"] = args;
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.SelectionActive, args.UIKey));
                }
                else if (args.UIKey.Contains("Player"))
                {
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
                        var action =
                            CurrentGame.PossibleActions.FirstOrDefault(
                                a => a.ActionType == PlayerActionType.Aggression && card == (CardInfo)a.Data[0]);

                        var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");

                        if (StateData.ContainsKey("PoliticalIdle-Player"))
                        {
                            var playerNo =
                                (int)((GameUIEventArgs)StateData["PoliticalIdle-Player"]).AttachedData["PlayerNo"];
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
                        Channel.Broadcast(msg);
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
