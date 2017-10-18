using System.Collections;
using Assets.CSharpCode.Entity;
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
            var game = SceneTransporter.LastListedGames[GameNumber];
            SceneTransporter.CurrentGame = game;

            SceneManager.LoadScene("Scene/BoardScene-PC");
            //StartCoroutine(LoadGame(game));
        }

        private IEnumerator LoadGame(TtaGame game)
        {
            return SceneTransporter.Server.RefreshBoard(game, (error) =>
            {
                SceneManager.LoadScene("Scene/BoardScene-PC");
            });
        }
    }
}
