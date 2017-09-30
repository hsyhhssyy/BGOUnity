using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists;

namespace Assets.CSharpCode.Managers.GameBoardStateHandlers
{
    public class SendColonistsStateHandler: GameBoardStateHandler
    {
        public override void ProcessGameEvents(object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains("SendColonists"))
            {
                return;
            }
            if (args.EventType == GameUIEventType.TrySelect)
            {
                if (args.UIKey.Contains("Card"))
                {
                    //点的是卡
                    Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));

                    //当然可以啦
                }
                if (args.UIKey.Contains("ConfirmSendColonists"))
                {
                    var controller = args.AttachedData["Data"] as SendColonistsDialogController;
                    controller.UpdateColonizeForceValue();
                    if (controller.NeedValue <= controller.CurrentValue)
                    {
                        Channel.Broadcast(new ManagerGameUIEventArgs(GameUIEventType.AllowSelect, args.UIKey));
                    }

                }
            }
            else if (args.EventType == GameUIEventType.Selected)
            {
                if (args.UIKey.Contains("Card"))
                {
                    //点的是卡
                    var controller=args.AttachedData["CardController"] as SendColonistsCardSelectionController;

                    if (controller != null)
                    {
                        var dialogController = controller.gameObject.transform.parent.parent.parent.GetComponent<SendColonistsDialogController>();
                        if (dialogController != null)
                        {
                            controller.Selected = !controller.Selected;

                            dialogController.UpdateColonizeForceValue();
                        }
                       
                    }

                   
                }
                else 
                if (args.UIKey.Contains("ConfirmSendColonists"))
                {
                    var controller = args.AttachedData["Data"] as SendColonistsDialogController;
                    var cards=controller.CollectSelectedCards();
                    var action =
                        Manager.CurrentGame.PossibleActions.FirstOrDefault(
                            a => a.ActionType == PlayerActionType.SendColonists);
                    var cardsInAction = action.Data[2] as List<CardInfo>;
                    cardsInAction.Clear();
                    cardsInAction.AddRange(cards);
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

        public SendColonistsStateHandler(GameBoardManager manager) : base(manager)
        {
        }
    }
}
