using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.Tactic;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard
{
    public class TacticController:DisplayOnlyUIController
    {
        public TextMesh TacticName;
        public PCBoardCardDisplayBehaviour Popup;
        public GameObject TacticFrame;

        private bool toggled;

        protected override void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            TacticName.text = board.Tactic == null ? "" : board.Tactic.CardName;
            Popup.Bind(board.Tactic);
            TacticFrame.GetComponent<TacticPopupDialogController>().MyTactic = board.Tactic;
            TacticFrame.SetActive(toggled);
        }

        public override bool OnTriggerClick()
        {
            TacticFrame.transform.localPosition = new Vector3(-1.93f, -1.1f, -5f);
            toggled = !toggled;
            TacticFrame.SetActive(toggled);
            
            TacticFrame.GetComponent<TacticPopupDialogController>().ForceRefresh();

            return true;
        }
    }
}
