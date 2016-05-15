using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Menu
{
    public class BuildingMenuAnimationBehaviour:MonoBehaviour
    {
        public BuildingCellDisplayBehavior DisplayBehavior;

        private String animateStep = "";

        private List<PlayerAction> Actions;

        private int ListedAction = 0;
        private float ItemsToBeListed = 0;

        private GameObject ItemPrefab;

        public void Start()
        {
            ItemPrefab = Resources.Load<GameObject>("Dynamic-PC/Menu/BuildUpgradeMenu");
            
        }

        public void Update()
        {
            if (animateStep == "PreparePopup")
            {
                transform.localScale = new Vector3(0.3f, 1f, 1f);
                transform.localPosition = new Vector3(0.054f, 0.025f, 1f);
                transform.gameObject.SetActive(true);

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                animateStep = "Popup";
            }else
            if (animateStep == "Popup")
            {
                float scale = transform.localScale.x + (0.8f/Time.deltaTime);
                if (scale >= 1)
                {
                    scale = 1;
                    ItemsToBeListed = 0;
                    ListedAction = 0;
                    animateStep = "ListItem";
                }
                transform.localScale=new Vector3(scale,1f,1f);
                transform.localPosition= new Vector3(0.817f*scale-0.191f, 0.025f, 1f);//0.526
            }
            else
            if (animateStep == "ListItem")
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(0.626f, 0.025f, 1f);
                ItemsToBeListed = ItemsToBeListed + (0.5f/Time.deltaTime);

                for (int i = ListedAction; i < ItemsToBeListed&&i< Actions.Count; i++)
                {
                    var mSp = Instantiate(ItemPrefab);
                    mSp.transform.parent = this.gameObject.transform;
                    mSp.transform.localPosition=new Vector3(-0.275f,0.45f-0.18f*i,-0.001f);

                    ListedAction = i+1;
                }

                if (ListedAction >= Actions.Count)
                {
                    animateStep = "PopupComplete";
                }
            }
        }

        public void Popup(int index,List<PlayerAction> actions)
        {
            animateStep = "Popup";
            Actions = actions;
        }

        public void Collapse()
        {
            
        }
    }
}
