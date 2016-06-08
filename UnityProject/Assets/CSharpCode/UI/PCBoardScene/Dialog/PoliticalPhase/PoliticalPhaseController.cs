using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.ResolveEvent;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhase
{
    public class PoliticalPhaseController : DisplayOnlyUIController
    {
        public PoliticalPhaseStartDialogController PoliticalPhaseDialogFrame;
        public ResolveEventDialogController ResolveEventDialogFrame;

        protected override void Refresh()
        {
            if (Manager.CurrentGame.CurrentPhase == TtaPhase.PoliticalPhase
                || Manager.CurrentGame.CurrentPhase == TtaPhase.EventResolution)
            {
                //不一定你是当前玩家，因为有可能别人在他的政治行动阶段打你呢
                //或者你正在处理一个事件
                if (Manager.CurrentGame.PossibleActions.Count > 0)
                {
                    DisplayPoliticalPhaseDialogs(Manager.CurrentGame);
                }
                else
                {
                    HideAllPoliticalPhaseDialogs();
                }
            }
        }

        public void DisplayPoliticalPhaseDialogs(TtaGame game)
        {
            HideAllPoliticalPhaseDialogs();
            //根据事件判断当前阶段到底是哪一个阶段
            var actions = game.PossibleActions;

            //每一个内容都对应一个独立的dialog
            if (Manager.CurrentGame.CurrentPhase== TtaPhase.PoliticalPhase /*actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null*/)
            {
                //假如含有PassPoliticalPhase，就是始动阶段
                PoliticalPhaseDialogFrame.gameObject.SetActive(true);
                PoliticalPhaseDialogFrame.transform.localPosition = new Vector3(-3.9f, 2.4f, -5f);

                PoliticalPhaseDialogFrame.DisplayDialog();
            }
            else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //含有Bid或者SendColonist则是殖民阶段
            }
            else if (Manager.CurrentGame.CurrentPhase == TtaPhase.EventResolution)
            {
                //含有ResolveAction则是受到事件影响的阶段
                ResolveEventDialogFrame.gameObject.SetActive(true);
                ResolveEventDialogFrame.transform.localPosition = new Vector3(-3.61f, 1.6f, -5f);

                ResolveEventDialogFrame.DisplayDialog();
            }
            else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //含有Defend则是抵御入侵的阶段
            }
        }

        private void HideAllPoliticalPhaseDialogs()
        {
            PoliticalPhaseDialogFrame.HideDialog();
            ResolveEventDialogFrame.HideDialog();
        }
    }
}
