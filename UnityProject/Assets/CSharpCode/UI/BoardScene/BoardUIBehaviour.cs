using System;
using System.Collections;
using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Translation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.CSharpCode.UI.BoardScene
{
    [UsedImplicitly]
    public class BoardUIBehaviour : MonoBehaviour
    {

        public GameObject LoadingGo;
        public GameObject CardRowGo;

        private readonly Dictionary<String,Sprite> _dictSprites=new Dictionary<string, Sprite>();
        private TtaCivilopedia civilopedia;

        [UsedImplicitly]
        void Start()
        {
            civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

            LoadingGo.SetActive(true);
            CardRowGo.SetActive(false);

            var sprites = Resources.LoadAll<Sprite>("SpriteTile/CardRow_Sprite_CardBackground");

            foreach (Sprite sp in sprites)
            {
                _dictSprites.Add(sp.name, sp);
            }
        
        
            StartCoroutine(GetCardRow());
        
        }

        [UsedImplicitly]
        public void BackButton_Clicked()
        {
            SceneManager.LoadScene("Scene/LobbyScene");
        }
    

        private IEnumerator GetCardRow()
        {
            return SceneTransporter.server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                CardRowGo.SetActive(true);
                DisplayGameBoard(SceneTransporter.CurrentGame);

                LoadingGo.SetActive(false);
    });
        }

        private void DisplayGameBoard(TtaGame game)
        {
            //1、展示卡牌列

            GameObject.Find("CardRow").SetActive(true);
            int cardIndex = 0;
            foreach(var card in game.CardRow)
            {
                var cardGo = GameObject.Find("CardRow/CardRow-Card"+ cardIndex.ToString());

                String spriteName = "CardRowBackground_" + Enum.GetName(typeof(CardType),card.CardType);
                var rend = cardGo.GetComponent<SpriteRenderer>();
                var sprite = _dictSprites[spriteName];
                rend.sprite = sprite;

                var textGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString()+"/NameText");
                var textMesh = textGo.GetComponent<TextMesh>();

                textMesh.text = card.CardName;

                var ageGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/AgeText");
                textMesh = ageGo.GetComponent<TextMesh>();

                textMesh.text = Enum.GetName(
                        typeof (Age),
                        Convert.ToInt32(
                            card.InternalId.Split("-".ToCharArray(), 2)[0]));
            

                cardIndex++;
            }

            //2.展示玩家版
            DisplayPlayerBoard(game, 0);
        }

        private void DisplayPlayerBoard(TtaGame game, int playerNo)
        {
            TtaBoard board = game.Boards[playerNo];

            var boardName = "PlayerBoard"+ playerNo;

            //玩家名称
            var txtPlayerName = GameObject.Find("PlayerBoard0/PlayerName").GetComponent<TextMesh>();
            txtPlayerName.text = board.PlayerName;

            //资源面板
            DisplayResourcePanel(boardName, board);

            //建筑面板
            foreach (var pair in board.Buildings)
            {
                var cellName = boardName + "/BuildingBoard/"+Enum.GetName(typeof(BuildingType),pair.Key)+"Cell";
                DisplayBuildingCell(cellName, pair.Value);
            }

            //蓝点
            int blueMarkerOwn = board.BlueBank;
            for (int blueMarkerDisplay = 15;
                blueMarkerOwn >= 0 || blueMarkerDisplay >= 0;
                blueMarkerDisplay--, blueMarkerOwn--)
            {
                if (blueMarkerDisplay >= 0)
                {
                    var bankGo = GameObject.Find(boardName + "/BlueMarkerBank/" + blueMarkerDisplay);
                    bankGo.SetActive(blueMarkerOwn > 0);
                }
                else
                {
                    //Add new markers
                }
            }

            //黄点
            int yellowMarkerOwn = board.YellowBank;
            for (int yellowMarkerDisplay = 16;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = GameObject.Find(boardName + "/YellowMarkerBank/" + yellowMarkerDisplay);
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Add new markers
                }
            }
        }

        private static void DisplayResourcePanel(String boardName,TtaBoard boardData)
        {
            //2. 设置资源面板
            var txtResourceCulture =
                GameObject.Find(boardName+"/ResourceDisplay/ResourceCulture").GetComponent<TextMesh>();
            txtResourceCulture.text = boardData.ResourceTotal[ResourceType.Culture].ToString() +
                                      " | <color=#ffa500ff>" +
                                      (boardData.ResourceIncrement[ResourceType.Culture] >= 0 ? "+" : "-") +
                                      Math.Abs(boardData.ResourceIncrement[ResourceType.Culture]).ToString() + "</color>";

            var txtResourceScience =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceScience").GetComponent<TextMesh>();
            txtResourceScience.text = boardData.ResourceTotal[ResourceType.Science].ToString() +
                                      (boardData.ResourceTotal[ResourceType.ScienceForMilitary] > 0
                                          ? "+<color=#ff0000ff>" + boardData.ResourceTotal[ResourceType.ScienceForMilitary] +
                                            "</color>"
                                          : "")
                                      +
                                      " | <color=#ffa500ff>" +
                                      (boardData.ResourceIncrement[ResourceType.Science] >= 0 ? "+" : "-") +
                                      Math.Abs(boardData.ResourceIncrement[ResourceType.Science]).ToString() + "</color>";

            var txtResourceMilitaryForce =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceMilitaryForce").GetComponent<TextMesh>();
            txtResourceMilitaryForce.text = boardData.ResourceTotal[ResourceType.MilitaryForce].ToString();

            var txtResourceExploration =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceExploration").GetComponent<TextMesh>();
            txtResourceExploration.text = boardData.ResourceTotal[ResourceType.Exploration] <= 0
                ? " - "
                : ("+" + boardData.ResourceTotal[ResourceType.Exploration].ToString());

            var txtResourceFood =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceFood").GetComponent<TextMesh>();
            txtResourceFood.text = boardData.ResourceTotal[ResourceType.Food].ToString() +
                                   " | <color=#ffa500ff>" +
                                   (boardData.ResourceIncrement[ResourceType.Food] >= 0 ? "+" : "-") +
                                   Math.Abs(boardData.ResourceIncrement[ResourceType.Food]).ToString() + "</color>";

            var txtResourceOre =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceOre").GetComponent<TextMesh>();
            txtResourceOre.text = boardData.ResourceTotal[ResourceType.Ore].ToString() +
                                  (boardData.ResourceTotal[ResourceType.OreForMilitary] > 0
                                      ? "+<color=#ff0000ff>" + boardData.ResourceTotal[ResourceType.OreForMilitary] +
                                        "</color>"
                                      : "")
                                  +
                                  " | <color=#ffa500ff>" +
                                  (boardData.ResourceIncrement[ResourceType.Ore] >= 0 ? "+" : "-") +
                                  Math.Abs(boardData.ResourceIncrement[ResourceType.Ore]).ToString() + "</color>";
        }

        private void DisplayBuildingCell(String cellName,Dictionary<Age,BuildingCell> cellData)
        {
            var cellGo = GameObject.Find(cellName);
            cellGo.SetActive(true);

            GameObject.Find(cellName + "/BuildingMarker/Enable").SetActive(true);
            GameObject.Find(cellName + "/BuildingMarker/Disable").SetActive(true);

            for (int ageNum = 0; ageNum <= 3; ageNum++)
            {
                var ageEnumValue = (Age) ageNum;

                var disableMarker = GameObject.Find(cellName + "/BuildingMarker/Disable/" + ageNum);
                var enableMarker = GameObject.Find(cellName + "/BuildingMarker/Enable/" + ageNum);
                var buildingCounter = GameObject.Find(cellName + "/BuildingCounter/" + ageNum);
                var resourceMarker = GameObject.Find(cellName + "/ResourceMarker/" + ageNum);
                var resourceCounter = GameObject.Find(cellName + "/ResourceCounter/" + ageNum);

                bool techDisabled = !cellData.ContainsKey(ageEnumValue);
                disableMarker.SetActive(techDisabled);
                enableMarker.SetActive(!techDisabled);
                resourceMarker.SetActive(false);
                resourceCounter.SetActive(false);
                buildingCounter.SetActive(false);

                if (techDisabled)
                {
                    continue;
                }

                var cell = cellData[ageEnumValue];

                if (cell.Worker > 0)
                {
                    buildingCounter.SetActive(true);
                    buildingCounter.GetComponent<TextMesh>().text = "x"+cell.Worker.ToString();
                }

                if (cell.Storage > 0)
                {
                    resourceMarker.SetActive(true);
                    resourceCounter.SetActive(true);
                    resourceCounter.GetComponent<TextMesh>().text = "x"+cell.Storage.ToString();
                }
                
            }
        }
    }
}
