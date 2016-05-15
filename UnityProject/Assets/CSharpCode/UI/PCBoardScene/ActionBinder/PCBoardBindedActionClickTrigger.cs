using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class PCBoardBindedActionClickTrigger : MonoBehaviour
    {
        public PlayerAction Action;

        public PCBoardBehavior BoardBehavior;

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            if (Action == null)
            {
                return;
            }

            if (Action.ActionType == PlayerActionType.ProgramDelegateAction)
            {
                var act = Action.Data[0] as System.Action;
                if (act != null)
                {
                    act();
                }
            }
            else
            {
                BoardBehavior.TakeAction(Action);
            }
        }
    }
}
