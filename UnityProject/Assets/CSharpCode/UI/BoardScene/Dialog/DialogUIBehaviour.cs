using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.BoardScene.Dialog
{
    public class DialogUIBehaviour :MonoBehaviour
    {
        public GameObject DialogBoard;

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            DialogBoard.GetComponent<Animator>().SetTrigger("Collide");
        }
    }
}
