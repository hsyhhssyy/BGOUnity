using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PlayerBriefPlateBehavior : MonoBehaviour
    {
        public PCBoradBehavior BoardBehavior;

        public int PlayerNo;
        public GameObject PlayerNameTextMesh;
        public GameObject CultureTotalTextMesh;
        public GameObject CultureIncrementalTextMesh;
        public GameObject ScienceTotalTextMesh;
        public GameObject ScienceIncrementalTextMesh;
        public GameObject MilitaryStrengthTextMesh;
        public GameObject ExplorationTextMesh;
        public GameObject ResourceTotalTextMesh;
        public GameObject ResourceIncrementalTextMesh;
        public GameObject FoodTotalTextMesh;
        public GameObject FoodIncrementalTextMesh;
        public GameObject WhiteMarkerTextMesh;
        public GameObject RedMarkerTextMesh;

        [UsedImplicitly]
        private void Update()
        {

            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                return;
            }

            if (SceneTransporter.CurrentGame.Boards.Count <= PlayerNo)
            {
                return;
            }

            TtaBoard board = SceneTransporter.CurrentGame.Boards[PlayerNo];


            PlayerNameTextMesh.GetComponent<TextMesh>().text =board.PlayerName;

            CultureTotalTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.Culture].ToString();
            CultureIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.ResourceFluctuation[ResourceType.Culture].ToString();

            ScienceTotalTextMesh.GetComponent<TextMesh>().text =
                board.ResourceQuantity[ResourceType.Science].ToString() +
                (board.ResourceQuantity[ResourceType.ScienceForMilitary] == 0
                    ? ""
                    : "<color=#ffa500ff>" +
                      (board.ResourceQuantity[ResourceType.ScienceForMilitary] > 0 ? "+" : "")
                      + board.ResourceQuantity[ResourceType.ScienceForMilitary].ToString() + "</color>");
            ScienceIncrementalTextMesh.GetComponent<TextMesh>().text =
               board.ResourceFluctuation[ResourceType.Science].ToString();

            MilitaryStrengthTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.MilitaryForce].ToString();
            ExplorationTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.Exploration].ToString();

            ResourceTotalTextMesh.GetComponent<TextMesh>().text =
                board.ResourceQuantity[ResourceType.Ore].ToString() +
                (board.ResourceQuantity[ResourceType.OreForMilitary] == 0
                    ? ""
                    : "<color=#ffa500ff>" +
                      (board.ResourceQuantity[ResourceType.OreForMilitary] > 0 ? "+" : "")
                      + board.ResourceQuantity[ResourceType.OreForMilitary].ToString() + "</color>");
             ResourceIncrementalTextMesh.GetComponent<TextMesh>().text =
               board.ResourceFluctuation[ResourceType.Ore].ToString();

            FoodTotalTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.Food].ToString();
            FoodIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.ResourceFluctuation[ResourceType.Food].ToString();

            WhiteMarkerTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.WhiteMarker] + "/" +
                                                                (board.ResourceQuantity[ResourceType.WhiteMarker] +
                                                                 board.ResourceFluctuation[ResourceType.WhiteMarker])
                                                                    .ToString()
                ;
            RedMarkerTextMesh.GetComponent<TextMesh>().text = board.ResourceQuantity[ResourceType.RedMarker] + "/" +
                                                                (board.ResourceQuantity[ResourceType.RedMarker] +
                                                                 board.ResourceFluctuation[ResourceType.RedMarker])
                                                                    .ToString()
                ;
        }


        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            BoardBehavior.SwitchBoard(PlayerNo);
        }
    }
}
