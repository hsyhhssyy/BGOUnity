using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class WhiteRedMarkerController : TtaUIControllerMonoBehaviour
    {
        public GameObject WhiteMarkerFrame;
        public GameObject RedMarkerFrame ;

        private bool _refreshRequired;

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            Manager.GameBoardManagerEvent += OnSubscribedGameEvents;
            Manager.Regiseter(this);
           
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
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        public void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];

            //framename是指ActionPoint的frame
            var missingPrefab = Resources.Load<GameObject>("Dynamic-PC/MissingMarker");
            var whitePrefab = Resources.Load<GameObject>("Dynamic-PC/WhiteMarker");
            var redPrefab = Resources.Load<GameObject>("Dynamic-PC/RedMarker");

            DisplayActionPoint(WhiteMarkerFrame,
                board.Resource[ResourceType.WhiteMarkerMax] - board.Resource[ResourceType.WhiteMarker],
                board.Resource[ResourceType.WhiteMarker], whitePrefab, missingPrefab);

            DisplayActionPoint(RedMarkerFrame,
                board.Resource[ResourceType.RedMarkerMax] - board.Resource[ResourceType.RedMarker],
                board.Resource[ResourceType.RedMarker], redPrefab, missingPrefab);
        }
        private void DisplayActionPoint(GameObject civilActionFrame, int missingCount, int markerCount,
            GameObject whitePrefab, GameObject missingPrefab)
        {
            foreach (Transform child in civilActionFrame.transform)
            {
                Destroy(child.gameObject);
            }
            //确定第一行

            var markerTotal = missingCount + markerCount;

            if (markerTotal > 6)
            {
                //2行的第一行
                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb = null;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(0.02f + i * 0.15f, -0.075f);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }

                //第二行
                var initate = 0.02f + (6 - markerCount - missingCount) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else if (missingCount > 0)
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.225f);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
            }
            else
            {
                var initate = 0.02f + (6 - markerCount - missingCount) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else if (missingCount > 0)
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.15f);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
            }
        }

    }
}
