﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PlayerBoardDisplayBehavior:MonoBehaviour
    {
        public PCBoradBehavior BoardBehavior;

        public GameObject BackgroundSpriteGo;

        public GameObject LeaderFrame;
        public GameObject GovernmentFrame;

        public GameObject[] BlueBankMarkers;
        public GameObject[] YellowBankMarkers;

        public GameObject WhiteMarkerFrame;
        public GameObject RedMarkerFrame;

        public GameObject CivilHandCardFrame;
        public GameObject MilitaryHandCardFrame;

        public GameObject CompletedWondersFrame;
        public GameObject ConstructingWonderFrame;

        public GameObject CardNormalImagePopup;

        public GameObject BuildingCellFrame;

        public SpecialTechDisplayBehavior SpecialTechFrame;

        public CardRowDisplayBehaviour CardRowFrame;

        [UsedImplicitly]
        //用update会导致弹出窗口的OnMouseExit捕捉不到
        public void Refresh()
        {
            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                return;
            }

            if (SceneTransporter.CurrentGame.Boards.Count <= BoardBehavior.CurrentPlayerNo)
            {
                return;
            }

            TtaBoard board = SceneTransporter.CurrentGame.Boards[BoardBehavior.CurrentPlayerNo];


            DisplayCardRow(SceneTransporter.CurrentGame.CardRow);

            var backgroundSpriteName = "SpriteTile/PCBoard/pc-board-player-background-" + "orange,purple,green,grey".Split(",".ToCharArray())[BoardBehavior.CurrentPlayerNo];
            var backgroundsp = UnityResources.GetSprite(backgroundSpriteName);
            BackgroundSpriteGo.GetComponent<SpriteRenderer>().sprite = backgroundsp;

            //展示用代码
            //Colliders 是独立的文件

            DisplayYellowBlueBank(board);

            DisplayWhiteRedMarkers(board);

            DisplayGovenrment(board);
            DisplayLeader(board);

            DisplayCivilHandCard(board);
            DisplayMilitaryHandCard(board);

            DisplayWonders(board);

            DisplayBuildings(board);

            DisplaySpecialTech(board);
        }

        private void DisplayCardRow(List<CardRowCardInfo> cardRow)
        {
            CardRowFrame.CardRow = cardRow;
            CardRowFrame.Refresh();
        }

        private void DisplayGovenrment(TtaBoard board)
        {
            GovernmentFrame.FindObject("Name").GetComponent<TextMesh>().text = board.Government.CardName;
            GovernmentFrame.GetComponent<CardNormalImagePopupCollider>().CardInternalId =
                board.Government.InternalId;
        }

        private void DisplayLeader(TtaBoard board)
        {
            if (board.Leader == null)
            {
                LeaderFrame.GetComponent<CardNormalImagePopupCollider>().CardInternalId = null;
            }
            else
            {
                var sprite = UnityResources.GetSprite(board.Leader.SpecialImage) ??
                             UnityResources.GetSprite("leader_unknown");
                LeaderFrame.GetComponent<SpriteRenderer>().sprite = sprite;
                LeaderFrame.GetComponent<CardNormalImagePopupCollider>().CardInternalId = board.Leader.InternalId;
            }

        }

        private void DisplayYellowBlueBank(TtaBoard board)
        {
            //蓝点
            int blueMarkerOwn = board.ResourceQuantity[ResourceType.BlueMarker];
            for (int blueMarkerDisplay = 15;
                blueMarkerOwn >= 0 || blueMarkerDisplay >= 0;
                blueMarkerDisplay--, blueMarkerOwn--)
            {
                if (blueMarkerDisplay >= 0)
                {
                    var bankGo = BlueBankMarkers[15-blueMarkerDisplay];
                    bankGo.SetActive(blueMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }

            //黄点
            int yellowMarkerOwn = board.ResourceQuantity[ResourceType.YellowMarker];
            for (int yellowMarkerDisplay = 17;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = YellowBankMarkers[17-yellowMarkerDisplay];
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }
        }

        private void DisplayWhiteRedMarkers(TtaBoard board)
        {
            //framename是指ActionPoint的frame
            var missingPrefab = Resources.Load<GameObject>("MissingMarker");
            var whitePrefab = Resources.Load<GameObject>("WhiteMarker");
            var redPrefab = Resources.Load<GameObject>("RedMarker");

            DisplayActionPoint(WhiteMarkerFrame, board.ResourceFluctuation[ResourceType.WhiteMarker],
                board.ResourceQuantity[ResourceType.WhiteMarker], whitePrefab, missingPrefab);

            DisplayActionPoint(RedMarkerFrame, board.ResourceFluctuation[ResourceType.RedMarker],
                board.ResourceQuantity[ResourceType.RedMarker], redPrefab, missingPrefab);
        }

        private void DisplayActionPoint(GameObject civilActionFrame, int markerFluctuation, int markerQuantity,
            GameObject whitePrefab, GameObject missingPrefab)
        {
            foreach (Transform child in civilActionFrame.transform)
            {
                Destroy(child.gameObject);
            }
            //确定第一行

            var markerTotal = markerFluctuation + markerQuantity;

            if (markerTotal > 6)
            {
                //2行的第一行
                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb = null;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(0.02f + i * 0.15f, -0.075f);
                }

                //第二行
                var initate = 0.02f + (6 - markerQuantity - markerFluctuation) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else if (markerFluctuation > 0)
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.225f);
                }
            }
            else
            {
                var initate = 0.02f + (6 - markerQuantity - markerFluctuation) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else if (markerFluctuation > 0)
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.15f);
                }
            }
        }

        private void DisplayCivilHandCard(TtaBoard board)
        {
            var unknownCardPrefab = Resources.Load<GameObject>("Dynamic-PC/UnknownCard-Small");

            foreach (Transform child in CivilHandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.CivilCards.Count > 5)
            {
                incr = 0.7f*4/(board.CivilCards.Count-1);
            }

            for (int i = 0; i < board.CivilCards.Count; i++)
            {
                GameObject cardGo = Instantiate(unknownCardPrefab);

                var card = board.CivilCards[i];
                Sprite civilCard = UnityResources.GetSprite("Card-Small-"+card.InternalId);

                if (civilCard != null)
                {
                    cardGo.GetComponent<SpriteRenderer>().sprite = civilCard;
                }
                else
                {
                    cardGo.FindObject("AgeText").GetComponent<TextMesh>().text = card.CardAge.ToString();
                    cardGo.FindObject("NameText").GetComponent<TextMesh>().text = card.CardName.WordWrap(10);
                }

                cardGo.transform.SetParent(CivilHandCardFrame.transform);
                cardGo.transform.localPosition = new Vector3(i * incr, 0);

                CardNormalImagePopupCollider popupCollider = cardGo.GetComponent<CardNormalImagePopupCollider>();
                popupCollider.popup = CardNormalImagePopup;
                popupCollider.CardInternalId = card.InternalId;

            }
        }
        private void DisplayMilitaryHandCard(TtaBoard board)
        {
            var unknownCardPrefab = Resources.Load<GameObject>("Dynamic-PC/UnknownCard-Small");

            foreach (Transform child in MilitaryHandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.MilitaryCards.Count > 5)
            {
                incr = 0.7f * 4 / (board.MilitaryCards.Count-1);
            }

            for (int i = 0; i < board.MilitaryCards.Count; i++)
            {
                GameObject cardGo = Instantiate(unknownCardPrefab);

                var card = board.MilitaryCards[i];
                Sprite civilCard = UnityResources.GetSprite("Card-Small-" + card.InternalId);

                if (civilCard != null)
                {
                    cardGo.GetComponent<SpriteRenderer>().sprite = civilCard;
                }
                else
                {
                    cardGo.FindObject("AgeText").GetComponent<TextMesh>().text = card.CardAge.ToString();
                    cardGo.FindObject("NameText").GetComponent<TextMesh>().text = card.CardName.WordWrap(10);
                }

                cardGo.transform.SetParent(MilitaryHandCardFrame.transform);
                cardGo.transform.localPosition = new Vector3(i * incr, 0);

                CardNormalImagePopupCollider popupCollider= cardGo.GetComponent<CardNormalImagePopupCollider>();
                popupCollider.popup = CardNormalImagePopup;
                popupCollider.CardInternalId = card.InternalId;
            }
        }

        private void DisplayWonders(TtaBoard board)
        {

            //建造中
            if (board.ConstructingWonder != null)
            {
                ConstructingWonderFrame.SetActive(true);
                ConstructingWonderFrame.FindObject("WonderName").GetComponent<TextMesh>().text =
                    board.ConstructingWonder.CardName;

                String buildStr = " ";
                foreach (var str in board.ConstructingWonderSteps)
                {
                    if (str == "X")
                    {
                        buildStr += "<quad material=1 size=15 x=0 y=0 width=1 height=1 />";
                    }
                    else
                    {
                        buildStr += " " + str;
                    }
                }
                ConstructingWonderFrame.FindObject("WonderStep").GetComponent<TextMesh>().text =
                    buildStr;

                CardNormalImagePopupCollider popupCollider = ConstructingWonderFrame.GetComponent<CardNormalImagePopupCollider>();
                popupCollider.popup = CardNormalImagePopup;
                popupCollider.CardInternalId = board.ConstructingWonder.InternalId;
            }
            else
            {
                ConstructingWonderFrame.SetActive(false);
            }

            //建造完成
            var wonderPrefab = Resources.Load<GameObject>("Dynamic-PC/CompletedWonder");

            foreach (Transform child in CompletedWondersFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.CompletedWonders.Count; i++)
            {
                var wonderCard = board.CompletedWonders[i];

                GameObject cardGo = Instantiate(wonderPrefab);
                cardGo.FindObject("WonderName").GetComponent<TextMesh>().text = wonderCard.CardName;

                cardGo.transform.SetParent(CompletedWondersFrame.transform);
                cardGo.transform.localPosition = new Vector3(0, -0.27f * i);

                CardNormalImagePopupCollider popupCollider = cardGo.FindObject("Mask").GetComponent<CardNormalImagePopupCollider>();
                popupCollider.popup = CardNormalImagePopup;
                popupCollider.CardInternalId = wonderCard.InternalId;
            }
        }

        private void DisplayBuildings(TtaBoard board)
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

            int i = 0;
            foreach (var t in buildingArray)
            {
                if (!board.Buildings.ContainsKey(t))
                {
                    continue;
                }

                var buildings = board.Buildings[t];
                
                GameObject cellGo = Instantiate(buildingPrefab);
                BuildingCellDisplayBehavior bds = cellGo.GetComponent<BuildingCellDisplayBehavior>();
                bds.Cells = buildings;
                bds.CardPopupFrame = CardNormalImagePopup;

                cellGo.transform.SetParent(BuildingCellFrame.transform);
                cellGo.transform.localPosition=new Vector3(-3.926f+ incr * i, 0.726f,-0.1f*i);

                bds.Refresh();
                i ++;
            }
        }

        private void DisplayColony(TtaBoard board)
        {
            
        }

        private void DisplaySpecialTech(TtaBoard board)
        {
            SpecialTechFrame.SpecialTechs = board.SpecialTechs;

            SpecialTechFrame.Refresh();
            
        }
    }
}
