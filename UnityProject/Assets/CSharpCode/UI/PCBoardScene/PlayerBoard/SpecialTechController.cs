using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class SpecialTechController : TtaUIControllerMonoBehaviour
    {
        public GameObject CivilFrame;
        public GameObject MilitaryFrame;
        public GameObject ExplorationFrame;
        public GameObject EngineeringFrame;
        
        private bool _refreshRequired;

        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            Manager.Regiseter(this);
        }
        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
                _refreshRequired = true;
            }
        }

        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                Refresh();
            }
        }

        public void Refresh()
        {
            var specialTechs = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo].SpecialTechs;
            CivilFrame.SetActive(false);
            MilitaryFrame.SetActive(false);
            ExplorationFrame.SetActive(false);
            EngineeringFrame.SetActive(false);

            foreach (var info in specialTechs)
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
                frame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(info);
            }
        }
    }
}
