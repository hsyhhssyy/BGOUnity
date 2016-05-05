using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    [UsedImplicitly]
    public class BuildingCellDisplayBehavior:MonoBehaviour
    {
        public GameObject[] Frames;

        public Dictionary<Age, BuildingCell> Cells;

        public GameObject CardPopupFrame;

        public void Refresh()
        {
            var civilopedia = TtaCivilopedia.GetCivilopedia(SceneTransporter.CurrentGame.Version);
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

                var civilCard = UnityResources.GetSprite("Card-Small-" + cellInfo.Card.InternalId);
                if (civilCard != null)
                {
                    Frames[i].GetComponent<SpriteRenderer>().sprite = civilCard;
                }

                //显示黄点
                var yellowPrefab = Resources.Load<GameObject>("Dynamic-PC/YellowMarker");
                DisplayMarker(cellInfo.Worker, yellowPrefab, Frames[i].FindObject("YellowMarkers"));

                //显示蓝点
                var bluePrefab = Resources.Load<GameObject>("Dynamic-PC/BlueMarker");
                DisplayMarker(cellInfo.Storage, bluePrefab, Frames[i].FindObject("BlueMarkers"));

                var popCollider=Frames[i].GetComponent<CardNormalImagePopupCollider>();
                popCollider.CardInternalId = cellInfo.Card.InternalId;
                popCollider.popup = CardPopupFrame;
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
                    mSp.transform.localPosition = new Vector3( i * 0.15f, 0);
                }

                //第二行
                //var initate = 0.02f + (4 - markerTotal) * 0.075f;

                for (int i = 0; i < markerTotal-4; i++)
                {
                    var mSp = Instantiate(markerPrefab);
                    mSp.transform.SetParent(frame.transform);
                    mSp.transform.localPosition = new Vector3(i * 0.15f, -0.15f);
                }
            }
            else
            {
                //显示Marker x N
            }
        }
    }
}
