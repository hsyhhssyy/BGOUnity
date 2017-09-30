using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.ResolveEvent;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.SelectTargetPlayer;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhase
{
    public class PoliticalPhaseController : DisplayOnlyUIController
    {
        public PoliticalPhaseStartDialogController PoliticalPhaseDialogFrame;
        public ResolveEventDialogController ResolveEventDialogFrame;
        public SendColonistsDialogController SendColonistsDialogFrame;
        public SelectTargetPlayerDialogController SelectTargetPlayerDialogFrame;

        protected override void Refresh()
        {
            //这里建议修改为直接使用Manager的State属性
            //Manager会决定当前状态，并且每次刷新页面后，也会自动判断当前状态。
            //会比这里更准确
            if (Manager.CurrentGame.CurrentPhase == TtaPhase.PoliticalPhase
                || Manager.CurrentGame.CurrentPhase == TtaPhase.EventResolution|| 
                Manager.CurrentGame.CurrentPhase == TtaPhase.Colonize||
                Manager.CurrentGame.CurrentPhase== TtaPhase.SendColonists
                )
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
            else
            {
                HideAllPoliticalPhaseDialogs();
            }
        }
        //根据当前的游戏数据和状态来判断要显示哪一个对话框，并显示他
        public void DisplayPoliticalPhaseDialogs(TtaGame game)
        {
            HideAllPoliticalPhaseDialogs();
            //根据事件判断当前阶段到底是哪一个阶段
            var actions = game.PossibleActions;

            //每一个内容都对应一个独立的dialog
            if (Manager.State == GameManagerState.SelectTargetPlayer)
            {
                SelectTargetPlayerDialogFrame.gameObject.SetActive(true);
                SelectTargetPlayerDialogFrame.transform.localPosition = new Vector3(-3.9f, 2.4f, -5f);

                SelectTargetPlayerDialogFrame.DisplayDialog();
            }else
            if (Manager.CurrentGame.CurrentPhase== TtaPhase.PoliticalPhase /*actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null*/)
            {
                //假如含有PassPoliticalPhase，就是始动阶段
                PoliticalPhaseDialogFrame.gameObject.SetActive(true);
                PoliticalPhaseDialogFrame.transform.localPosition = new Vector3(-3.9f, 2.4f, -5f);

                PoliticalPhaseDialogFrame.DisplayDialog();
            }else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.SendColonists)!=null)
            {
                SendColonistsDialogFrame.DisplayDialog();
                SendColonistsDialogFrame.transform.localPosition = new Vector3(-3.30f, 1.6f, -5f);
            }
            else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.ColonizeBid) != null)
            {
                //含有Bid或者SendColonist则是殖民阶段
                ResolveEventDialogFrame.gameObject.SetActive(true);
                ResolveEventDialogFrame.transform.localPosition = new Vector3(-3.61f, 1.6f, -5f);

                ResolveEventDialogFrame.DisplayDialog();
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
            SendColonistsDialogFrame.HideDialog();
            SelectTargetPlayerDialogFrame.HideDialog();
        }
    }
}
