using System.Linq;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard.Buildings
{
    public class BuildingParentController : TtaUIControllerMonoBehaviour
    {
        public GameBoardManager Manager;

        public GameObject BuildingCellFrame;

        private bool _refreshRequired;

        public BuildingParentController()
        {
            UIKey = "PCBoard.Building.Parent." + Guid;
        }

        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
                return;
            }
        }

        [UsedImplicitly]
        public void Start()
        {
            Manager.Regiseter(this);
        }

        [UsedImplicitly]
        public void FixedUpdate()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh(Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo]);
            }

        }
        private void Refresh(TtaBoard board)
        {
            var buildingPrefab = Resources.Load<GameObject>("Dynamic-PC/BuildingColumn");

            BuildingType[] buildingArray =
            {
                BuildingType.Farm, BuildingType.Mine, BuildingType.Arena, BuildingType.Lab,
                BuildingType.Library, BuildingType.Theater, BuildingType.Temple, BuildingType.AirForce,
                BuildingType.Artillery, BuildingType.Cavalry, BuildingType.Infantry
            };

            foreach (Transform child in BuildingCellFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.Buildings.Count() > 9)
            {
                incr = 0.7f * 8 / (board.Buildings.Count() - 1);
            }

            //Age[] ages = { Age.A, Age.I, Age.II, Age.III };(age可以直接Cast)

            int i = 0;
            foreach (var t in buildingArray)
            {
                if (!board.Buildings.ContainsKey(t))
                {
                    continue;
                }

                var buildings = board.Buildings[t];

                GameObject cellGo = Instantiate(buildingPrefab);

                BuildingChildController bds = cellGo.GetComponent<BuildingChildController>();
                bds.Cells = buildings;
                bds.Manager = this.Manager;

                cellGo.transform.SetParent(BuildingCellFrame.transform);
                cellGo.transform.localPosition = new Vector3(-3.926f + incr * i, 0.726f, -1f + 0.1f * i);
                
                i++;
            }
        }

    }
}
