using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    [UsedImplicitly]
    public class EventsPopupDisplayBehaviour:MonoBehaviour
    {
        public GameObject JoanOfArcPreviewPopup;

        public GameObject[] PlayedEventTableFrames;

        public GameObject MyEventsFrame;

        private GameObject _hiddenEventsPrefeb;
        private GameObject _hiddenEventsSeperatorPrefeb;
        private GameObject _smallCardPrefeb;

        public void Refresh()
        {
            TtaGame game = SceneTransporter.CurrentGame;

            if (SceneTransporter.CurrentGame.CurrentEventCard != null)
            {
                JoanOfArcPreviewPopup.GetComponent<PCBoardCardDisplayBehaviour>().Bind(SceneTransporter.CurrentGame.CurrentEventCard);
                JoanOfArcPreviewPopup.SetActive(true);
            }
            else
            {
                JoanOfArcPreviewPopup.SetActive(false);
            }

            if (_hiddenEventsPrefeb == null)
            {
                _hiddenEventsPrefeb = Resources.Load<GameObject>("Dynamic-PC/HiddenEvent");
                _hiddenEventsSeperatorPrefeb = Resources.Load<GameObject>("Dynamic-PC/HiddenEventSeperator");
                _smallCardPrefeb= Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");
            }

            int i = 0;
            for (; i < game.Boards.Count; i++)
            {
                PlayedEventTableFrames[i].SetActive(true);
                DisplayPlayerEvents(game.Boards[i], PlayedEventTableFrames[i]);
            }

            for (; i < 4; i++)
            {
                PlayedEventTableFrames[i].SetActive(false);
            }

            DisplayMyEvents(game.Boards[SceneTransporter.CurrentGame.MyPlayerIndex]);
        }

        public void DisplayPlayerEvents(TtaBoard player, GameObject frame)
        {
            frame.FindObject("PlayerName").GetComponent<TextMesh>().text =
                player.PlayerName+":";


            float start = 1.032f;

            var hidFrame = frame.FindObject("HiddenEvents");
            foreach (Transform child in hidFrame.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var eventCard in player.CurrentEventPlayed)
            {
                GameObject mSp = Instantiate(_hiddenEventsPrefeb);
                mSp.transform.SetParent(hidFrame.transform);
                mSp.transform.localPosition = new Vector3(start, -0.07f);
                mSp.transform.localScale = new Vector3(1f, 1f, 1f);

                mSp.FindObject("AgeText").GetComponent<TextMesh>().text = eventCard.CardAge.ToString();

                start += 0.4f;
            }

            if (player.FutureEventPlayed.Count + player.CurrentEventPlayed.Count > 0)
            {
                GameObject sep = Instantiate(_hiddenEventsSeperatorPrefeb);
                sep.transform.SetParent(hidFrame.transform);
                sep.transform.localPosition = new Vector3(start - 0.16f, -0.07f);
                sep.transform.localScale = new Vector3(1f, 1f, 1f);
                start += 0.22f - 0.16f;
            }

            foreach (var eventCard in player.FutureEventPlayed)
            {
                GameObject mSp = Instantiate(_hiddenEventsPrefeb);
                mSp.transform.SetParent(hidFrame.transform);
                mSp.transform.localPosition = new Vector3(start, -0.07f);
                mSp.transform.localScale = new Vector3(1f, 1f, 1f);

                mSp.FindObject("AgeText").GetComponent<TextMesh>().text = eventCard.CardAge.ToString();

                start += 0.4f;
            }
        }

        public void DisplayMyEvents(TtaBoard player)
        {
            float start = 0f;

            foreach (Transform child in MyEventsFrame.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var eventCard in player.CurrentEventPlayed)
            {
                GameObject mSp = Instantiate(_smallCardPrefeb);
                mSp.transform.SetParent(MyEventsFrame.transform);
                mSp.transform.localPosition = new Vector3(start, 0f);
                mSp.transform.localScale = new Vector3(1f, 1f, 1f);

                mSp.GetComponent<PCBoardCardDisplayBehaviour>().Bind(eventCard);

                start += 0.7f;
            }

            if (player.FutureEventPlayed.Count + player.CurrentEventPlayed.Count > 0)
            {
                GameObject sep = Instantiate(_hiddenEventsSeperatorPrefeb);
                sep.transform.SetParent(MyEventsFrame.transform);
                sep.transform.localPosition = new Vector3(start - 1.13f, 0f);
                sep.transform.localScale = new Vector3(1f, 1f, 1f);
                start += 0.16f;
            }

            foreach (var eventCard in player.FutureEventPlayed)
            {
                GameObject mSp = Instantiate(_smallCardPrefeb);
                mSp.transform.SetParent(MyEventsFrame.transform);
                mSp.transform.localPosition = new Vector3(start, 0f);
                mSp.transform.localScale = new Vector3(1f, 1f, 1f);

                mSp.GetComponent<PCBoardCardDisplayBehaviour>().Bind(eventCard);

                start += 0.7f;
            }
        }
    }
}
