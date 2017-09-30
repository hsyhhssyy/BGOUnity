using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Effects;
using Assets.CSharpCode.UI.Util.Controller;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists
{
    public class SendColonistsCardSelectionController:SimpleClickUIController
    {
        private PCBoardCardSmallHighlightEffect highlightGo;
        public bool Selected { get; set; }

        public override void Start()
        {
            highlightGo = gameObject.AddComponent<PCBoardCardSmallHighlightEffect> ();
            base.Start();
        }

        protected override void Refresh()
        {
            if (IsHoveringAndAllowSelected|| Selected)
            {
                highlightGo.Highlight = true;
            }
            else
            {
                highlightGo.Highlight = false;
            }
        }


        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["CardController"] = this;
            base.AttachDataOnSelected(args);
        }


        protected override string GetUIKey()
        {
            return "PCBoard.PoliticalDialog.SendColonists.Card." + Guid;
        }
    }
}
