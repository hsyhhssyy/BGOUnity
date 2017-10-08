using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.Managers
{
    public class NetworkManager
    {
        private GameEventChannel Channel;
        private GameBoardManager Manager;

        public IServerAdapter Server { get { return SceneTransporter.Server; } }

        public NetworkManager(GameBoardManager manager, GameEventChannel channel)
        {
            Manager = manager;
            this.Channel = channel;
            Channel.GameEvent += OnSubscribedGameEvents;
        }

        private void OnSubscribedGameEvents(object sender, GameUIEventArgs args)
        {
            if (Manager == null)
            {
                Channel.GameEvent -= OnSubscribedGameEvents;
                return;
            }
            if (args.UIKey != "NetworkManager")
            {
                return;
            }

            if (args.EventType == GameUIEventType.ForceRefresh)
            {
                //通知界面暗转等待网络
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "PCBoard"));

                Manager.StartCoroutine(Server.RefreshBoard(Manager.CurrentGame, (error) =>
                {
                    Manager.CurrentDisplayingBoardNo = Manager.CurrentGame.MyPlayerIndex;
                    
                    var msg = new ServerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard");
                    Channel.Broadcast(msg);
                }));
            }else if (args.EventType == GameUIEventType.TakeAction)
            {
                //通知界面暗转等待网络
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "PCBoard"));

                var action = (PlayerAction) args.AttachedData["PlayerAction"];
                if (action.Internal)
                {
                    Manager.StartCoroutine(Server.TakeInternalAction(Manager.CurrentGame,
                        action, (actions) =>
                        {
                            Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "PCBoard"));


                            var msg = new ManagerGameUIEventArgs(GameUIEventType.ReportInternalAction, "PCBoard");
                            msg.AttachedData["Actions"] = actions;
                            Channel.Broadcast(msg);
                            /*
                            PlayerAction actionToPerform =null;
                            foreach (var candidateAction in actions)
                            {
                                if (candidateAction.ActionType == PlayerActionType.DevelopTechCard)
                                {
                                    //选出Action
                                    if (args.AttachedData.ContainsKey("AdditionalCard1") &&
                                        candidateAction.Data[0] as CardInfo == args.AttachedData["AdditionalCard1"] as CardInfo)
                                    {
                                        actionToPerform = candidateAction;
                                    }
                                }

                            }
                            

                            if (actionToPerform == null)
                            {
                                LogRecorder.Log("Selection invalid!");
                                //GameManager提交过来的选择无效
                                Channel.Broadcast(new ServerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
                            }
                            else
                            {
                                Manager.StartCoroutine(Server.TakeAction(Manager.CurrentGame,
                                    actionToPerform, () =>
                                    {
                                        Channel.Broadcast(new ServerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard"));
                                    }));
                            }*/
                        }));
                }
                else
                {
                    Manager.StartCoroutine(Server.TakeAction(Manager.CurrentGame,
                        action, (error) =>
                        {
                            var msg = new ServerGameUIEventArgs(GameUIEventType.Refresh, "PCBoard");
                            Channel.Broadcast(msg);
                        }));
                }
            }
        }
    }
}
