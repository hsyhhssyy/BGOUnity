using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.PCBoardScene.Controller;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog
{
    public abstract class DialogController:DisplayOnlyUIController
    {
        public virtual void DisplayDialog()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void HideDialog()
        {
            this.gameObject.SetActive(false);
        }
    }
}
