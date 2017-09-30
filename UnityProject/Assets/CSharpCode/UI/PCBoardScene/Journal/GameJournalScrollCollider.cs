﻿using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Journal
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
            //LogRecorder.Log("The mouse moved " + distance.magnitude + " pixels");
            
            //distance = Camera.main.ScreenToWorldPoint(distance);

            panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, originalY + distance.y,
                panel.transform.localPosition.z);
        }
    }
}
