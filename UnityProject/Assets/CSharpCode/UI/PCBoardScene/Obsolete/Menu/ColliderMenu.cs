using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    [UsedImplicitly]
    public class ColliderMenu:MonoBehaviour
    {
        public List<ColliderMenuButton> Buttons;

        public Dictionary<int, Func<bool>> Actions;

        public void Popup()
        {
            Vector3 pos = Input.mousePosition;

            pos = Camera.main.ScreenToWorldPoint(pos);

            this.transform.position = new Vector3(pos.x,pos.y,-5f);
        }

        internal void Triggered(ColliderMenuButton button)
        {
            if (Buttons == null || Actions == null)
            {
                return;
            }
            if (Buttons.Contains(button))
            {
                var index = Buttons.IndexOf(button);
                if (Actions.ContainsKey(index))
                {
                    var action = Actions[index];
                    if (action != null)
                    {
                        if (action())
                        {
                            this.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
