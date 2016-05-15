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
        public GameObject MyTacticFrame;

        public CardInfo MyTactic;

        public GameObject SharedTacticsFrame;
        
        public void Refresh()
        {
            //Your 
            if (MyTactic != null)
            {
                MyTacticFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(MyTactic);
                MyTacticFrame.SetActive(true);
            }
            else
            {
                MyTacticFrame.SetActive(false);
            }

            var prefab=Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in SharedTacticsFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < SceneTransporter.CurrentGame.SharedTactics.Count; i++)
            {
                Instantiate(prefab).GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(SceneTransporter.CurrentGame.SharedTactics[i], SharedTacticsFrame.transform,
                        new Vector3(0.72f*i, 0f));
            }
        }
    }
}
