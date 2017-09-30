using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers.GameBoardStateHandlers;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.UnityEnhancement;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.SelectTargetPlayer
{
    public class SelectTargetPlayerDialogController: DialogController
    {
        public GameObject OptionsFrame;
        public PCBoardCardDisplayBehaviour EventCardFrame;

        protected override void Refresh()
        {
            var prefab = Resources.Load<GameObject>("Dynamic-PC/Dialog/DialogOption");

            //每一个Action都生成一个文本选项
            OptionsFrame.RemoveAllTransformChildren();
            
            var boards =
                Manager.CurrentGame.Boards;
            for (var index = 0; index < boards.Count; index++)
            {
                if (index == Manager.CurrentGame.MyPlayerIndex)
                {
                    continue;
                }
                var board = boards[index];

                var optionText = board.PlayerName;

                var mSp = Instantiate(prefab);

                var buttonController = mSp.GetComponent<DialogButtonController>();
                buttonController.Manager = this.Manager;
                buttonController.ButtonName = "SelectTargetPlayerDialog.Option." + index;
                buttonController.Data = index;

                mSp.transform.parent = OptionsFrame.transform;
                mSp.transform.localPosition = new Vector3(0f, -0.6f * index, -0.01f);
                mSp.FindObject("OptionText").GetComponent<TextMesh>().text = optionText;


            }

            if (Manager.StateData.ContainsKey(SelectTargetPlayerStateHandler.SourceCardStateDataKey))
            {
                var card = Manager.StateData[SelectTargetPlayerStateHandler.SourceCardStateDataKey] as CardInfo;
                EventCardFrame.Bind(card);
                EventCardFrame.gameObject.SetActive(true);
            }
            else
            {
                EventCardFrame.gameObject.SetActive(false);
            }
        }
    }
}
