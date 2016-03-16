using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Experimental.Networking;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Translation;
using Assets.CSharpCode.UI;
using UnityEngine.SceneManagement;

//using UnityEngine.Experimental.Networking;

public class UIBehaviour : MonoBehaviour
{

    public GameObject LoadingGo;
    
    private readonly Dictionary<String,Sprite> dictSprites=new Dictionary<string, Sprite>();

    void Start()
    {
        LoadingGo.SetActive(true);

        var sprites = Resources.LoadAll<Sprite>("SpriteTile/CardRow_Sprite_CardBackground");

        foreach (Sprite sp in sprites)
        {
            dictSprites.Add(sp.name, sp);
        }
        
        
        StartCoroutine(GetCardRow());
        
    }

    public void BackButton_Clicked()
    {
        SceneManager.LoadScene("Scene/LobbyScene");
    }
    

    private IEnumerator GetCardRow()
    {
        return SceneTransporter.server.RefreshBoard(SceneTransporter.CurrentGame, () =>
        {
            DisplayGameBoard(SceneTransporter.CurrentGame);

            LoadingGo.SetActive(false);
        });
    }

    private void DisplayGameBoard(TtaGame game)
    {
        System.Random rand = new System.Random(System.DateTime.Now.Second);
        int cardIndex = 0;
        foreach(var card in game.CardRow)
        {
            var cardGo = GameObject.Find("CardRow/CardRow-Card"+ cardIndex.ToString());

            String spriteName = "CardRow_Sprite_CardBackground_" + rand.Next(0, 21);
            var rend = cardGo.GetComponent<SpriteRenderer>();
            var sprite = dictSprites[spriteName];
            rend.sprite = sprite;

            var textGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString()+"/NameText");
            var textMesh = textGo.GetComponent<TextMesh>();
            
            textMesh.text = TtaTranslation.GetTranslatedText(card.InternalId.Split("-".ToCharArray(),2)[1]).WordWrap(15).Trim();

            var ageGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/AgeText");
            textMesh = ageGo.GetComponent<TextMesh>();

            textMesh.text =
                Enum.GetName(
                    typeof (Age),
                    Convert.ToInt32(
                        card.InternalId.Split("-".ToCharArray(), 2)[0]));
            

            cardIndex++;
        }
    }
}
