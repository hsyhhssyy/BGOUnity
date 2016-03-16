using UnityEngine;
using System.Collections;
using Assets.CSharpCode.UI;
using UnityEngine.SceneManagement;

public class GameButtonBehaviour : MonoBehaviour
{

    public int GameNumber;

    public void OnMouseUpAsButton()
    {
        SceneTransporter.CurrentGame = SceneTransporter.LastListedGames[GameNumber];

        SceneManager.LoadScene("Scene/BoardScene");
    }
}
