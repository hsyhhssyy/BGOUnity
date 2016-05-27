using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhaseDialog;
using Assets.CSharpCode.UI.PCBoardScene.Menu;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PCBoardActionBinder:MonoBehaviour, TtaActionBinder
    {
        #region Unity GameObjects

        public CardRowActionBinder CardRowBinder;
        public PCBoardBindedActionClickTrigger WorkerBankActionTrigger;
        public GameObject BuildingCellFrame;
        public GameObject UnknownActionFrame;
        public PCBoardBindedActionClickTrigger EndActionPhaseButtonFrame;
        public GameObject CivilCardFrame ;
        public GameObject MilitaryCardFrame;

        public HandCardMenuBehaviour HandCardMenuFrame;

        public SpecificCodeActionTrigger ConstructingWonderFrame;
        public WonderMenuAnimationBehaviour ConstructingWonderMenuFrame;

        public PoliticalPhaseDialogDisplayBehaviour PoliticalPhaseDialogFrame;
        #endregion

        #region TtaActionBinder

        public void BindAction(List<PlayerAction> actions,PCBoardBehavior boardBehavior)
        {
            BindCardRow();

            BindWorkerBank(actions, boardBehavior);
            BindUnknown(actions);

            BindBuildings(actions, boardBehavior);

            BindSpriteUIButtons(actions, boardBehavior);

            BindHandCard(actions.Where(
                   action =>
                       action.ActionType == PlayerActionType.DevelopTechCard ||
                       action.ActionType == PlayerActionType.PlayActionCard ||
                       action.ActionType == PlayerActionType.Revolution ||
                       action.ActionType == PlayerActionType.ElectLeader).ToList(),CivilCardFrame, boardBehavior);
            BindHandCard(actions.Where(
                   action =>
                       action.ActionType == PlayerActionType.SetupTactic).ToList(), MilitaryCardFrame, boardBehavior);

            BindConstructingWonder(actions, boardBehavior);

            PoliticalPhaseDialogFrame.BindAction(actions, boardBehavior);
        }

        public void Unbind()
        {
            WorkerBankActionTrigger.Unbind();

            foreach (Transform child in BuildingCellFrame.transform)
            {
                var frame = child.gameObject.GetComponent<BuildingCellActionBinder>();
                frame.Unbind();
            }
        }

        #endregion

        private void BindCardRow()
        {
            CardRowBinder.BindAction();
        }

        private void BindWorkerBank(List<PlayerAction> actions, PCBoardBehavior boardBehavior)
        {
            List<PlayerAction> acceptedActions =
                actions.Where(
                    action =>
                        action.ActionType == PlayerActionType.IncreasePopulation).ToList();
            WorkerBankActionTrigger.Bind(acceptedActions.Count > 0 ? acceptedActions[0] : null, boardBehavior);

        }

        private void BindBuildings(List<PlayerAction> actions, PCBoardBehavior boardBehavior)
        {
            foreach (Transform child in BuildingCellFrame.transform)
            {
                var frame=child.gameObject.GetComponent<BuildingCellActionBinder>();
                
                frame.BindAction(actions,boardBehavior);
            }
        }

        private void BindConstructingWonder(List<PlayerAction> actions, PCBoardBehavior boardBehavior)
        {
            ConstructingWonderFrame.ActionOnMouseClick = () =>
            {
                List<PlayerAction> acceptedActions =
                    actions.Where(
                        action =>
                            action.ActionType == PlayerActionType.BuildWonder).ToList();

                acceptedActions.Sort((a, b) => ((int) a.Data[1]).CompareTo(b.Data[0]));

                ConstructingWonderMenuFrame.Popup(acceptedActions, boardBehavior);
            };
            ConstructingWonderFrame.ActionOnMouseClickOutside = () =>
            {
                ConstructingWonderMenuFrame.Collapse();
            };
            ConstructingWonderFrame.BoardBehavior = boardBehavior;
        }

        private void BindUnknown(List<PlayerAction> actions)
        {
            List<PlayerAction> acceptedActions =
                actions.Where(
                    action =>
                        action.ActionType == PlayerActionType.Unknown).ToList();

            if (UnknownActionFrame == null)
            {
                return;
            }

            foreach (Transform child in UnknownActionFrame.transform)
            {
                Destroy(child.gameObject);
            }

            var prefab = Resources.Load<GameObject>("Dynamic-PC/GameJournalItem");

            for (int i = 0; i < acceptedActions.Count; i++)
            {
                var mSp = Instantiate(prefab);
                mSp.transform.SetParent(UnknownActionFrame.transform);
                mSp.transform.localPosition = new Vector3(0f,0.3f*i, 0f);

                mSp.FindObject("JournalText").GetComponent<TextMesh>().text =
                    acceptedActions[i].Data[0].ToString().WordWrap(12);
            }

        }

        private void BindHandCard(List<PlayerAction> acceptedActions, GameObject cardFrame,PCBoardBehavior boardBehavior)
        {

            var behaviorList =
                cardFrame.transform.Cast<Transform>()
                    .Select(t => t.gameObject.GetComponent<PCBoardCardDisplayBehaviour>())
                    .Where(behave => behave != null)
                    .ToList().Where(cardDisplayBehavior => cardDisplayBehavior.Card != null)
                    .Where(cd => cd.gameObject.activeSelf == true);


            var cards = new Dictionary<CardInfo, List<PCBoardCardDisplayBehaviour>>();
            foreach (var cd in behaviorList)
            {
                if (!cards.ContainsKey(cd.Card))
                {
                    cards[cd.Card] =new List<PCBoardCardDisplayBehaviour>();
                }

                cards[cd.Card].Add(cd);
            }

            foreach (var action in acceptedActions)
            {
                var key = action.Data[0] as CardInfo;
                if (key == null)
                {
                    LogRecorder.Log("Wrong Action");
                    continue;
                }

                if (!cards.ContainsKey(key))
                {
                    LogRecorder.Log("Missing Card");
                    continue;
                }
                foreach (var behavior in cards[key])
                {
                    var trg = behavior.gameObject.GetComponent<PCBoardBindedActionClickTrigger>();
                    var actionAsync = action;
                    trg.Bind(new PlayerAction()
                    {
                       ActionType = PlayerActionType.ProgramDelegateAction,
                       Data=new Dictionary<int, object>() { { 0, new Action(() =>
                       {
                           HandCardMenuFrame.Popup(actionAsync,trg.gameObject,boardBehavior);
                       })} }
                    }, boardBehavior);
                }
            }
        }

        private void BindSpriteUIButtons(List<PlayerAction> actions, PCBoardBehavior boardBehavior)
        {
            var acceptedAction =
                actions.FirstOrDefault(action => action.ActionType == PlayerActionType.ResetActionPhase);
            if (acceptedAction != null)
            {
                EndActionPhaseButtonFrame.Bind(acceptedAction, boardBehavior);
            }
        }
        
    }
}
