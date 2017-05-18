using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using Assets.CSharpCode.UI.Util.Input;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard.Buildings
{
    public class WonderMenuItemController: SimpleClickUIController
    {
        public GameObject HighlightFrame;
        private PlayerAction  _action;
        
        protected override string GetUIKey()
        {
            return "PCBoard.ConstructingWonderMenuItem." + Guid;
        }
        
        public void SetAction(PlayerAction action)
        {
            _action = action;
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["Action"] = _action;
        }

        protected override void Refresh()
        {
            //Do nothing
        }

        protected override void OnHoveringHighlightChanged()
        {
            var highlight = IsHoveringAndAllowSelected;
            if (HighlightFrame != null)
            {
                HighlightFrame.SetActive(highlight);
            }
        }
    }
}
