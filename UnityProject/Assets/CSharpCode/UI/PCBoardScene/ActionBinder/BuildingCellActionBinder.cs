using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using Assets.CSharpCode.UI.Util;
using Assets.CSharpCode.UI.PCBoardScene.Menu;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.ActionBinder
{
    public class BuildingCellActionBinder : MonoBehaviour, TtaActionBinder
    {
        public BuildingCellDisplayBehavior DisplayBehavior;

        public BuildingMenuAnimationBehaviour MenuFrame;

        
        public void Start()
        {
        }

        public void Update()
        {

        }

        public void BindAction(PCBoardBehavior boardBehavior)
        {
            if (boardBehavior == null)
            {
                return;
            }
            
            var cells = DisplayBehavior.Cells;

            if (cells == null)
            {
                return;
            }

            Age[] ages = { Age.A, Age.I, Age.II, Age.III };

            for (int i = 0; i < 4; i++)
            {
                if (!cells.ContainsKey(ages[i]))
                {
                    continue;
                }

                //显示图片
                var cellInfo = cells[ages[i]];

                BindCell(boardBehavior,DisplayBehavior.Frames[i].FindObject("CardDisplay").GetComponent<SpecificCodeActionTrigger>(),i, cellInfo);

            }
        }

        public void Unbind()
        {
            foreach (GameObject frame in DisplayBehavior.Frames)
            {
                frame.FindObject("CardDisplay").GetComponent<SpecificCodeActionTrigger>().BoardBehavior=null;
            }
        }

        private void BindCell(PCBoardBehavior BoardBehavior,SpecificCodeActionTrigger trigger,int index, BuildingCell cell)
        {
            trigger.BoardBehavior = null;

            if (BoardBehavior.InterAction == null)
            {
                //将点击建筑物面板作为主操作的，只有两个可能
                //1、升级建筑物
                //2、建造新的建筑物
                //3、拆除和摧毁
                if (BoardBehavior.CurrentPlayerBoardIndex != SceneTransporter.CurrentGame.MyPlayerIndex)
                {
                    //不是自己的面板不显示操作菜单
                    return;
                }

                //因此按下的时候应该触发轮盘按钮
                //
                trigger.ActionOnMouseClick= () =>
                {
                    //匿名方法参数传递
                    int localIndex = index;
                    LogRecorder.Log("Popup Building Menu");
                    List<PlayerAction> acceptedActions = new List<PlayerAction>();


                    foreach (var action in SceneTransporter.CurrentGame.PossibleActions)
                    {
                        if (action.ActionType == PlayerActionType.BuildBuilding &&
                            ((CardInfo) action.Data[0]).InternalId == cell.Card.InternalId)
                        {
                            acceptedActions.Add(action);
                        }

                        if (action.ActionType == PlayerActionType.UpgradeBuilding)
                        {
                            if (((CardInfo) action.Data[0]).InternalId == cell.Card.InternalId ||
                                ((CardInfo) action.Data[1]).InternalId == cell.Card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }
                        if (action.ActionType == PlayerActionType.Disband ||
                            action.ActionType == PlayerActionType.Destory)
                        {
                            if (((CardInfo) action.Data[0]).InternalId == cell.Card.InternalId)
                            {
                                acceptedActions.Add(action);
                            }
                        }


                    }
                    MenuFrame.Collapse();
                    MenuFrame.Popup(localIndex, acceptedActions,BoardBehavior);
                };

                trigger.ActionOnMouseClickOutside = () =>
                {
                    MenuFrame.Collapse();
                };
                trigger.BoardBehavior = BoardBehavior;
            }
            else
            {
                //次要操作分门别类列在这里
            }
        }
    }
}
