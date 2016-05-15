using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    public class ColliderMenuButton:MonoBehaviour
    {
        public ColliderMenu ParentMenu;

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            if (ParentMenu != null)
            {
                ParentMenu.Triggered(this);
            }
        }
    }
}
