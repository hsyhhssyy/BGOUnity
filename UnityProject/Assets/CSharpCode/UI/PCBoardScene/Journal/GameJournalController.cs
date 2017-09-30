using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Dialog;
using Assets.CSharpCode.UI.Util.UnityEnhancement;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Journal
{
    public class GameJournalController:DisplayOnlyUIController
    {
        public GameObject JournalPanel;

        protected override void Refresh()
        {
            var prefab = Resources.Load<GameObject>("Dynamic-PC/GameJournalItem");

            JournalPanel.RemoveAllTransformChildren();

            for (int index = 0; index < Manager.CurrentGame.Journal.Count; index++)
            {
                var journal = Manager.CurrentGame.Journal[index];
                var mSp = Instantiate(prefab);

                var buttonController = mSp.GetComponent<GameJournalItemController>();
                buttonController.Entry = journal;
                mSp.transform.parent = JournalPanel.transform;
                mSp.transform.localPosition = new Vector3(0f, 2f-0.3f* index, -0.01f);
            }
        }
    }
}
