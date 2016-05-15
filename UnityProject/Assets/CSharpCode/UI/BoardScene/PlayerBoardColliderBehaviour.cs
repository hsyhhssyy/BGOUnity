using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.BoardScene
{
    [Obsolete]
    public class PlayerBoardColliderBehaviour:MonoBehaviour
    {
        public BoardUIBehaviour BehaviourController;
        public String Action;
        public String Data;
        public GameObject Go;

        /**

        #region Actions

        public const String ActionSwitchBoard = "ActionSwitchBoard";
        public const String ActionPopupDialog = "PopupDialog";
        public const String ActionTakeCardFromCardRow = "TakeCardFromCardRow";
        #endregion

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("ColliderAction:"+Action);
            switch (Action)
            {
                case ActionSwitchBoard:
                    BehaviourController.SwitchBoard(Data);
                    break;
                case ActionPopupDialog:
                    BehaviourController.PopupDialog(Data);
                    break;
                case ActionTakeCardFromCardRow:
                    BehaviourController.TakeCard(Convert.ToInt32(Data));
                    break;
                default:
                   Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown Action:"+Action);
                    break;
            }
        }
        **/
    }
}
