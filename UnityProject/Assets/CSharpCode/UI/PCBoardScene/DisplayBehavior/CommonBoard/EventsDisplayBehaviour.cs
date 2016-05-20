using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    public class EventsDisplayBehaviour:MonoBehaviour
    {
        public GameObject[] CurrentEventCountGOs;

        public GameObject[] FutureEventCountGOs;

        public GameObject JoanOfArcPreviewFrame;
        public GameObject OriginalPreviewFrame;

        public GameObject JoanOfArcAgeTextGo;
        public GameObject JoanOfArcNameTextGo;

        public EventsPopupDisplayBehaviour Popup;

        public void Refresh()
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

            Popup.Refresh();
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
    }
}
