using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.Dialog;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class SelectTargetPlayerStateHandler : GameBoardStateHandler
    {
        public static readonly string SourceCardStateDataKey = "SelectTargetPlayerStateHandler-SourceCard";

        public SelectTargetPlayerStateHandler(GameBoardManager manager) : base(manager)
        {
        }

        public override void EnteringState()
        {
        }

        public override void LeaveState()
        {
        }

        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains("SelectTargetPlayer"))
            {
                return;
            }

            if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("Option"))
                {
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Option"))
                {
                    var card = Manager.StateData[SelectTargetPlayerStateHandler.SourceCardStateDataKey] as CardInfo;
                    if (card != null)
                    {
                        switch (card.CardType)
                        {
                            case CardType.Aggression:
                            case CardType.War:
                                //找对应Action
                                int index = (int) args.AttachedData[DialogButtonController.DialogButtonDataArgsKey];
                                string name = Manager.CurrentGame.Boards[index].PlayerName;
                                var action = Manager.CurrentGame.PossibleActions.FirstOrDefault(a =>
                                    (a.ActionType == PlayerActionType.Aggression ||
                                     a.ActionType == PlayerActionType.DeclareWar)
                                    && a.Data[0] as CardInfo == card && (String)a.Data[1] == name);
                                if (action != null)
                                {
                                    var msg = new ManagerGameUIEventArgs(GameUIEventType.TakeAction, "NetworkManager");
                                    msg.AttachedData.Add("PlayerAction", action);
                                    Channel.Broadcast(msg);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public override void OnUnityUpdate()
        {
        }
    }
}
