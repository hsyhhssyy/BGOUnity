using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CSharpCode.UI.LobbyScene
{
    [UsedImplicitly]
    public class GameButtonBehaviour : MonoBehaviour
    {

        public int GameNumber;

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            SceneTransporter.CurrentGame = SceneTransporter.LastListedGames[GameNumber];

            SceneManager.LoadScene("Scene/BoardScene");
        }
    }
}
