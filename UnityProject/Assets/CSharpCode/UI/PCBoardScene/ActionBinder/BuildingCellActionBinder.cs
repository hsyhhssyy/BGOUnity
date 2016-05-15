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
        public PCBoardBehavior BoardBehavior;
        public BuildingCellDisplayBehavior DisplayBehavior;

        public GameObject MenuFrame;
        

        private GameObject BuildUpgradeMenu;

        public void Start()
        {
            if (BuildUpgradeMenu == null)
            {
                var prefab = Resources.Load<GameObject>("Dynamic-PC/Menu/BuildUpgradeMenu");
                BuildUpgradeMenu = Instantiate(prefab);
                BuildUpgradeMenu.transform.parent = this.gameObject.transform;
                BuildUpgradeMenu.SetActive(false);
            }
        }

        public void Update()
        {

        }

        public void BindAction()
        {
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

                BindCell(DisplayBehavior.Frames[i].FindObject("CardDisplay").GetComponent<PCBoardBindedActionClickTrigger>(), cellInfo);

            }
        }

        private void BindCell(PCBoardBindedActionClickTrigger trigger, BuildingCell cell)
        {
            trigger.Action = null;

            if (BoardBehavior.interAction == null)
            {
                //将点击建筑物面板作为主操作的，只有两个可能
                //1、升级建筑物
                //2、建造新的建筑物

                //因此按下的时候应该触发轮盘按钮
                //
                trigger.Action = new PlayerAction(() =>
                {

                    var buildingCell = cell;
                    BuildUpgradeMenu.SetActive(true);
                    BuildUpgradeMenu.GetComponent<ColliderMenu>().Actions = new Dictionary<int, Func<bool>>
                    {
                        {
                            0, () =>
                            {
                                List<PlayerAction> acceptedActions =
                                    SceneTransporter.CurrentGame.PossibleActions.Where(
                                        action =>
                                            action.ActionType == PlayerActionType.BuildBuilding).ToList();
                                
                                foreach (var action in acceptedActions)
                                {
                                    var card = action.Data[0] as CardInfo;
                                    //LogRecorder.Log("Build "+card.InternalId);
                                    if (card.InternalId == buildingCell.Card.InternalId)
                                    {
                                        BoardBehavior.TakeAction(action);
                                        return true;
                                    }
                                }

                                LogRecorder.Log("No Such Action "+buildingCell.Card.InternalId+".");
                                return false;
                            }
                        },
                        {
                            1, () =>
                            {
                                List<PlayerAction> acceptedActions =
                                    SceneTransporter.CurrentGame.PossibleActions.Where(
                                        action =>
                                            action.ActionType == PlayerActionType.UpgradeBuilding).ToList();

                                foreach (var action in acceptedActions)
                                {
                                    var card = action.Data[0] as CardInfo;

                                    if (card == buildingCell.Card)
                                    {
                                        BoardBehavior.interAction = action;
                                        return true;
                                    }
                                }
                                 LogRecorder.Log("No Such Action");
                                return false;
                            }
                        }
                    };

                    BuildUpgradeMenu.GetComponent<ColliderMenu>().Popup();
                });
            }
            else
            {
                //次要操作分门别类列在这里
            }
        }
    }
}
