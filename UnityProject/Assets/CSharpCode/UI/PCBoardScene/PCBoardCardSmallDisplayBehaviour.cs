using System;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [UsedImplicitly]
    public class PCBoardCardSmallDisplayBehaviour:MonoBehaviour
    {
        private bool _refreshRequired=true;

        private GameObject NormalImagePopup;
        private CardInfo Card;

        public bool DisablePopup;

        public GameObject ActionCardFrame;
        public GameObject BuildingCardFrame;

        /// <summary>
        /// 这是用于Popup预览图的，预览图是一个根据大图Prefab现场生成
        /// </summary>
        [UsedImplicitly]
        public void OnMouseEnter()
        {
            if (NormalImagePopup != null&& Card.InternalId!=null)
            {
                var card =
                    TtaCivilopedia.GetCivilopedia(SceneTransporter.CurrentGame.Version).GetCardInfo(Card.InternalId);

                Sprite civilCard = UnityResources.GetSprite(card.NormalImage);

                if (civilCard != null)
                {
                    NormalImagePopup.GetComponent<SpriteRenderer>().sprite = civilCard;
                    NormalImagePopup.FindObject("AgeText").GetComponent<TextMesh>().text = "";
                    NormalImagePopup.FindObject("NameText").GetComponent<TextMesh>().text = "";
                }
                else
                {
                    NormalImagePopup.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("no-card-image-normal"); ;
                    NormalImagePopup.FindObject("AgeText").GetComponent<TextMesh>().text = card.CardAge.ToString();
                    NormalImagePopup.FindObject("NameText").GetComponent<TextMesh>().text = card.CardName.WordWrap(10);
                }

                Vector3 pos = Input.mousePosition;
                
                pos = Camera.main.ScreenToWorldPoint(pos);

                if (pos.x < 0)
                {
                    pos.x = pos.x + 1.4f;
                }
                else
                {
                    pos.x = pos.x - 1.4f;
                }

                if (pos.y < 0)
                {
                    pos.y = pos.y + 2f;
                }
                else
                {
                    pos.y = pos.y - 2f;
                }

                pos.z = -9;

                NormalImagePopup.transform.position = pos;

                NormalImagePopup.SetActive(true);
            }
        }

        /// <summary>
        /// 这是用于Popup预览图的，预览图是一个根据大图Prefab现场生成
        /// </summary>
        [UsedImplicitly]
        public void OnMouseExit()
        {
            if (NormalImagePopup != null)
            {
                NormalImagePopup.SetActive(false);
            }
        }

        /// <summary>
        /// 每当卡牌变更的时候，重新激活一次刷新
        /// </summary>
        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired)
            {
                Refresh();
            }
        }


        public void Bind(CardInfo card, Transform parentTransform, Vector3 newLocation)
        {
            this.Card = card;
            this.gameObject.transform.SetParent(parentTransform);
            this.gameObject.transform.localPosition = newLocation;
            _refreshRequired = true;
        }

        public void Bind(CardInfo card)
        {
            this.Card = card;
            _refreshRequired = true;
        }


        private void Refresh()
        {
            if (Card == null)
            {
                return;
            }

            switch (Card.CardType)
            {
                    case CardType.Action:
                    DisplayAction();
                    break;
                case CardType.UrbanTechArena:
                    case CardType.UrbanTechLab:
                    case CardType.UrbanTechLibrary:
                    case CardType.UrbanTechTemple:
                    case CardType.UrbanTechTheater:
                    DisplayUrban();
                    break;
            }
        }

        private void DisplayAction()
        {
            //Get action background
            var sp=UnityResources.GetSprite("pc-board-card-small-government-background");
            ActionCardFrame.SetActive(true);
            ActionCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            ActionCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            ActionCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(5);
            ActionCardFrame.FindObject("Description").GetComponent<TextMesh>().text = Card.Description.WordWrap(5);

        }

        private void DisplayUrban()
        {
            //Get action background
            var sp = UnityResources.GetSprite("pc-board-card-small-urban-background");
            BuildingCardFrame.SetActive(true);
            BuildingCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            BuildingCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            BuildingCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(5);
            
            BuildingCardFrame.FindObject("TechCost").GetComponent<TextMesh>().text = Card.ResearchCost[0].ToString();
            BuildingCardFrame.FindObject("ResourceCost").GetComponent<TextMesh>().text = Card.BuildCost[0].ToString();
        }
    }
}
