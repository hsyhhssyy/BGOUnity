using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    public class WarningDisplayBehaviour:MonoBehaviour
    {
        public SpriteRenderer warningImg;
        public SpriteRenderer markerImg;

        private GameObject _warningPopup;
        private string _warningText="";

        public void DisplayWarning(Warning warning,GameObject warningPopup)
        {
            this._warningPopup = warningPopup;
            switch (warning.Type)
            {
                case WarningType.WarOverTerritory:
                case WarningType.WarOverTechnology:
                case WarningType.WarOverCulture:
                    warningImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-military");

                    var color = new[] { "e89d0e", "e00ee8", "2ce80e", "b4b4b4" };
                    var colorText = new[] { "橙色", "紫色", "绿色", "灰色" };
                    var warText = new[] {"领土战争", "文化战争", "资源战争"};
                    var warMarker = new[] { "yellow", "culture", "resource" };
                    _warningText = "与<color=#"+ color [Convert.ToInt32(warning.Data)] + ">" + colorText [Convert.ToInt32(warning.Data)]+ "</color>的" + warText[((int) warning.Type) - 1];
                    markerImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-"+warMarker[((int)warning.Type) - 1]);
                    break;
               case WarningType.Corruption:
                    _warningText = "腐败 "+ warning.Data+"级";
                    warningImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-resource");
                    warningImg.transform.localScale=new Vector3(0.4f,0.6f,1);
                    markerImg.gameObject.SetActive(false);
                    break;
                    case WarningType.CivilDisorder:
                    _warningText = "暴动";
                    warningImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-unhappy");
                    markerImg.gameObject.SetActive(false);
                    break;
                case WarningType.Famine:
                    _warningText = "饥荒";
                    warningImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-food");
                    markerImg.gameObject.SetActive(false);
                    break;
                case WarningType.LastTurn:
                    _warningText = "最后一回合";
                    warningImg.sprite = UnityResources.GetSprite("SpriteTile/UI/icon-science");
                    markerImg.gameObject.SetActive(false);
                    break;
                default:
                    markerImg.gameObject.SetActive(false);
                    _warningPopup = null;
                    break;
            }
        }

        public void OnMouseEnter()
        {
            if (_warningPopup == null)
            {
                return;
            }
            _warningPopup.SetActive(true);
            _warningPopup.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.26f,
                -3);
            _warningPopup.FindObject("Text").GetComponent<TextMesh>().text = _warningText;
        }

        public void OnMouseExit()
        {
            if (_warningPopup == null)
            {
                return;
            }
            _warningPopup.SetActive(false);
        }
    }
}
