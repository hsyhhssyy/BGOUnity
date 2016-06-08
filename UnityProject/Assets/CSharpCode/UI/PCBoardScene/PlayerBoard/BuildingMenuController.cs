using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.PlayerBoard;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    [UsedImplicitly]
    public class BuildingMenuController:MonoBehaviour
    {
        private BuildingCellController _controller;

        private String _animateStep = "CollapseComplete";
        
        private List<PlayerAction> _actions;

        private int _listedAction;
        private float _itemsToBeListed;
        private GameObject _itemPrefab;

        [UsedImplicitly]
        public float PopupSpeed=1f;
        [UsedImplicitly]
        public float CollapseSpeed = 1f;

        [UsedImplicitly]
        public void Start()
        {
            _itemPrefab = Resources.Load<GameObject>("Dynamic-PC/Menu/BuildingMenuItem");
            
        }

        [UsedImplicitly]
        public void Update()
        {
            var frames = _controller.ParentController.Frames.ToList();

            var index = frames.IndexOf(_controller.gameObject);

            #region Popup Animation
            if (_animateStep == "PreparePopup")
            {
                transform.localScale = new Vector3(0.3f, 1f, 1f);
                transform.localPosition = new Vector3(0.286f, frames[index].transform.localPosition.y + 0.025f, 0f);

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                _animateStep = "Popup";
            }else
            if (_animateStep == "Popup")
            {
                //popup menu
                float scale = transform.localScale.x + (0.8f*Time.deltaTime)* PopupSpeed;
                if (scale >= 1||scale<0)
                {
                    scale = 1;
                    _itemsToBeListed = 0;
                    _listedAction = 0;
                    _animateStep = "ListItem";
                }
                transform.localScale=new Vector3(scale,1f,1f);
                transform.localPosition= new Vector3(0.952f*scale, transform.localPosition.y, 0f);//0.526
                
                //如果有其他frame挡着卡，上移
                var maxY = frames[index].transform.localPosition.y + 1.06f;
                if (frames.Count > index + 1 && frames[index + 1].activeSelf)
                {
                    float locationY = frames[index + 1].transform.localPosition.y;
                    float delta = (1.59f * Time.deltaTime) * PopupSpeed;
                    if (locationY + delta > maxY)
                    {
                        delta = maxY - locationY;
                        if (delta < 0.001)
                        {
                            delta = 0;
                        }
                    }
                    for (var i = index + 1; i < frames.Count; i++)
                    {
                        frames[i].transform.localPosition =
                            new Vector3(frames[i ].transform.localPosition.x,
                                frames[i].transform.localPosition.y + delta,
                                frames[i].transform.localPosition.z);
                    }
                }
            }
            else
            if (_animateStep == "ListItem")
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(0.952f, transform.localPosition.y, 0f);
                _itemsToBeListed = _itemsToBeListed + (10f*Time.deltaTime)*PopupSpeed;

                for (var i = _listedAction; i < _itemsToBeListed&&i< _actions.Count; i++)
                {
                    var mSp = Instantiate(_itemPrefab);
                    mSp.transform.parent = gameObject.transform;
                    mSp.transform.localPosition=new Vector3(-0.594f, 0.429f - 0.18f*i,-0.001f);

                    mSp.transform.Find("MenuText").GetComponent<TextMesh>().text = _actions[i].GetDescription();
                    //mSp.GetComponent<PCBoardBindedActionClickTrigger>().Bind(Actions[i], BoardBehavior);

                    _listedAction = i+1;
                }

                if (_listedAction >= _actions.Count)
                {
                    _animateStep = "PopupComplete";
                }
            }

            #endregion

            #region Collapse

            if (_animateStep == "PrepareCollapse")
            {
                _animateStep = "RemoveMenu";
            }else if (_animateStep == "RemoveMenu")
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                _animateStep = "Collapse";
            }
            if (_animateStep == "Collapse")
            {
                float scale = transform.localScale.x - (0.8f * Time.deltaTime) * CollapseSpeed;
                if (scale < 0.3)
                {
                    scale = 0.3f;
                    _animateStep = "FrameDisappear";
                }
                transform.localScale = new Vector3(scale, 1f, 1f);
                transform.localPosition = new Vector3(0.952f * scale, transform.localPosition.y, 0f);//0.526

                //如果上一个frame被移动了，就移动回来
                float maxY =frames[index].transform.localPosition.y + 0.53f;
                if (frames.Count > index + 1 && frames[index + 1].transform.localPosition.y> maxY)
                {
                    float locationY = frames[index + 1].transform.localPosition.y;
                    float delta = (1.59f * Time.deltaTime) * CollapseSpeed;
                    if (locationY - delta < maxY)
                    {
                        delta = locationY - maxY;
                        if (delta < 0.001)
                        {
                            delta = 0;
                        }
                    }
                    for (int i = index + 1; i < frames.Count; i++)
                    {
                        frames[i].transform.localPosition =
                            new Vector3(frames[i].transform.localPosition.x,
                               frames[i].transform.localPosition.y - delta,
                                frames[i].transform.localPosition.z);
                    }
                }
            }
            if (_animateStep == "FrameDisappear")
            {
                if (frames.Count > index + 1)
                {
                    float delta = frames[index + 1].transform.localPosition.y -
                                  (frames[index].transform.localPosition.y + 0.53f);
                    for (int i = index + 1; i < frames.Count; i++)
                    {
                        frames[i].transform.localPosition =
                            new Vector3(frames[i].transform.localPosition.x,
                                frames[i].transform.localPosition.y - delta,
                                frames[i].transform.localPosition.z);
                    }
                }
                _animateStep = "CollapseComplete";
                transform.gameObject.SetActive(false);
            }

            #endregion
        }

        public void Popup(List<PlayerAction> actions, BuildingCellController controller)
        {
            if (_animateStep== "PopupComplete")
            {
                Collapse();
            }
            else if (_animateStep == "CollapseComplete")
            {
                _controller = controller;
                _animateStep = "PreparePopup";
                transform.gameObject.SetActive(true);
                _actions = actions;
            }
        }

        public void Collapse()
        {
            if (_animateStep == "PopupComplete")
            {
                _animateStep = "PrepareCollapse";
            }
        }
    }
}
