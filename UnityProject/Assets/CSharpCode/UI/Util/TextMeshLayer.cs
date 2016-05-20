using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.UI.Util
{
    public
         class TextMeshLayer:MonoBehaviour
    {
        public String SortingLayer = "";

        public void Start()
        {
            var rend=this.gameObject.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                rend.sortingLayerName = SortingLayer;
            }
        }
    }
}
