using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    [UsedImplicitly]
    public class LeaderController : SimpleClickUIController
    {
        public GameObject LeaderFrame;

        protected override string GetUIKey()
        {
            return "PCBoard.Leader." + Guid;
        }

        protected override  void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            if (board.Leader == null)
            {
                LeaderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(null);
                LeaderFrame.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("leader_image_no_leader");
            }
            else
            {
                var sprite = UnityResources.GetSprite(board.Leader.SpecialImage) ??
                             UnityResources.GetSprite("leader_unknown");
                LeaderFrame.GetComponent<SpriteRenderer>().sprite = sprite;
                LeaderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.Leader);
            }
        }
        
    }
}
