using UnityEngine;
using System.Collections;

namespace Assets.CSharpCode.UI
{
    public class CardRowButtonBehaviour : MonoBehaviour
    {

        public TextMesh AgeText;
        public TextMesh NameText;

        public void OnMouseUpAsButton()
        {
            NameText.text = "Clicked!";
        }
    }
}
