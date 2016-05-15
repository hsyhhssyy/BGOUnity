using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    public class BuildingMenuAnimationBehaviour:MonoBehaviour
    {
        public BuildingCellDisplayBehavior DisplayBehavior;
        public BuildingCellActionBinder ActionBinder;

        private String animateStep = "";

        private int index = 0; 
        private List<PlayerAction> Actions;

        private int ListedAction = 0;
        private float ItemsToBeListed = 0;
        private PCBoardBehavior BoardBehavior;
        private GameObject ItemPrefab;

        public float popupSpeed=1f;
        public float collapseSpeed = 1f;

        public void Start()
        {
            ItemPrefab = Resources.Load<GameObject>("Dynamic-PC/Menu/BuildingMenuItem");
            
        }

        public void Update()
        {
            #region Popup Animation
            if (animateStep == "PreparePopup")
            {
                transform.localScale = new Vector3(0.3f, 1f, 1f);
                transform.localPosition = new Vector3(0.054f, DisplayBehavior.Frames[index].transform.localPosition.y + 0.025f, 0f);

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                animateStep = "Popup";
            }else
            if (animateStep == "Popup")
            {
                //popup menu
                float scale = transform.localScale.x + (0.8f*Time.deltaTime)* popupSpeed;
                if (scale >= 1)
                {
                    scale = 1;
                    ItemsToBeListed = 0;
                    ListedAction = 0;
                    animateStep = "ListItem";
                }
                transform.localScale=new Vector3(scale,1f,1f);
                transform.localPosition= new Vector3(0.817f*scale-0.191f, transform.localPosition.y, 0f);//0.526
                
                //如果有其他frame挡着卡，上移
                float maxY = DisplayBehavior.Frames[index].transform.localPosition.y + 1.06f;
                if (DisplayBehavior.Frames.Length > index + 1 && DisplayBehavior.Frames[index + 1].activeSelf == true)
                {
                    float locationY = DisplayBehavior.Frames[index + 1].transform.localPosition.y;
                    float delta = (1.59f * Time.deltaTime) * popupSpeed;
                    if (locationY + delta > maxY)
                    {
                        delta = maxY - locationY;
                        if (delta < 0.001)
                        {
                            delta = 0;
                        }
                    }
                    for (int i = index + 1; i < DisplayBehavior.Frames.Length; i++)
                    {
                        DisplayBehavior.Frames[i].transform.localPosition =
                            new Vector3(DisplayBehavior.Frames[i ].transform.localPosition.x,
                                DisplayBehavior.Frames[i].transform.localPosition.y + delta,
                                DisplayBehavior.Frames[i].transform.localPosition.z);
                    }
                }
            }
            else
            if (animateStep == "ListItem")
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(0.626f, transform.localPosition.y, 0f);
                ItemsToBeListed = ItemsToBeListed + (10f*Time.deltaTime)*popupSpeed;

                for (int i = ListedAction; i < ItemsToBeListed&&i< Actions.Count; i++)
                {
                    var mSp = Instantiate(ItemPrefab);
                    mSp.transform.parent = this.gameObject.transform;
                    mSp.transform.localPosition=new Vector3(-0.275f,0.45f-0.18f*i,-0.001f);

                    mSp.transform.Find("MenuText").GetComponent<TextMesh>().text = Actions[i].GetDescription();
                    mSp.GetComponent<PCBoardBindedActionClickTrigger>().Action = Actions[i];
                    mSp.GetComponent<PCBoardBindedActionClickTrigger>().BoardBehavior = BoardBehavior;

                    ListedAction = i+1;
                }

                if (ListedAction >= Actions.Count)
                {
                    animateStep = "PopupComplete";
                }
            }

            #endregion

            #region Collapse

            if (animateStep == "PrepareCollapse")
            {
                animateStep = "RemoveMenu";
            }else if (animateStep == "RemoveMenu")
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                animateStep = "Collapse";
            }
            if (animateStep == "Collapse")
            {
                float scale = transform.localScale.x - (0.8f * Time.deltaTime) * collapseSpeed;
                if (scale >= 0.3)
                {
                    scale = 0.3f;
                    animateStep = "FrameDisappear";
                }
                transform.localScale = new Vector3(scale, 1f, 1f);
                transform.localPosition = new Vector3(0.817f * scale - 0.191f, transform.localPosition.y, 0f);//0.526

                //如果上一个frame被移动了，就移动回来
                float maxY = DisplayBehavior.Frames[index].transform.localPosition.y + 0.53f;
                if (DisplayBehavior.Frames.Length > index + 1 && DisplayBehavior.Frames[index + 1].transform.localPosition.y> maxY)
                {
                    float locationY = DisplayBehavior.Frames[index + 1].transform.localPosition.y;
                    float delta = (1.59f * Time.deltaTime) * collapseSpeed;
                    if (locationY - delta < maxY)
                    {
                        delta = locationY - maxY;
                        if (delta < 0.001)
                        {
                            delta = 0;
                        }
                    }
                    for (int i = index + 1; i < DisplayBehavior.Frames.Length; i++)
                    {
                        DisplayBehavior.Frames[i].transform.localPosition =
                            new Vector3(DisplayBehavior.Frames[i].transform.localPosition.x,
                                DisplayBehavior.Frames[i].transform.localPosition.y - delta,
                                DisplayBehavior.Frames[i].transform.localPosition.z);
                    }
                }
            }
            if (animateStep == "FrameDisappear")
            {
                if (DisplayBehavior.Frames.Length > index + 1)
                {
                    float delta = DisplayBehavior.Frames[index + 1].transform.localPosition.y -
                                  (DisplayBehavior.Frames[index].transform.localPosition.y + 0.53f);
                    for (int i = index + 1; i < DisplayBehavior.Frames.Length; i++)
                    {
                        DisplayBehavior.Frames[i].transform.localPosition =
                            new Vector3(DisplayBehavior.Frames[i].transform.localPosition.x,
                                DisplayBehavior.Frames[i].transform.localPosition.y - delta,
                                DisplayBehavior.Frames[i].transform.localPosition.z);
                    }
                }
                animateStep = "CollapseComplete";
                transform.gameObject.SetActive(false);
            }

            #endregion
        }

        public void Popup(int ind,List<PlayerAction> actions, PCBoardBehavior BoardBehavior)
        {
            this.BoardBehavior = BoardBehavior;
            if (animateStep == "PopupComplete")
            {
                Collapse();
            }
            else
            {
                index = ind;
                animateStep = "PreparePopup";
                transform.gameObject.SetActive(true);
                Actions = actions;
            }
        }

        public void Collapse()
        {
            animateStep = "PrepareCollapse";
        }
    }
}
