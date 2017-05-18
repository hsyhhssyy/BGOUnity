using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard
{
    public class YellowMarkerController : SimpleClickUIController
    {
        public GameObject[] YellowBankMarkers;

        public GameObject HighlightFrame;
        
        protected override string GetUIKey()
        {
            return "PCBoard.YellowMarker." + Guid;
        }
        
        protected override void Refresh()
        {
            HighlightFrame.SetActive(isActiveAndEnabled);
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];
            int yellowMarkerOwn = board.Resource[ResourceType.YellowMarker];
            for (int yellowMarkerDisplay = 17;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = YellowBankMarkers[17 - yellowMarkerDisplay];
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }
        }

        protected override void OnHoveringHighlightChanged()
        {
            HighlightFrame.SetActive(isActiveAndEnabled);
        }
    }
}
