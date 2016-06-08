using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Effects;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhase
{
    [UsedImplicitly]
    public class PoliticalPhaseCardController: SimpleClickUIController
    {

        public CardInfo Card;

        private bool _selected;


        protected override string GetUIKey()
        {
           return "PCBoard.PoliticalDialog.Card." + Guid;
        }

        protected override void Refresh()
        {
           
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            base.OnSubscribedGameEvents(sender,args);
            if (args.UIKey.Contains(Guid))
            {
                if (args.EventType == GameUIEventType.SelectionActive)
                {
                    //不管当前是否是激活，收到SelectionActive都变成取消态
                    _selected = true;
                    gameObject.GetComponent<PCBoardCardSmallHighlightEffect>().Highlight = true;
                }
                if (args.EventType == GameUIEventType.SelectionDeactive)
                {
                    //不管当前是否是激活，收到Deactive都变成取消态
                    _selected = false;
                    gameObject.GetComponent<PCBoardCardSmallHighlightEffect>().Highlight = false;
                }
            }
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Card"] = Card;
            args.AttachedData["Selected"] = _selected;
            base.AttachDataOnSelected(args);
        }

        protected override void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Selected"] = _selected;
            base.AttachDataOnTrySelect(args);
        }
        
    }
}
