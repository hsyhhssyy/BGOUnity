using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Managers;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.StatusBar
{
    public class ResetAndSkipButtonController:MonoBehaviour
    {
        public GameBoardManager Manager;
        
        private bool _toggled=true;

        public void Update()
        {
           if(Manager.State != GameManagerState.ActionPhaseIdle)
           {
                if (_toggled)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, -1.064f,
                        transform.localPosition.z);
                }
                _toggled = false;
                return;
            }
           else
           {
                if (_toggled == false)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, -0.064f,
                       transform.localPosition.z);
                }
                _toggled = true;
            }
        }
    }
}
