using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Collider
{
    public class TacticsPopupToggleCollider:MonoBehaviour
    {

        public GameObject Popup;

        public bool toggled=false;

        public void OnMouseUpAsButton()
        {
            if (Popup != null)
            {
                Popup.transform.localPosition = new Vector3(-1.93f,-1.1f,-5f);
                toggled = !toggled;
                Popup.SetActive(toggled);
            }
        }
    }
}
