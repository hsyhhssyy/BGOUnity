using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class OutOfMyTurnStateHandler:GameBoardStateHandler
    {
        public override void OnUnityUpdate()
        {
            if (!StateData.ContainsKey("Toggle") || (bool)StateData["Toggle"] == false)
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
                Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
            }
        }

        public override void EnteringState()
        {
            
        }

        public override void LeaveState()
        {
        }

        public override void ProcessGameEvents(System.Object sender, GameUIEventArgs args)
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
                    var playerNo = (int)args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }
                }
                else if (args.UIKey.Contains("ForceRefreshButton"))
                {
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));

                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("PlayerTab"))
                {
                    //玩家面板
                    var playerNo = (int)args.AttachedData["PlayerNo"];
                    if (CurrentGame.Boards.Count > playerNo)
                    {
                        Manager.SwitchDisplayingBoardNo(playerNo);
                    }
                }
                else if (args.UIKey.Contains("ForceRefreshButton"))
                {
                    StateData["Toggle"] = false;
                    //Refresh
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.ForceRefresh, "NetworkManager"));
                }
            }
        }

        public OutOfMyTurnStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
