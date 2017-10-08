using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.CSharpCode.UI.LobbyScene
{
    public class RankingUiBehaviour:MonoBehaviour
    {
        public const String RankMode1Vs1NewExpantion = "RankMode1vs1NewExpantion";

        public Button Btn1Vs1Ranking;

        private bool Queueing1vs1 = false;
        private float _refreshIntervel = 0;

        public void Update()
        {
            if (_refreshIntervel >0)
            {
                _refreshIntervel -= Time.deltaTime;
                if (_refreshIntervel < 0)
                {
                    _refreshIntervel = 0;
                }
            }

            if (Math.Abs(_refreshIntervel) < 0.001)
            {
                _refreshIntervel = -1;
                StartCoroutine(SceneTransporter.Server.CheckRankedMatch((game) =>
                {
                    if (game != null)
                    {

                        SceneTransporter.CurrentGame = game;
                        SceneManager.LoadScene("Scene/BoardScene-PC");
                        return;
                    }
                    _refreshIntervel = 10;
                }));
            }

        }

        public void Btn1vs1Ranking_Click()
        {
            if (Queueing1vs1)
            {
                Btn1Vs1Ranking.gameObject.FindObject("Text").GetComponent<Text>().text = "1v1天梯（Beta)";
                StartCoroutine(SceneTransporter.Server.StopRanking(RankMode1Vs1NewExpantion,(b) =>
                {
                }));
            }
            else
            {
                Btn1Vs1Ranking.gameObject.FindObject("Text").GetComponent<Text>().text = "1v1比赛搜索中...";
                StartCoroutine(SceneTransporter.Server.StartRanking(RankMode1Vs1NewExpantion, (b) =>
                {
                }));

            }
            Queueing1vs1 = !Queueing1vs1;
        }
    }
}
