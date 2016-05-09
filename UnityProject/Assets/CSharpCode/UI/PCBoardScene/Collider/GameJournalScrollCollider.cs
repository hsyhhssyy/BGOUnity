
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Collider
{
    public class GameJournalScrollCollider:MonoBehaviour
    {
        private Vector3 lastMousePosition;

        private float originalY;

        public GameObject panel;

        void OnMouseDown()
        {
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            originalY = panel.transform.localPosition.y;
        }

        void OnMouseDrag()
        {
            Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;
            //Debug.Log("The mouse moved " + distance.magnitude + " pixels");
            
            //distance = Camera.main.ScreenToWorldPoint(distance);

            panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, originalY + distance.y,
                panel.transform.localPosition.z);
        }
    }
}
