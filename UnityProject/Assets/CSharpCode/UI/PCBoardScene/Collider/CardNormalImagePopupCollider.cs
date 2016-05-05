using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Collider
{
    [UsedImplicitly]
    public class CardNormalImagePopupCollider:MonoBehaviour
    {
        public GameObject popup;
        public String CardInternalId;

        public void OnMouseEnter()
        {
            if (popup != null&& CardInternalId!=null)
            {
                var card =
                    TtaCivilopedia.GetCivilopedia(SceneTransporter.CurrentGame.Version).GetCardInfo(CardInternalId);

                Sprite civilCard = UnityResources.GetSprite("Card-Small-" + card.InternalId);

                if (civilCard != null)
                {
                    popup.GetComponent<SpriteRenderer>().sprite = civilCard;
                }
                else
                {
                    popup.FindObject("AgeText").GetComponent<TextMesh>().text = card.CardAge.ToString();
                    popup.FindObject("NameText").GetComponent<TextMesh>().text = card.CardName.WordWrap(10);
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

                popup.transform.position = pos;

                popup.SetActive(true);
            }
        }

        public void OnMouseExit()
        {
            if (popup != null)
            {
                popup.SetActive(false);
            }
        }
    }
}
