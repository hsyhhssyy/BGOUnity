using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.UI.LobbyScene
{
    public class LobbyUIBehaviour : MonoBehaviour
    {
        public GameObject LoadingGo;

        private List<TtaGame> Games;

        // Use this for initialization
        private void Start()
        {
            LoadingGo.SetActive(true);
            StartCoroutine(ListGames());
        }

        // Update is called once per frame
        private IEnumerator ListGames()
        {
            yield return StartCoroutine(SceneTransporter.server.ListGames(
            gamesReturn =>
            {
                Games = gamesReturn;

                Debug.Log("List Get!");

                SceneTransporter.LastListedGames = new List<TtaGame>();

                int gameNumber = 0;
                foreach (var game in Games)
                {
                    var bgoGame = (BgoGame) game;
                    if (gameNumber > 5)
                    {
                        break;
                    }

                    var gameIDGo = GameObject.Find("GameLists/Lobby-Game" + gameNumber.ToString() + "/GameID");
                    var textMesh = gameIDGo.GetComponent<TextMesh>();
                    textMesh.text = bgoGame.GameId;

                    var gameNameGo = GameObject.Find("GameLists/Lobby-Game" + gameNumber.ToString() + "/GameName");
                    textMesh = gameNameGo.GetComponent<TextMesh>();
                    textMesh.text = bgoGame.Name;

                    SceneTransporter.LastListedGames.Add(game);

                    gameNumber++;
                }

                LoadingGo.SetActive(false);
            }));
        }
    }
}
