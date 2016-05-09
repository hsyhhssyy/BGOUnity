using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Collider;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior
{
    [UsedImplicitly]
    public class SpecialTechDisplayBehavior:MonoBehaviour
    {
        public GameObject CivilFrame;
        public GameObject MilitaryFrame;
        public GameObject ExplorationFrame;
        public GameObject EngineeringFrame;

        public List<CardInfo> SpecialTechs;

        public void Refresh()
        {
            CivilFrame.SetActive(false);
            MilitaryFrame.SetActive(false);
            ExplorationFrame.SetActive(false);
            EngineeringFrame.SetActive(false);

            foreach (var info in SpecialTechs)
            {
                GameObject frame = null;
                switch (info.CardType)
                {
                    case CardType.SpecialTechCivil:
                        frame = CivilFrame;
                        break;
                    case CardType.SpecialTechEngineering:
                        frame = EngineeringFrame;
                        break;
                    case CardType.SpecialTechExploration:
                        frame = ExplorationFrame;
                        break;
                    case CardType.SpecialTechMilitary:
                        frame = MilitaryFrame;
                        break;
                    default:
                        continue;
                }

                frame.SetActive(true);
                frame.FindObject("AgeText").GetComponent<TextMesh>().text = info.CardAge.ToString();
                frame.GetComponent<CardNormalImagePreviewCollider>().Card=info;
            }
        }
    }
}
