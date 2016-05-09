using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    public class TacticsPopupDisplayBehaviour:MonoBehaviour
    {
        public GameObject MyTactics;

        public String MyTacticsID;

        public GameObject SharedTacticsFrame;

        public GameObject CardImagePopup;

        public void Refresh()
        {
            //Your 
            if (MyTacticsID != null)
            {
                var card = TtaCivilopedia.GetCivilopedia(SceneTransporter.CurrentGame.Version)
                    .GetCardInfo(MyTacticsID);
                MyTactics.GetComponent<PCBoardCardSmallDisplayBehaviour>().Bind(card);
                MyTactics.SetActive(true);
            }
            else
            {
                MyTactics.SetActive(false);
            }

            var prefab=Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in SharedTacticsFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < SceneTransporter.CurrentGame.SharedTactics.Count; i++)
            {
                Instantiate(prefab).GetComponent<PCBoardCardSmallDisplayBehaviour>()
                    .Bind(SceneTransporter.CurrentGame.SharedTactics[i], SharedTacticsFrame.transform,
                        new Vector3(0.72f*i, 0f));
            }
        }
    }
}
