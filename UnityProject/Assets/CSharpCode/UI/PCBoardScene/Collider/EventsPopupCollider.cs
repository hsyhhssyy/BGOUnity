using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Collider
{
    public class EventsPopupCollider:MonoBehaviour
    {
        public GameObject EventsPopup;

        public void OnMouseEnter()
        {
            if (EventsPopup != null)
            {
                EventsPopup.transform.position = new Vector3(2f,1.5f,-2f);

                EventsPopup.SetActive(true);
            }
        }

        public void OnMouseExit()
        {
            if (EventsPopup != null)
            {
                EventsPopup.SetActive(false);
            }
        }
    }
}
