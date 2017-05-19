using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class ResolveEventStateHandler:GameBoardStateHandler
    {
        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
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
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));

                    //当然可以啦
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Option"))
                {
                    var action =
                               CurrentGame.PossibleActions.FirstOrDefault(
                                   a =>
                                      a.Data[1] == args.AttachedData["Data"]);
                    //点的是选项
                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                    msg.AttachedData.Add("PlayerAction", action);
                    Channel.Broadcast(msg);
                }
            }
        }

        public override void EnteringState()
        {
        }

        public override void LeaveState()
        {
        }

        public override void OnUnityUpdate()
        {
        }

        public ResolveEventStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
