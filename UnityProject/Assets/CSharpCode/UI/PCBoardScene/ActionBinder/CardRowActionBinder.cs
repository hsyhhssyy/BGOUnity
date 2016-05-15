using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    [UsedImplicitly]
    public class CardRowActionBinder:MonoBehaviour
    {
        public PCBoardBehavior BoardBehaviour ;

        public PCBoardBindedActionClickTrigger[] CardRowColliderItems;

        public void BindAction()
        {
            //第一步，把要的action找出来
            List<PlayerAction> acceptedActions =
                SceneTransporter.CurrentGame.PossibleActions.Where(
                    action =>
                        action.ActionType == PlayerActionType.TakeCardFromCardRow ||
                        action.ActionType == PlayerActionType.PutBackCard).ToList();

            //按照卡牌列位置从小到大排序
            acceptedActions.Sort((a, b) => Convert.ToInt32(a.Data[1]).CompareTo(Convert.ToInt32(b.Data[1])));

            foreach (var action in acceptedActions)
            {
                int position = Convert.ToInt32(action.Data[1]);

                CardRowColliderItems[position].Action = action;
                CardRowColliderItems[position].BoardBehavior = BoardBehaviour;
            }
        }
    }
}
