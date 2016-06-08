using Assets.CSharpCode.Entity;
using JetBrains.Annotations;
using UnityEngine;
using System;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [Obsolete]
    public class PlayerBriefPlateDisplayBehavior : MonoBehaviour
    {
        public PCBoardBehavior BoardBehavior;

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


            PlayerNameTextMesh.GetComponent<TextMesh>().text = board.PlayerName;

            CultureTotalTextMesh.GetComponent<TextMesh>().text = board.Resource[ResourceType.Culture].ToString();
            CultureIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.CultureIncrement].ToString();

            ScienceTotalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.Science].ToString() +
                (board.Resource[ResourceType.ScienceForMilitary] == 0
                    ? ""
                    : "<color=#ffa500ff>" +
                      (board.Resource[ResourceType.ScienceForMilitary] > 0 ? "+" : "")
                      + board.Resource[ResourceType.ScienceForMilitary].ToString() + "</color>");
            ScienceIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.ScienceIncrement].ToString();

            MilitaryStrengthTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.MilitaryForce].ToString();
            ExplorationTextMesh.GetComponent<TextMesh>().text = board.Resource[ResourceType.Exploration].ToString();

            ResourceTotalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.Resource].ToString() +
                (board.Resource[ResourceType.ResourceForMilitary] == 0
                    ? ""
                    : "<color=#ffa500ff>" +
                      (board.Resource[ResourceType.ResourceForMilitary] > 0 ? "+" : "")
                      + board.Resource[ResourceType.ResourceForMilitary].ToString() + "</color>");
            ResourceIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.ResourceIncrement].ToString();

            FoodTotalTextMesh.GetComponent<TextMesh>().text = board.Resource[ResourceType.Food].ToString();
            FoodIncrementalTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.FoodIncrement].ToString();

            WhiteMarkerTextMesh.GetComponent<TextMesh>().text =
                board.Resource[ResourceType.WhiteMarker] + "/" + board.Resource[ResourceType.WhiteMarkerMax];

            RedMarkerTextMesh.GetComponent<TextMesh>().text = board.Resource[ResourceType.RedMarker] + "/" +
                                                              board.Resource[ResourceType.RedMarkerMax];

        }


        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            BoardBehavior.SwitchBoard(PlayerNo);
        }
    }
}
