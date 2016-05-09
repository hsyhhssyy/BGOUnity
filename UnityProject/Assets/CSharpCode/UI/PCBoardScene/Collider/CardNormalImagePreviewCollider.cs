using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Collider
{
    public class CardNormalImagePreviewCollider : MonoBehaviour
    {
        private bool _refreshRequired = true;

        private GameObject NormalImagePopup;
        public CardInfo Card;

        public bool DisablePopup;

        /// <summary>
        /// 这是用于Popup预览图的，预览图是一个根据大图Prefab现场生成
        /// </summary>
        [UsedImplicitly]
        public void OnMouseEnter()
        {
            if (NormalImagePopup != null && Card.InternalId != null)
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

        private void Refresh()
        {

        }
    }
}
