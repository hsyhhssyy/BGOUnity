using System;
using System.Collections;
using System.Collections.Generic;
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

        [UsedImplicitly]
        void Start()
        {
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
            System.Random rand = new System.Random(System.DateTime.Now.Second);
            int cardIndex = 0;
            foreach(var card in game.CardRow)
            {
                var cardGo = GameObject.Find("CardRow/CardRow-Card"+ cardIndex.ToString());

                String spriteName = "CardRow_Sprite_CardBackground_" + rand.Next(0, 21);
                var rend = cardGo.GetComponent<SpriteRenderer>();
                var sprite = _dictSprites[spriteName];
                rend.sprite = sprite;

                var textGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString()+"/NameText");
                var textMesh = textGo.GetComponent<TextMesh>();

                var cardName = TtaTranslation.GetTranslatedText(card.InternalId.Split("-".ToCharArray(), 2)[1]).WordWrap(7);
                textMesh.text = cardName.Trim();

                var ageGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/AgeText");
                textMesh = ageGo.GetComponent<TextMesh>();

                textMesh.text =
                    Enum.GetName(
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

            //1. 设置玩家名称
            var txtPlayerName = GameObject.Find("PlayerBoard0/PlayerName").GetComponent<TextMesh>();
            txtPlayerName.text = board.PlayerName;

            //2. 设置资源面板
            var txtResourceCulture =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceCulture").GetComponent<TextMesh>();
            txtResourceCulture.text = board.ResourceTotal[ResourceType.Culture].ToString() +
                                      " | <color=#ffa500ff>" +
                                      (board.ResourceIncrement[ResourceType.Culture] >= 0 ? "+" : "-") +
                                      Math.Abs(board.ResourceIncrement[ResourceType.Culture]).ToString() + "</color>";

            var txtResourceScience =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceScience").GetComponent<TextMesh>();
            txtResourceScience.text = board.ResourceTotal[ResourceType.Science].ToString() +
                                      (board.ResourceTotal[ResourceType.ScienceForMilitary] > 0
                                          ? "+<color=#ff0000ff>" + board.ResourceTotal[ResourceType.ScienceForMilitary] +
                                            "</color>"
                                          : "")
                                      +
                                      " | <color=#ffa500ff>" +
                                      (board.ResourceIncrement[ResourceType.Science] >= 0 ? "+" : "-") +
                                      Math.Abs(board.ResourceIncrement[ResourceType.Science]).ToString() + "</color>";

            var txtResourceMilitaryForce =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceMilitaryForce").GetComponent<TextMesh>();
            txtResourceMilitaryForce.text = board.ResourceTotal[ResourceType.MilitaryForce].ToString();

            var txtResourceExploration =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceExploration").GetComponent<TextMesh>();
            txtResourceExploration.text = board.ResourceTotal[ResourceType.Exploration] <= 0
                ? " - "
                : ("+" + board.ResourceTotal[ResourceType.Exploration].ToString());

            var txtResourceFood =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceFood").GetComponent<TextMesh>();
            txtResourceFood.text = board.ResourceTotal[ResourceType.Food].ToString() +
                                      " | <color=#ffa500ff>" +
                                      (board.ResourceIncrement[ResourceType.Food] >= 0 ? "+" : "-") +
                                      Math.Abs(board.ResourceIncrement[ResourceType.Food]).ToString() + "</color>";

            var txtResourceOre =
                GameObject.Find("PlayerBoard0/ResourceDisplay/ResourceOre").GetComponent<TextMesh>();
            txtResourceOre.text = board.ResourceTotal[ResourceType.Ore].ToString() +
                                      (board.ResourceTotal[ResourceType.OreForMilitary] > 0
                                          ? "+<color=#ff0000ff>" + board.ResourceTotal[ResourceType.OreForMilitary] +
                                            "</color>"
                                          : "")
                                      +
                                      " | <color=#ffa500ff>" +
                                      (board.ResourceIncrement[ResourceType.Ore] >= 0 ? "+" : "-") +
                                      Math.Abs(board.ResourceIncrement[ResourceType.Ore]).ToString() + "</color>";

        }
    }
}
