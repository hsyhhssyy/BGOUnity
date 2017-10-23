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
        public GameObject[] HappyFaces;

        public GameObject HighlightFrame;
        
        protected override string GetUIKey()
        {
            return "PCBoard.WorkerBank." + Guid;
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

            int happyface = board.Resource[ResourceType.HappyFace];
            for (int i = 0; i < 9; i++)
            {
                HappyFaces[i].SetActive(i > happyface);
            }
        }

        protected override void OnHoveringHighlightChanged()
        {
            HighlightFrame.SetActive(isActiveAndEnabled);
        }
    }
}
