using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.TestScene
{
    public class TestSceneColliders:MonoBehaviour
    {
        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            var Animaion1Go = GameObject.Find("TestAnimations/Animate1Sprite");
            Animaion1Go.SetActive(true);
            Animaion1Go.transform.localPosition = new Vector3(1, 0);
            var anime = Animaion1Go.GetComponent<Animator>();
            anime.SetTrigger("Play");
           Assets.CSharpCode.UI.Util.LogRecorder.Log("AnimationDone");
        }
    }
}
