using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.CSharpCode.UI.Util.UnityEnhancement
{
    public static class StaticHelper
    {
        /// <summary>
        /// Remove every children from this transform.
        /// </summary>
        /// <param name="transform">transform to remove from</param>
        public static void RemoveAllChildren(this Transform transform)
        {
            if (transform == null || transform.gameObject == null) { return;}
            foreach (Transform t in transform)
            {
                Object.Destroy(t.gameObject);
            }
        }

        /// <summary>
        /// Remove every children from its transform.
        /// </summary>
        /// <param name="gameObject">object to remove from</param>
        public static void RemoveAllTransformChildren(this GameObject gameObject)
        {
            if (gameObject==null|| gameObject.transform == null) { return; }
            foreach (Transform t in gameObject.transform)
            {
                Object.Destroy(t.gameObject);
            }
        }
    }
}
