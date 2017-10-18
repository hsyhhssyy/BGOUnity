using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.StatusBar
{
   public class RefreshProgressBarController:MonoBehaviour
   {
       public GameBoardManager Manager;
       public TextMesh Text;
       private bool _toggled=true;

       public void Update()
       {
           if (Manager.State != GameManagerState.OutOfMyTurn||SceneTransporter.Server.ServerType!= ServerType.PassiveServer30Sec)
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

           if (Manager.StateData.ContainsKey("Progress"))
           {
               Text.text =  ((int)(float)Manager.StateData["Progress"]).ToString()+"秒刷新";
           }
           //Text=status percentage
           //Image = image
       }
    }
}
