using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.BoardScene
{
    [UsedImplicitly]
    public class CardRowButtonBehaviour : MonoBehaviour
    {

        public TextMesh AgeText;
        public TextMesh NameText;

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            NameText.text = "Clicked!";
        }
    }
}
