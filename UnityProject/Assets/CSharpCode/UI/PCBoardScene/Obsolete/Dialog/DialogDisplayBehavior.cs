using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog
{
    public class DialogDisplayBehavior:InputActionTriggerMonoBehaviour
    {
        public GameObject DialogFrame;

        public GameObject MinButton;

        public GameObject DialogBackground;
        public GameObject DialogContent;

        //这里只负责移动dialog和最小化

        private Vector3 lastMousePosition;

        private Vector3 original;
        private Vector3 contentOriginal;

        public override bool OnTriggerDown()
        {
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            original = DialogFrame.transform.localPosition;
            contentOriginal= DialogContent.transform.localPosition;
            return true;
        }

        public override bool OnMouseDrag()
        {
            Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;
            //LogRecorder.Log("The mouse moved " + distance.magnitude + " pixels");

            //distance = Camera.main.ScreenToWorldPoint(distance);

            DialogFrame.transform.localPosition = new Vector3(original.x + distance.x, original.y + distance.y,
                DialogFrame.transform.localPosition.z);
            DialogContent.transform.localPosition = new Vector3(contentOriginal.x + distance.x, contentOriginal.y + distance.y,
               DialogContent.transform.localPosition.z);
            return true;
        }

        public override bool OnTriggerClick()
        {
            var mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (MinButton.GetComponent<BoxCollider2D>().OverlapPoint(new Vector2(mousePoint.x, mousePoint.y)))
            {
                MinimizeResume();
            }
            else
            {
                
            }
            return true;
        }


        public void MinimizeResume()
        {
            DialogContent.SetActive(!DialogContent.activeSelf);
            DialogBackground.SetActive(DialogContent.activeSelf);
        }
    }
    
}
