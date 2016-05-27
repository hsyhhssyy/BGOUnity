using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    [UsedImplicitly]
    public class GovernmentController : SimpleClickUIController
    {
        public GameObject GovernmentFrame;

        protected override string GetUIKey()
        {
            return "PCBoard.Government." + Guid;
        }

        protected override  void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            GovernmentFrame.FindObject("Name").GetComponent<TextMesh>().text = board.Government.CardName;
            GovernmentFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.Government);
        }
        
    }
}
