using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.Util.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Effects
{
    /// <summary>
    /// 使用方法:<para/>
    /// private PCBoardCardSmallHighlightEffect highlightGo;<para/>
    /// public void Start()<para/>
    /// {<para/>
    ///    highligtGo=gameObject.AddComponent&lt;PCBoardCardSmallHighlightEffect&gt;();<para/>
    /// }<para/>
    /// </summary>
    /// 
    /// 
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
