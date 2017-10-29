using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Actions;
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
            if (args.EventType == GameUIEventType.BackgroundRefresh)
            {
                /* ? 强行取消谁发出的界面暗转？
                //强行取消界面暗转
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "PCBoard");
                */
                Manager.StartCoroutine(Server.RefreshBoard(Manager.CurrentGame, (error) =>
                {
                    var msg = new ServerGameUIEventArgs(GameUIEventType.Refresh, "NetworkManager");
                    Channel.Broadcast(msg);
                }));
            }else
            if (args.EventType == GameUIEventType.ForceRefresh)
            {
                //通知界面暗转等待网络
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "NetworkManager"));

                Manager.StartCoroutine(Server.RefreshBoard(Manager.CurrentGame, (error) =>
                {
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "NetworkManager"));

                    Manager.CurrentDisplayingBoardNo = Manager.CurrentGame.MyPlayerIndex;
                    
                    var msg = new ServerGameUIEventArgs(GameUIEventType.Refresh, "NetworkManager");
                    Channel.Broadcast(msg);
                }));
            }else if (args.EventType == GameUIEventType.TakeAction)
            {
                //通知界面暗转等待网络
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.WaitingNetwork, "NetworkManager"));

                var action = (PlayerAction) args.AttachedData["PlayerAction"];
                if (action.Internal)
                {
                    Manager.StartCoroutine(Server.TakeInternalAction(Manager.CurrentGame,
                        action, (actionResponse,actions) =>
                        {
                            Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "NetworkManager"));


                            var msg = new ManagerGameUIEventArgs(GameUIEventType.ReportInternalAction, "NetworkManager");
                            msg.AttachedData["Actions"] = actions;
                            Channel.Broadcast(msg);
                        }));
                }
                else
                {
                    Manager.StartCoroutine(Server.TakeAction(Manager.CurrentGame,
                        action, (actionResponse) =>
                        {
                            Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.CancelWaitingNetwork, "NetworkManager"));

                            if (actionResponse.Type != ActionResponseType.Canceled)
                            {
                                var msg = new ServerGameUIEventArgs(GameUIEventType.Refresh, "NetworkManager");
                                Channel.Broadcast(msg);
                            }

                        }));
                }
            }
        }
    }
}
