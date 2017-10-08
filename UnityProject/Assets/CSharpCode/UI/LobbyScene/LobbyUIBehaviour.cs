using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Network.Bgo;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.LobbyScene
{
    [UsedImplicitly]
    public class LobbyUIBehaviour : MonoBehaviour
    {
        public GameObject LoadingGo;

        private List<TtaGame> _games;

        [UsedImplicitly]
        private void Start()
        {
            LoadingGo.SetActive(true);
            StartCoroutine(ListGames());
        }

        
        private IEnumerator ListGames()
        {
            yield return StartCoroutine(SceneTransporter.Server.ListGames(
            gamesReturn =>
            {
                _games = gamesReturn;

               Assets.CSharpCode.UI.Util.LogRecorder.Log("List Get!");

                SceneTransporter.LastListedGames = new List<TtaGame>();

                int gameNumber = 0;
                foreach (var game in _games)
                {
                    if (gameNumber > 5)
                    {
                        break;
                    }

                    var gameIdGo = GameObject.Find("GameLists/Lobby-Game" + gameNumber.ToString() + "/GameID");
                    var textMesh = gameIdGo.GetComponent<TextMesh>();
                    textMesh.text = "--";

                    var gameNameGo = GameObject.Find("GameLists/Lobby-Game" + gameNumber.ToString() + "/GameName");
                    textMesh = gameNameGo.GetComponent<TextMesh>();
                    textMesh.text = game.Name;

                    SceneTransporter.LastListedGames.Add(game);

                    gameNumber++;
                }
                for (int i = gameNumber; i <= 5; i++)
                {
                    var go=GameObject.Find("GameLists/Lobby-Game" + i);
                    go.SetActive(false);
                }

                LoadingGo.SetActive(false);
            }));
        }
    }
}
