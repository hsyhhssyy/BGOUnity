using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.Tactic
{
    public class SharedTacticController:SimpleClickUIController
    {
        public CardInfo TacticCard;

        protected override void Refresh()
        {
            
        }

        protected override string GetUIKey()
        {
            return ".PCBoardScene.Dialog.Tactic.TacticItem." + Guid;
        }

        protected override void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Card"] = TacticCard;
            base.AttachDataOnTrySelect(args);
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Card"] = TacticCard;
            base.AttachDataOnSelected(args);
        }
    }
}
