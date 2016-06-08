using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhase
{
    public class PlayerSelectionController : SimpleClickUIController
    {
        public GameObject SelectionPointer;
        public TextMesh PlayerName;
        public int PlayerNo;
        

        private bool _selected;

        protected override string GetUIKey()
        {
            return "PCBoard.PoliticalDialog.Player."+ PlayerNo + "."+Guid;
        }

        protected override void Refresh()
        {
            if (Manager.CurrentGame.Boards.Count <= PlayerNo)
            {
                gameObject.SetActive(false);
                return;
            }
            if (PlayerNo == Manager.CurrentGame.MyPlayerIndex)
            {
                gameObject.SetActive(false);
                return;
            }
            SelectionPointer.SetActive(_selected);
            PlayerName.text = Manager.CurrentGame.Boards[PlayerNo].PlayerName;
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            base.OnSubscribedGameEvents(sender, args);
            if (args.UIKey.Contains(Guid))
            {
                if (args.EventType == GameUIEventType.SelectionActive)
                {
                    //不管当前是否是激活，收到SelectionActive都变成取消态
                    _selected = true;
                    SelectionPointer.SetActive(true);
                }
                if (args.EventType == GameUIEventType.SelectionDeactive)
                {
                    //不管当前是否是激活，收到Deactive都变成取消态
                    _selected = false;
                    SelectionPointer.SetActive(false);
                }
            }
        }

        protected override void AttachDataOnTrySelect(ControllerGameUIEventArgs args)
        {
            args.AttachedData["PlayerNo"] = PlayerNo;
            args.AttachedData["Selected"] = _selected;
        }

        protected override void AttachDataOnSelected(ControllerGameUIEventArgs args)
        {
            //请求是否允许选的时候，把自己的状态调整一下
            args.AttachedData["PlayerNo"] = PlayerNo;
            args.AttachedData["Selected"] = _selected;
        }
    }
}
