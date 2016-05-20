using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhaseDialog;
using Assets.CSharpCode.UI.PCBoardScene.DisplayBehavior;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    public class PCBoardDisplayBehavior:MonoBehaviour
    {
        public PCBoardBehavior BoardBehavior;

        #region Unity Game Objects

        public GameObject BackgroundSpriteGo;
        
        public GameObject LeaderFrame;
        public GameObject GovernmentFrame;

        public GameObject[] BlueBankMarkers;
        public GameObject[] YellowBankMarkers;

        public GameObject WhiteMarkerFrame;
        public GameObject RedMarkerFrame;

        public GameObject CivilHandCardFrame;
        public GameObject MilitaryHandCardFrame;

        public GameObject CompletedWondersFrame;
        public GameObject ConstructingWonderFrame;

        public GameObject ColonyFrame;


        public GameObject BuildingCellFrame;

        public SpecialTechDisplayBehavior SpecialTechFrame;

        public GameObject TacticOnBoardFrame;
        public TacticsPopupDisplayBehaviour TacticFrame;

        public CardRowDisplayBehaviour CardRowFrame;

        public EventsDisplayBehaviour EventsFrame;

        public GameObject CivilCardLeft;
        public GameObject MilitaryCardLeft;
        public GameObject CivilCardAge;
        public GameObject MilitaryCardAge;

        public GameJournalDisplayBehaviour JournalFrame;

        public GameObject WarningFrame;
        public GameObject WarningPopupFrame;

        public GameObject WorkerPoolFrame;

        //Dialogs and Menus
        public PoliticalPhaseDialogDisplayBehaviour PoliticalPhaseDialogFrame;

        #endregion

        [UsedImplicitly]
        //用update会导致弹出窗口的OnMouseExit捕捉不到
        public void Refresh()
        {
            if (!SceneTransporter.IsCurrentGameRefreshed())
            {
                //当前游戏不完整，不刷新
                ////此处应该抛出异常
                return;
            }

            if (SceneTransporter.CurrentGame.Boards.Count <= BoardBehavior.CurrentPlayerBoardIndex)
            {
                //当前玩家编号出现错误，不刷新
                //此处应该抛出异常
                return;
            }

            TtaBoard board = SceneTransporter.CurrentGame.Boards[BoardBehavior.CurrentPlayerBoardIndex];
            
            DisplayCardRow(SceneTransporter.CurrentGame.CardRow);
            DisplayEventsAndCardCounts(SceneTransporter.CurrentGame);

            //如果当前阶段是PoliticalPhase，那么就展示PoliticalPhase
            if (SceneTransporter.CurrentGame.CurrentPhase == TtaPhase.PoliticalPhase)
            {
                //不一定你是当前玩家，因为有可能别人在他的政治行动阶段打你呢
                //或者你正在处理一个事件
                if (SceneTransporter.CurrentGame.PossibleActions.Count > 0)
                {
                    DisplayPoliticalPhaseDialogs(SceneTransporter.CurrentGame);
                }
                else
                {
                    HideAllPoliticalPhaseDialogs();
                }
            }
            else
            {
                HideAllPoliticalPhaseDialogs();
            }

            var backgroundSpriteName = "SpriteTile/PCBoard/pc-board-player-background-" + "orange,purple,green,grey".Split(",".ToCharArray())[BoardBehavior.CurrentPlayerBoardIndex];
            var backgroundsp = UnityResources.GetSprite(backgroundSpriteName);
            BackgroundSpriteGo.GetComponent<SpriteRenderer>().sprite = backgroundsp;
            

            JournalFrame.Refresh();
            
            //展示用代码
            

            DisplayYellowBlueBank(board);

            DisplayWhiteRedMarkers(board);

            DisplayGovenrment(board);
            DisplayLeader(board);

            DisplayCivilHandCard(board);
            DisplayMilitaryHandCard(board);

            DisplayWonders(board);
            DisplayColony(board);

            DisplayBuildings(board);

            DisplaySpecialTech(board);

            DisplayWarnings(board);

            DisplayTactics(board);

            DisplayWorkerPool(board);
        }

        private void DisplayEventsAndCardCounts(TtaGame game)
        {
            EventsFrame.Refresh();

            CivilCardLeft.GetComponent<TextMesh>().text = game.CivilCardsRemain.ToString();
            MilitaryCardLeft.GetComponent<TextMesh>().text = game.MilitaryCardsRemain.ToString();

            CivilCardAge.GetComponent<TextMesh>().text = game.CurrentAge.ToString();
            MilitaryCardAge.GetComponent<TextMesh>().text = game.CurrentAge.ToString();
            
        }
        private void DisplayCardRow(List<CardRowCardInfo> cardRow)
        {
            CardRowFrame.CardRow = cardRow;
            CardRowFrame.Refresh();
        }

        private void DisplayGovenrment(TtaBoard board)
        {
            GovernmentFrame.FindObject("Name").GetComponent<TextMesh>().text = board.Government.CardName;
            GovernmentFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.Government);
        }

        private void DisplayLeader(TtaBoard board)
        {
            if (board.Leader == null)
            {
                LeaderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(null);
                LeaderFrame.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("leader_image_no_leader");
            }
            else
            {
                var sprite = UnityResources.GetSprite(board.Leader.SpecialImage) ??
                             UnityResources.GetSprite("leader_unknown");
                LeaderFrame.GetComponent<SpriteRenderer>().sprite = sprite;
                LeaderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.Leader);
            }

        }

        private void DisplayYellowBlueBank(TtaBoard board)
        {
            //蓝点
            int blueMarkerOwn = board.Resource[ResourceType.BlueMarker];
            for (int blueMarkerDisplay = 15;
                blueMarkerOwn >= 0 || blueMarkerDisplay >= 0;
                blueMarkerDisplay--, blueMarkerOwn--)
            {
                if (blueMarkerDisplay >= 0)
                {
                    var bankGo = BlueBankMarkers[15-blueMarkerDisplay];
                    bankGo.SetActive(blueMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }

            //黄点
            int yellowMarkerOwn = board.Resource[ResourceType.YellowMarker];
            for (int yellowMarkerDisplay = 17;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = YellowBankMarkers[17-yellowMarkerDisplay];
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }
        }

        private void DisplayWhiteRedMarkers(TtaBoard board)
        {
            //framename是指ActionPoint的frame
            var missingPrefab = Resources.Load<GameObject>("Dynamic-PC/MissingMarker");
            var whitePrefab = Resources.Load<GameObject>("Dynamic-PC/WhiteMarker");
            var redPrefab = Resources.Load<GameObject>("Dynamic-PC/RedMarker");

            DisplayActionPoint(WhiteMarkerFrame,
                board.Resource[ResourceType.WhiteMarkerMax] - board.Resource[ResourceType.WhiteMarker],
                board.Resource[ResourceType.WhiteMarker], whitePrefab, missingPrefab);

            DisplayActionPoint(RedMarkerFrame,
                board.Resource[ResourceType.RedMarkerMax] - board.Resource[ResourceType.RedMarker],
                board.Resource[ResourceType.RedMarker], redPrefab, missingPrefab);
        }

        private void DisplayActionPoint(GameObject civilActionFrame, int missingCount, int markerCount,
            GameObject whitePrefab, GameObject missingPrefab)
        {
            foreach (Transform child in civilActionFrame.transform)
            {
                Destroy(child.gameObject);
            }
            //确定第一行

            var markerTotal = missingCount + markerCount;

            if (markerTotal > 6)
            {
                //2行的第一行
                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb = null;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(0.02f + i * 0.15f, -0.075f);
                    mSp.transform.localScale = new Vector3(0.5f,0.5f,1f);
                }

                //第二行
                var initate = 0.02f + (6 - markerCount - missingCount) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else if (missingCount > 0)
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.225f);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
            }
            else
            {
                var initate = 0.02f + (6 - markerCount - missingCount) * 0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerCount > 0)
                    {
                        markerCount--;
                        prefeb = whitePrefab;
                    }
                    else if (missingCount > 0)
                    {
                        missingCount--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i * 0.15f, -0.15f);
                    mSp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
            }
        }

        private void DisplayCivilHandCard(TtaBoard board)
        {
            var unknownCardPrefab = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in CivilHandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.CivilCards.Count > 5)
            {
                incr = 0.7f*4/(board.CivilCards.Count-1);
            }

            for (int i = 0; i < board.CivilCards.Count; i++)
            {
                Instantiate(unknownCardPrefab).GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(board.CivilCards[i], CivilHandCardFrame.transform, new Vector3(i*incr, 0,-0.1f*i));

            }
        }
        private void DisplayMilitaryHandCard(TtaBoard board)
        {
            var unknownCardPrefab = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");

            foreach (Transform child in MilitaryHandCardFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.MilitaryCards.Count > 5)
            {
                incr = 0.7f * 4 / (board.MilitaryCards.Count-1);
            }

            for (int i = 0; i < board.MilitaryCards.Count; i++)
            {
                Instantiate(unknownCardPrefab).GetComponent<PCBoardCardDisplayBehaviour>()
                    .Bind(board.MilitaryCards[i], MilitaryHandCardFrame.transform, new Vector3(i * incr, 0, -0.1f * i));
            }
        }

        private void DisplayWonders(TtaBoard board)
        {

            //建造中
            if (board.ConstructingWonder != null)
            {
                ConstructingWonderFrame.SetActive(true);
                ConstructingWonderFrame.FindObject("WonderName").GetComponent<TextMesh>().text =
                    board.ConstructingWonder.CardName;
                var stepFrame = ConstructingWonderFrame.FindObject("Steps");

                foreach (Transform child in stepFrame.transform)
                {
                    Destroy(child.gameObject);
                }

                var stepPrefab = Resources.Load<GameObject>("Dynamic-PC/WonderBuildingStage");

                float init = -0.16f*board.ConstructingWonderSteps.Count+0.16f;
                for (int index = 0; index < board.ConstructingWonderSteps.Count; index++)
                {
                    var str = board.ConstructingWonderSteps[index];

                    var mSp = Instantiate(stepPrefab);
                    if (str == "X")
                    {
                        mSp.FindObject("StepText").SetActive(false);
                    }
                    else
                    {
                        mSp.FindObject("BlueMarker").SetActive(false);
                        mSp.FindObject("StepText").GetComponent<TextMesh>().text = str;
                    }
                    mSp.transform.parent = stepFrame.transform;
                    mSp.transform.localPosition = new Vector3(init+0.16f*index, 0,0);
                }

                ConstructingWonderFrame.GetComponent<PCBoardCardDisplayBehaviour>().Bind(board.ConstructingWonder);
            }
            else
            {
                ConstructingWonderFrame.SetActive(false);
            }

            //建造完成
            var wonderPrefab = Resources.Load<GameObject>("Dynamic-PC/CompletedWonder");

            foreach (Transform child in CompletedWondersFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.CompletedWonders.Count; i++)
            {
                var wonderCard = board.CompletedWonders[i];

                GameObject cardGo = Instantiate(wonderPrefab);
                cardGo.FindObject("WonderName").GetComponent<TextMesh>().text = wonderCard.CardName;

                cardGo.transform.SetParent(CompletedWondersFrame.transform);
                cardGo.transform.localPosition = new Vector3(0, -0.27f * i);
                
                cardGo.FindObject("Mask").GetComponent<PCBoardCardDisplayBehaviour>().Bind(wonderCard);

                var sp = UnityResources.GetSprite(wonderCard.SpecialImage);
                if (sp != null)
                {
                    cardGo.FindObject("Image").GetComponent<SpriteRenderer>().sprite = sp;
                }
            }
        }

        private void DisplayBuildings(TtaBoard board)
        {
            var buildingPrefab = Resources.Load<GameObject>("Dynamic-PC/BuildingColumn");

            BuildingType[] buildingArray =
            {
                BuildingType.Farm, BuildingType.Mine, BuildingType.Arena, BuildingType.Lab,
                BuildingType.Library, BuildingType.Theater, BuildingType.Temple, BuildingType.AirForce,
                BuildingType.Artillery, BuildingType.Cavalry, BuildingType.Infantry
            };

            foreach (Transform child in BuildingCellFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = 0.7f;
            if (board.Buildings.Count() > 9)
            {
                incr = 0.7f * 8 / (board.Buildings.Count() - 1);
            }

            int i = 0;
            foreach (var t in buildingArray)
            {
                if (!board.Buildings.ContainsKey(t))
                {
                    continue;
                }

                var buildings = board.Buildings[t];
                
                GameObject cellGo = Instantiate(buildingPrefab);
                BuildingCellDisplayBehavior bds = cellGo.GetComponent<BuildingCellDisplayBehavior>();
                bds.Cells = buildings;
                

                cellGo.transform.SetParent(BuildingCellFrame.transform);
                cellGo.transform.localPosition=new Vector3(-3.926f+ incr * i, 0.726f,-1f+0.1f*i);

                bds.Refresh();
                i ++;
            }
        }

        private void DisplayColony(TtaBoard board)
        {
            var colonyPrefab = Resources.Load<GameObject>("Dynamic-PC/Colony");

            foreach (Transform child in ColonyFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.Colonies.Count; i++)
            {
                var colonyCard = board.Colonies[i];

                GameObject cardGo = Instantiate(colonyPrefab);
                var sp = UnityResources.GetSprite(colonyCard.SpecialImage);
                if (sp != null)
                {
                    cardGo.GetComponent<SpriteRenderer>().sprite = sp;
                }

                cardGo.transform.SetParent(ColonyFrame.transform);
                cardGo.transform.localPosition = new Vector3(0, -0.205f * i);

                cardGo.GetComponent<PCBoardCardDisplayBehaviour>().Bind(colonyCard);
            }
        }

        private void DisplaySpecialTech(TtaBoard board)
        {
            SpecialTechFrame.SpecialTechs = board.SpecialTechs;

            SpecialTechFrame.Refresh();
            
        }

        private void DisplayTactics(TtaBoard board)
        {
            TacticFrame.MyTactic = board.Tactic;
            TacticFrame.Refresh();

            TacticOnBoardFrame.GetComponent<TextMesh>().text = board.Tactic == null ? "" : board.Tactic.CardName;
        }

        private void DisplayWarnings(TtaBoard board)
        {
            var warningPrefab = Resources.Load<GameObject>("Dynamic-PC/Warning");

            foreach (Transform child in WarningFrame.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < board.Warnings.Count; i++)
            {
                var warning = board.Warnings[i];
                
                GameObject cardGo = Instantiate(warningPrefab);
                
                cardGo.transform.SetParent(WarningFrame.transform);
                cardGo.transform.localPosition = new Vector3(0.26f*i, 0);
                
                cardGo.GetComponent<WarningDisplayBehaviour>().DisplayWarning(warning,WarningPopupFrame);
            }
        }

        private void DisplayWorkerPool(TtaBoard board)
        {
            var happyPrefab = Resources.Load<GameObject>("Dynamic-PC/HappyWorker");
            var unhappyPrefab = Resources.Load<GameObject>("Dynamic-PC/UnhappyWorker");

            foreach (Transform child in WorkerPoolFrame.transform)
            {
                Destroy(child.gameObject);
            }

            float incr = board.Resource[ResourceType.WorkerPool] > 1
                ? (0.66f/(board.Resource[ResourceType.WorkerPool] - 1))
                : 0f;

            for (int i = 0; i < board.Resource[ResourceType.WorkerPool]; i++)
            {
                GameObject mSp;
                if (i < board.DisorderValue)
                {
                    mSp = Instantiate(unhappyPrefab);
                }
                else
                {
                    mSp = Instantiate(happyPrefab);
                }

                mSp.transform.SetParent(WorkerPoolFrame.transform);
                mSp.transform.localPosition = new Vector3(incr*i, 0);
            }
        }

        public void HideAllPoliticalPhaseDialogs()
        {
            PoliticalPhaseDialogFrame.gameObject.SetActive(false);
        }

        public void DisplayPoliticalPhaseDialogs(TtaGame game)
        {
            HideAllPoliticalPhaseDialogs();
            //根据事件判断当前阶段到底是哪一个阶段
            var actions = game.PossibleActions;
            
            //每一个内容都对应一个独立的dialog
            if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //假如含有PassPoliticalPhase，就是始动阶段
                PoliticalPhaseDialogFrame.gameObject.SetActive(true);
                PoliticalPhaseDialogFrame.transform.position=new Vector3(-3.9f,2.4f,-5f);
                
                PoliticalPhaseDialogFrame.Refresh();
            }
            else if(actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //含有Bid或者SendColonist则是殖民阶段
            }
            else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //含有ResolveAction则是受到事件影响的阶段
            }
            else if (actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase) != null)
            {
                //含有Defend则是抵御入侵的阶段
            }
        }
    }
    


}
