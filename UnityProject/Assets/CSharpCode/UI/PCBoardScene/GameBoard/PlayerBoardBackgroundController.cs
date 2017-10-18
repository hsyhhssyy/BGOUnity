using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.GameBoard
{
    public class PlayerBoardBackgroundController:DisplayOnlyUIController
    {
        private String[] _playerColor = "orange,purple,green,grey".Split(",".ToCharArray());

        protected override void Refresh()
        {
            //var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            
            this.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("SpriteTile/PCBoard/pc-board-player-background-"+_playerColor[Manager.CurrentDisplayingBoardNo]);
        }
    }
}
