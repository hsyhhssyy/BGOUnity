using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog
{
    [UsedImplicitly]
    public class DialogButtonController:SimpleClickUIController
    {
        public const string DialogButtonDataArgsKey = "Data";

        public String ButtonName;

        public Object Data;

        protected override void Refresh()
        {
            //Do Nothing
        }

        protected override string GetUIKey()
        {
            return "PCBoard." + ButtonName + ".DialogButton." + Guid;
        }

        protected override void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
            args.AttachedData["ButtonName"] = ButtonName;
            if (Data != null)
            {
                args.AttachedData[DialogButtonDataArgsKey] = Data;
            }
            base.AttachDataOnTrySelect(args);
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            args.AttachedData["ButtonName"] = ButtonName;
            if (Data != null)
            {
                args.AttachedData[DialogButtonDataArgsKey] = Data;
            }
            base.AttachDataOnSelected(args);
        }
    }
}
