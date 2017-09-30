using System;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.Civilopedia;
using UnityEngine;
using Assets.CSharpCode.UI.PCBoardScene.Controller;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.Tactic
{
    public class TacticPopupDialogController:DisplayOnlyUIController
    {
        public GameObject MyTacticFrame;

        public CardInfo MyTactic;

        public GameObject SharedTacticsFrame;

        public void ForceRefresh()
        {
            RefreshRequired = true;
        }

        protected override void Refresh()
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

            var prefab = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in SharedTacticsFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < SceneTransporter.CurrentGame.SharedTactics.Count; i++)
            {
                var mSp=Instantiate(prefab);
                mSp.GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(SceneTransporter.CurrentGame.SharedTactics[i], SharedTacticsFrame.transform,
                        new Vector3(0.72f * i, 0f, -0.1f));
                var ctrl=mSp.AddComponent<SharedTacticController>();
                ctrl.TacticCard = SceneTransporter.CurrentGame.SharedTactics[i];
            }
        }
    }
}
