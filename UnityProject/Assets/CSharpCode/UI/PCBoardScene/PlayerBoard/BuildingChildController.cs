using System;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Menu;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard
{
    public class BuildingChildController : TtaUIControllerMonoBehaviour
    {
        public GameObject[] Frames;
        public Dictionary<Age, BuildingCell> Cells;
        public BuildingMenuController MenuFrame;
        
        // 来自Prefab的Controller，默认要requ
        //其实放到start里都行了
        private bool _refreshRequired=true;

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            Manager.Regiseter(this);
            foreach (var frame in Frames)
            {
                frame.GetComponent<BuildingCellController>().ParentController = this;
            }
        }
        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
            }
        }

        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired&&Cells!=null)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        public void Refresh()
        {
            Age[] ages = { Age.A, Age.I, Age.II, Age.III };

            for (int i = 0; i < 4; i++)
            {
                if (!Cells.ContainsKey(ages[i]))
                {
                    Frames[i].SetActive(false);
                    continue;
                }

                Frames[i].SetActive(true);

                //显示图片
                var cellInfo = Cells[ages[i]];

                Frames[i].FindObject("CardDisplay").GetComponent<PCBoardCardDisplayBehaviour>()
                        .Bind(cellInfo.Card);

                Frames[i].GetComponent<BuildingCellController>()
                    .Card = cellInfo.Card;

                //显示黄点
                var yellowPrefab = Resources.Load<GameObject>("Dynamic-PC/YellowMarker");
                DisplayMarker(cellInfo.Worker, yellowPrefab, Frames[i].FindObject("YellowMarkers"));

                //显示蓝点
                var bluePrefab = Resources.Load<GameObject>("Dynamic-PC/BlueMarker");
                DisplayMarker(cellInfo.Storage, bluePrefab, Frames[i].FindObject("BlueMarkers"));

            }

            for (int i = 0; i < 4; i++)
            {
                if (Frames[i].activeSelf == false)
                {
                    continue;
                }
                var cellInfo = Cells[ages[i]];
                if (i == 3 || Frames[i + 1].activeSelf != true)
                {
                    Frames[i].FindObject("AgeText").SetActive(true);
                    Frames[i].FindObject("NameText").SetActive(true);
                    Frames[i].FindObject("SideName").SetActive(false);

                    Frames[i].FindObject("AgeText").GetComponent<TextMesh>().text = cellInfo.Card.CardAge.ToString();
                    Frames[i].FindObject("NameText").GetComponent<TextMesh>().text = cellInfo.Card.CardName.WordWrap(4);
                }
                else
                {
                    Frames[i].FindObject("AgeText").SetActive(false);
                    Frames[i].FindObject("NameText").SetActive(false);
                    Frames[i].FindObject("SideName").SetActive(true);

                    Frames[i].FindObject("SideName").GetComponent<TextMesh>().text = cellInfo.Card.CardName.WordWrap(4);
                }

                var imageSp = UnityResources.GetSprite(cellInfo.Card.NormalImage);
                if (imageSp != null)
                {
                    Frames[i].FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
                }

                String typeStr = "urban";
                switch (cellInfo.Card.CardType)
                {

                    case CardType.ResourceTechFarm:
                    case CardType.ResourceTechMine:
                        typeStr = "production";
                        break;
                    case CardType.MilitaryTechAirForce:
                    case CardType.MilitaryTechArtillery:
                    case CardType.MilitaryTechCavalry:
                    case CardType.MilitaryTechInfantry:
                        typeStr = "military";
                        break;

                    case CardType.SpecialTechCivil:
                    case CardType.SpecialTechEngineering:
                    case CardType.SpecialTechExploration:
                    case CardType.SpecialTechMilitary:
                        typeStr = "special";
                        break;
                }
                var sp = UnityResources.GetSprite("pc-board-card-small-" + typeStr + "-background");
                Frames[i].FindObject("TypeFrame").GetComponent<SpriteRenderer>().sprite = sp;
            }
        }
        private void DisplayMarker(int markerTotal, GameObject markerPrefab, GameObject frame)
        {
            foreach (Transform child in frame.transform)
            {
                Destroy(child.gameObject);
            }

            if (markerTotal <= 8)
            {
                //双行显示的第一行
                for (int i = 0; i < 4 && i < markerTotal; i++)
                {
                    var mSp = Instantiate(markerPrefab);
                    mSp.transform.SetParent(frame.transform);
                    mSp.transform.localPosition = new Vector3(i * -0.07f, 0);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }

                //第二行
                //var initate = 0.02f + (4 - markerTotal) * 0.075f;

                for (int i = 0; i < markerTotal - 4; i++)
                {
                    var mSp = Instantiate(markerPrefab);
                    mSp.transform.SetParent(frame.transform);
                    mSp.transform.localPosition = new Vector3(i * -0.07f, -0.15f, -0.01f * i);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
            }
            else
            {
                //显示Marker x N
            }
        }

    }
}
