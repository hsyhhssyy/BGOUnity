using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    [UsedImplicitly]
    public class BuildingCellDisplayBehavior:MonoBehaviour
    {
        public GameObject[] Frames;

        public Dictionary<Age, BuildingCell> Cells;
        

        public void Refresh()
        {
            Age[] ages = {Age.A, Age.I, Age.II, Age.III};

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

            }
        }

        private void DisplayMarker(int markerTotal,GameObject markerPrefab,GameObject frame)
        {
            foreach (Transform child in frame.transform)
            {
                Destroy(child.gameObject);
            }

            if (markerTotal <= 8)
            {
                //双行显示的第一行
                for (int i = 0; i < 4&&i<markerTotal; i++)
                {
                    var mSp = Instantiate(markerPrefab);
                    mSp.transform.SetParent(frame.transform);
                    mSp.transform.localPosition = new Vector3( i * -0.07f, 0);
                    mSp.transform.localScale = new Vector3(0.5f,0.5f,1f);
                }

                //第二行
                //var initate = 0.02f + (4 - markerTotal) * 0.075f;

                for (int i = 0; i < markerTotal-4; i++)
                {
                    var mSp = Instantiate(markerPrefab);
                    mSp.transform.SetParent(frame.transform);
                    mSp.transform.localPosition = new Vector3(i * -0.07f, -0.15f,-0.01f*i);
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
