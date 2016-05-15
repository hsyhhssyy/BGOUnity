using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PCBoardActionBinder:MonoBehaviour
    {
        public PCBoardBehavior BoardBehavior;

        public CardRowActionBinder CardRowBinder;

        public PCBoardBindedActionClickTrigger WorkerBankActionTrigger;

        public GameObject BuildingCellFrame;

        public GameObject UnknownActionFrame;

        public void BindAction()
        {
            BindCardRow();
            BindWorkerBank(SceneTransporter.CurrentGame.PossibleActions);
            BindUnknown(SceneTransporter.CurrentGame.PossibleActions);

            BindBuildings(SceneTransporter.CurrentGame.PossibleActions);
        }

        private void BindCardRow()
        {
            CardRowBinder.BindAction();
        }

        private void BindWorkerBank(List<PlayerAction> actions)
        {
            List<PlayerAction> acceptedActions =
                SceneTransporter.CurrentGame.PossibleActions.Where(
                    action =>
                        action.ActionType == PlayerActionType.IncreasePopulation).ToList();
            WorkerBankActionTrigger.Action = acceptedActions.Count > 0 ? acceptedActions[0] : null;

        }

        private void BindBuildings(List<PlayerAction> actions)
        {
            foreach (Transform child in BuildingCellFrame.transform)
            {
                var frame=child.gameObject.GetComponent<BuildingCellActionBinder>();
                frame.BoardBehavior = BoardBehavior;
                frame.BindAction();
            }
        }

        private void BindUnknown(List<PlayerAction> actions)
        {
            List<PlayerAction> acceptedActions =
                SceneTransporter.CurrentGame.PossibleActions.Where(
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
    }
}
