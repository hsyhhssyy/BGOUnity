using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.Util.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Effects
{
    public class PCBoardCardSmallHighlightEffect:MonoBehaviour
    {
        private GameObject _frame;
        public bool Highlight;

        public PCBoardCardSmallHighlightEffect()
        {
            
        }
        
        public void Update()
        {
            if (_frame == null)
            {
                _frame=new GameObject();
                _frame.transform.parent = gameObject.transform;
                _frame.transform.localPosition=new Vector3(0,0,-0.1f);
                _frame.AddComponent<SpriteRenderer>().sprite =
                    UnityResources.GetSprite("pc-board-card-small-highlight");
            }

            _frame.SetActive(Highlight);
        } 
    }
}
