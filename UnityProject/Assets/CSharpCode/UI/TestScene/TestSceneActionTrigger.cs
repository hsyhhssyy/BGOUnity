using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.TestScene
{
    public class TestSceneActionTrigger:InputActionTriggerMonoBehaviour
    {
        public TextMesh mesh;
        private String lastlastAction = "N/A";
        private int lastCombo=0;
        private String lastAction="N/A";
        private int Combo=0;

        public void Update()
        {
            mesh.text = lastlastAction + "x" + lastCombo.ToString() +" + " +lastAction + "x" + Combo.ToString();
        }

        public void AddCombo(String act)
        {
            if (lastAction != act)
            {
                lastlastAction = lastAction;
                lastCombo = Combo;

                lastAction = act;
                Combo = 1;
            }
            else
            {
                Combo += 1;
            }
        }

        public override bool OnTriggerClick()
        {
            AddCombo("Click");
            return true;
        }

        public override bool OnTriggerClickOutside()
        {
            AddCombo("ClickOutside");
            return true;
        }

        public override bool OnTriggerDown()
        {
            AddCombo("Down");
            return true;
        }

        public override bool OnTriggerUp()
        {
            AddCombo("Up");
            return true;
        }

        public override bool OnTriggerEnter()
        {
            AddCombo("Enter");
            return true;
        }

        public override bool OnTriggerExit()
        {
            AddCombo("Exit");
            return true;
        }
    }
}
