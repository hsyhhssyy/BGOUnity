using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    [UsedImplicitly]
    public class EventsController:SimpleClickUIController
    {
        public GameObject[] CurrentEventCountGOs;

        public GameObject[] FutureEventCountGOs;

        public GameObject JoanOfArcPreviewFrame;
        public GameObject OriginalPreviewFrame;

        public GameObject JoanOfArcAgeTextGo;
        public GameObject JoanOfArcNameTextGo;

        public EventsPopupController Popup;

        protected override string GetUIKey()
        {
            return "PCBoard.EventPopup."+Guid;
        }

        protected override void Refresh()
        {
            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                return;
            }

            if (SceneTransporter.CurrentGame.CurrentEventCard != null)
            {
                JoanOfArcPreviewFrame.SetActive(true);
                OriginalPreviewFrame.SetActive(false);
                JoanOfArcAgeTextGo.GetComponent<TextMesh>().text =
                    SceneTransporter.CurrentGame.CurrentEventCard.CardAge.ToString();
                JoanOfArcNameTextGo.GetComponent<TextMesh>().text =
                    SceneTransporter.CurrentGame.CurrentEventCard.CardName;

            }
            else
            {
                JoanOfArcPreviewFrame.SetActive(false);
                OriginalPreviewFrame.SetActive(true);
                OriginalPreviewFrame.GetComponent<TextMesh>().text =
                    SceneTransporter.CurrentGame.CurrentEventAge.ToString();
            }

            Fill(CurrentEventCountGOs, SceneTransporter.CurrentGame.CurrentAge,
                SceneTransporter.CurrentGame.CurrentEventCount);
            Fill(FutureEventCountGOs, SceneTransporter.CurrentGame.CurrentAge,
                SceneTransporter.CurrentGame.FutureEventCount);
            
        }

        private void Fill(GameObject[] frames, Age CurrentAge, String CardStr)
        {
            var splits = CardStr.Split("+".ToCharArray());

            int agenum = (int)CurrentAge -1;

            frames[0].GetComponent<TextMesh>().text = "0";
            frames[1].GetComponent<TextMesh>().text = "0";
            frames[2].GetComponent<TextMesh>().text = "0";

            for (int i = agenum; i > 0; i--)
            {
                if (splits.Length - (agenum - i) - 1 < 0)
                {
                    break;
                }
                if (splits[splits.Length - (agenum - i) - 1] != "")
                {
                    frames[agenum].GetComponent<TextMesh>().text = splits[splits.Length - (agenum - i) - 1];
                }
            }
        }

        public override bool OnTriggerEnter()
        {
            if (Popup != null)
            {
                Popup.gameObject.transform.position = new Vector3(2f, 1.5f, -5f);

                Popup.gameObject.SetActive(true);
            }
            return base.OnTriggerEnter();
        }

        public override bool OnTriggerExit()
        {
            if (Popup != null)
            {
                Popup.gameObject.SetActive(false);
            }
            return base.OnTriggerExit();
        }
    }
}
