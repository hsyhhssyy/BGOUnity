using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.Translation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Assets.CSharpCode.UI.BoardScene
{
    [UsedImplicitly]
    public class BoardUIBehaviour : MonoBehaviour
    {
        #region UnityEditorGameObjects
        public GameObject LoadingGo;
        public GameObject CardRowGo;
        public GameObject[] PlayerNameGos;

        public GameObject CivilHandCardFrame;
        public GameObject MilitaryHandCardFrame;

        public GameObject ActionsDialogFrame;
        public GameObject CivilCardsDialogFrame;

        public UnityEngine.UI.Dropdown UnknownActionDropDown;
        #endregion

        //private TtaCivilopedia civilopedia;

        [UsedImplicitly]
        void Start()
        {
            ExceptionHandle.SetupExceptionHandling();

            if (SceneTransporter.CurrentGame == null)
            {
                SceneManager.LoadScene("Scene/TestScene");
                return;
            }

            //civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

            LoadingGo.SetActive(true);
            CardRowGo.SetActive(false);
        
        
            StartCoroutine(RefreshBoard());
        
        }

        
    

        private IEnumerator RefreshBoard()
        {
            return SceneTransporter.Server.RefreshBoard(SceneTransporter.CurrentGame, () =>
            {
                CardRowGo.SetActive(true);
                DisplayGameBoard(SceneTransporter.CurrentGame);

                LoadingGo.SetActive(false);
    });
        }

        #region Display Part

        private void DisplayGameBoard(TtaGame game)
        {
            //展示卡牌列

            DisplayCardRow(game);

            //处理未知Action
            DisplayActions(game);

            //显示名称

            for (int i = 0; i < game.Boards.Count; i++)
            {
                //玩家名称
                var txtPlayerName = PlayerNameGos[i].GetComponent<TextMesh>();
                txtPlayerName.text = game.Boards[i].PlayerName;
            }

            //2.展示1号玩家版
            SwitchBoard(0.ToString());

        }

        private void DisplayCardRow(TtaGame game)
        {
            GameObject.Find("CardRow").SetActive(true);
            int cardIndex = 0;
            foreach (var card in game.CardRow)
            {
                var cardGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString());

                String spriteName = "CardRowBackground_" + Enum.GetName(typeof (CardType), card.Card.CardType);
                var rend = cardGo.GetComponent<SpriteRenderer>();
                var sprite = UnityResources.GetSprite(spriteName);
                rend.sprite = sprite;

                var textGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/NameText");
                var textMesh = textGo.GetComponent<TextMesh>();

                textMesh.text = card.Card.CardName;

                var ageGo = GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/AgeText");
                textMesh = ageGo.GetComponent<TextMesh>();

                textMesh.text = Enum.GetName(
                    typeof (Age),
                    Convert.ToInt32(
                        card.Card.InternalId.Split("-".ToCharArray(), 2)[0]));

                var cvPath = "CardRow/CardRow-Card" + cardIndex.ToString() + "/CivilActions";
                GameObject.Find(cvPath+"/1").SetActive(false);
                GameObject.Find(cvPath + "/2").SetActive(false);
                GameObject.Find(cvPath + "/3").SetActive(false);
                GameObject.Find(cvPath + "/4").SetActive(false);
                GameObject.Find(cvPath + "/4More").SetActive(false);

                var putbackGo=GameObject.Find("CardRow/CardRow-Card" + cardIndex.ToString() + "/PutBack");
                putbackGo.SetActive(false);

                if (card.CanTake == true)
                {
                    if (card.CivilActionCost > 0 && card.CivilActionCost <= 4)
                    {
                        GameObject.Find(cvPath + "/" + card.CivilActionCost).SetActive(true);
                    }
                    else if (card.CivilActionCost > 4)
                    {
                        GameObject.Find(cvPath + "/4More").SetActive(true);
                        GameObject.Find(cvPath + "/4More/MarkerCount").GetComponent<TextMesh>().text = "x" +
                                                                                                       card
                                                                                                           .CivilActionCost;
                    }
                }else if (card.CanPutBack)
                {
                    putbackGo.SetActive(true);
                }

                cardIndex++;
            }
        }

        private void DisplayPlayerBoard(TtaGame game, int playerNo)
        {
            TtaBoard board = game.Boards[playerNo];

            var boardName = "PlayerBoard";
            

            //资源面板
            DisplayResourcePanel(boardName, board);

            //红白点
            DisplayActionPoint(boardName+"/ActionPoint",board);
            
            //建筑面板
            foreach (var pair in board.Buildings)
            {
                var cellName = boardName + "/BuildingBoard/"+Enum.GetName(typeof(BuildingType),pair.Key)+"Cell";
                DisplayBuildingCell(cellName, pair.Value);
            }

            //蓝点
            int blueMarkerOwn = board.ResourceQuantity[ResourceType.BlueMarker];
            for (int blueMarkerDisplay = 15;
                blueMarkerOwn >= 0 || blueMarkerDisplay >= 0;
                blueMarkerDisplay--, blueMarkerOwn--)
            {
                if (blueMarkerDisplay >= 0)
                {
                    var bankGo = GameObject.Find(boardName + "/BlueMarkerBank/" + blueMarkerDisplay);
                    bankGo.SetActive(blueMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }

            //黄点
            int yellowMarkerOwn = board.ResourceQuantity[ResourceType.YellowMarker];
            for (int yellowMarkerDisplay = 16;
                yellowMarkerOwn >= 0 || yellowMarkerDisplay >= 0;
                yellowMarkerDisplay--, yellowMarkerOwn--)
            {
                if (yellowMarkerDisplay >= 0)
                {
                    var bankGo = GameObject.Find(boardName + "/YellowMarkerBank/" + yellowMarkerDisplay);
                    bankGo.SetActive(yellowMarkerOwn > 0);
                }
                else
                {
                    //Marker比上限还多
                    //添加几个新的
                }
            }

            //政体
            GameObject.Find(boardName + "/Government/Text").GetComponent<TextMesh>().text=board.Government.CardName;

            //工人
            var workerPoolX = 5.05f;
            var workerPoolFrame = GameObject.Find(boardName + "/WorkerPool");
            foreach (Transform child in workerPoolFrame.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject prefeb = Resources.Load<GameObject>("HappyFace");
            for (int i = 0; i < board.ResourceQuantity[ResourceType.WorkerPool]; i++)
            {
                var mSp = Instantiate(prefeb);
                mSp.transform.SetParent(workerPoolFrame.transform);
                mSp.transform.localPosition = new Vector3(workerPoolX, -2.4f);
                workerPoolX += 0.225f;
            }
            prefeb = Resources.Load<GameObject>("UnhappyFace");
            for (int i = 0; i < board.ResourceFluctuation[ResourceType.WorkerPool]; i++)
            {
                var mSp = Instantiate(prefeb);
                mSp.transform.SetParent(workerPoolFrame.transform);
                mSp.transform.localPosition = new Vector3(workerPoolX, -2.4f);
                workerPoolX += 0.225f;
            }
            Debug.Log("Total Worker:"+ board.ResourceQuantity[ResourceType.WorkerPool]+" "+ board.ResourceFluctuation[ResourceType.WorkerPool]);

            //奇迹
            DisplayWonders(board);

            //手牌
            DisplayHandCard(board.CivilCards, CivilHandCardFrame);
            DisplayHandCard(board.MilitaryCards, MilitaryHandCardFrame);
        }

        private static void DisplayResourcePanel(String boardName,TtaBoard boardData)
        {
            //设置资源面板
            var txtResourceCulture =
                GameObject.Find(boardName+"/ResourceDisplay/ResourceCulture").GetComponent<TextMesh>();
            txtResourceCulture.text = boardData.ResourceQuantity[ResourceType.Culture].ToString() +
                                      " | <color=#ffa500ff>" +
                                      (boardData.ResourceFluctuation[ResourceType.Culture] >= 0 ? "+" : "-") +
                                      Math.Abs(boardData.ResourceFluctuation[ResourceType.Culture]).ToString() + "</color>";

            var txtResourceScience =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceScience").GetComponent<TextMesh>();
            txtResourceScience.text = boardData.ResourceQuantity[ResourceType.Science].ToString() +
                                      (boardData.ResourceQuantity[ResourceType.ScienceForMilitary] > 0
                                          ? "+<color=#ff0000ff>" + boardData.ResourceQuantity[ResourceType.ScienceForMilitary] +
                                            "</color>"
                                          : "")
                                      +
                                      " | <color=#ffa500ff>" +
                                      (boardData.ResourceFluctuation[ResourceType.Science] >= 0 ? "+" : "-") +
                                      Math.Abs(boardData.ResourceFluctuation[ResourceType.Science]).ToString() + "</color>";

            var txtResourceMilitaryForce =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceMilitaryForce").GetComponent<TextMesh>();
            txtResourceMilitaryForce.text = boardData.ResourceQuantity[ResourceType.MilitaryForce].ToString();

            var txtResourceExploration =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceExploration").GetComponent<TextMesh>();
            txtResourceExploration.text = boardData.ResourceQuantity[ResourceType.Exploration] <= 0
                ? " - "
                : ("+" + boardData.ResourceQuantity[ResourceType.Exploration].ToString());

            var txtResourceFood =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceFood").GetComponent<TextMesh>();
            txtResourceFood.text = boardData.ResourceQuantity[ResourceType.Food].ToString() +
                                   " | <color=#ffa500ff>" +
                                   (boardData.ResourceFluctuation[ResourceType.Food] >= 0 ? "+" : "-") +
                                   Math.Abs(boardData.ResourceFluctuation[ResourceType.Food]).ToString() + "</color>";

            var txtResourceOre =
                GameObject.Find(boardName + "/ResourceDisplay/ResourceOre").GetComponent<TextMesh>();
            txtResourceOre.text = boardData.ResourceQuantity[ResourceType.Ore].ToString() +
                                  (boardData.ResourceQuantity[ResourceType.OreForMilitary] > 0
                                      ? "+<color=#ff0000ff>" + boardData.ResourceQuantity[ResourceType.OreForMilitary] +
                                        "</color>"
                                      : "")
                                  +
                                  " | <color=#ffa500ff>" +
                                  (boardData.ResourceFluctuation[ResourceType.Ore] >= 0 ? "+" : "-") +
                                  Math.Abs(boardData.ResourceFluctuation[ResourceType.Ore]).ToString() + "</color>";
        }

        private void DisplayActionPoint(String frameName, TtaBoard board)
        {
            //framename是指ActionPoint的frame名而且不含斜杠/
            var missingPrefab = Resources.Load<GameObject>("MissingMarker");
            var whitePrefab = Resources.Load<GameObject>("WhiteMarker");
            var redPrefab = Resources.Load<GameObject>("RedMarker");
            
            DisplayActionPoint(GameObject.Find(frameName + "/CivilAction"), board.ResourceFluctuation[ResourceType.WhiteMarker],
                board.ResourceQuantity[ResourceType.WhiteMarker], whitePrefab, missingPrefab);
            
            DisplayActionPoint(GameObject.Find(frameName + "/MilitaryAction"), board.ResourceFluctuation[ResourceType.RedMarker],
                board.ResourceQuantity[ResourceType.RedMarker], redPrefab, missingPrefab);

        }

        private static void DisplayActionPoint(GameObject civilActionFrame, int markerFluctuation, int markerQuantity,
            GameObject whitePrefab, GameObject missingPrefab)
        {
            foreach (Transform child in civilActionFrame.transform)
            {
                Destroy(child.gameObject);
            }
            //确定第一行

            var markerTotal = markerFluctuation + markerQuantity;

            if (markerTotal > 6)
            {
                //2行的第一行
                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb = null;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(0.02f + i*0.15f, -0.075f);
                }

                //第二行
                var initate = 0.02f + (6 - markerQuantity - markerFluctuation)*0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else if (markerFluctuation > 0)
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate +i*0.15f, -0.225f);
                }
            }
            else
            {
                var initate = 0.02f + (6 - markerQuantity - markerFluctuation)*0.075f;

                for (int i = 0; i < 6; i++)
                {
                    GameObject prefeb;
                    if (markerQuantity > 0)
                    {
                        markerQuantity--;
                        prefeb = whitePrefab;
                    }
                    else if (markerFluctuation > 0)
                    {
                        markerFluctuation--;
                        prefeb = missingPrefab;
                    }
                    else
                    {
                        break;
                    }
                    var mSp = Instantiate(prefeb);
                    mSp.transform.SetParent(civilActionFrame.transform);
                    mSp.transform.localPosition = new Vector3(initate + i*0.15f, -0.15f);
                }
            }
        }

        private void DisplayBuildingCell(String cellName,Dictionary<Age,BuildingCell> cellData)
        {
            var cellGo = GameObject.Find(cellName);
            cellGo.SetActive(true);

            GameObject.Find(cellName + "/BuildingMarker/Enable").SetActive(true);
            GameObject.Find(cellName + "/BuildingMarker/Disable").SetActive(true);

            for (int ageNum = 0; ageNum <= 3; ageNum++)
            {
                var ageEnumValue = (Age) ageNum;

                var disableMarker = GameObject.Find(cellName + "/BuildingMarker/Disable/" + ageNum);
                var enableMarker = GameObject.Find(cellName + "/BuildingMarker/Enable/" + ageNum);
                var buildingCounter = GameObject.Find(cellName + "/BuildingCounter/" + ageNum);
                var resourceMarker = GameObject.Find(cellName + "/ResourceMarker/" + ageNum);
                var resourceCounter = GameObject.Find(cellName + "/ResourceCounter/" + ageNum);

                bool techDisabled = !cellData.ContainsKey(ageEnumValue);
                disableMarker.SetActive(techDisabled);
                enableMarker.SetActive(!techDisabled);
                resourceMarker.SetActive(false);
                resourceCounter.SetActive(false);
                buildingCounter.SetActive(false);

                if (techDisabled)
                {
                    continue;
                }

                var cell = cellData[ageEnumValue];

                if (cell.Worker > 0)
                {
                    buildingCounter.SetActive(true);
                    buildingCounter.GetComponent<TextMesh>().text = "x"+cell.Worker.ToString();
                }

                if (cell.Storage > 0)
                {
                    resourceMarker.SetActive(true);
                    resourceCounter.SetActive(true);
                    resourceCounter.GetComponent<TextMesh>().text = "x"+cell.Storage.ToString();
                }
                
            }
        }

        private void DisplayWonders(TtaBoard board)
        {
            var y = -0.39f;
            var wonderGo = GameObject.Find("PlayerBoard/Wonders/Constructing");
            if (board.ConstructingWonder != null)
            {
                y = -0.95f;
                wonderGo.SetActive(true);
                GameObject.Find("PlayerBoard/Wonders/Constructing/Name").GetComponent<TextMesh>().text =
                    board.ConstructingWonder.CardName.WordWrap(6);
                String buildStr = " ";
                foreach (var str in board.ConstructingWonderSteps)
                {
                    if (str == "X")
                    {
                        buildStr += "<quad material=1 size=15 x=0 y=0 width=1 height=1 />";
                    }
                    else
                    {
                        buildStr += " "+str ;
                    }
                }
                GameObject.Find("PlayerBoard/Wonders/Constructing/Build").GetComponent<TextMesh>().text =
                    buildStr;

            }
            else
            {
                wonderGo.SetActive(false);
            }
            var frameGo = GameObject.Find("PlayerBoard/Wonders/Completed");
            foreach (Transform child in frameGo.transform)
            {
                Destroy(child.gameObject);
            }
            var wonderPrefab = Resources.Load<GameObject>("CompletedWonder");
            foreach (var wonder in board.CompletedWonders)
            {
                var cWonderGo=Instantiate(wonderPrefab);

                var rend=cWonderGo.FindObject("Wonder").GetComponent<SpriteRenderer>();
                var sp = Resources.Load<Sprite>(wonder.SmallImage);
                if (sp != null)
                {
                    rend.sprite = sp;
                }
                else
                {
                    Debug.Log("No sprite for:"+ wonder.SmallImage);
                }

                cWonderGo.transform.parent = frameGo.transform;

                cWonderGo.transform.localPosition = new Vector3(3.485f, y);
                y = y - 0.55f;
            }
        }

        private void DisplayHandCard(List<CardInfo> cards,GameObject frame)
        {
            var handCardPrefeb = Resources.Load<GameObject>("HandCard");

            foreach (Transform child in frame.transform)
            {
                Destroy(child.gameObject);
            }

            var y = 0f;

            foreach (var card in cards)
            {
                //确定tile
                String spriteName = "HandCard_";
                switch (card.CardType)
                {
                    case CardType.Action:
                        spriteName += "Action";
                        break;
                        case CardType.MilitaryTechArtillery:
                    case CardType.MilitaryTechAirForce:
                        case CardType.MilitaryTechCavalry:
                        case CardType.MilitaryTechInfantry:
                        spriteName += "Military";
                        break;
                    case CardType.UrbanTechArena:
                        case CardType.UrbanTechLab:
                        case CardType.UrbanTechLibrary:
                        case CardType.UrbanTechTemple:
                        case CardType.UrbanTechTheater:
                        spriteName += "Urban";
                        break;
                    case CardType.SpecialTechCivil:
                        case CardType.SpecialTechEngineering:
                        case CardType.SpecialTechExploration:
                        case CardType.SpecialTechMilitary:
                        spriteName += "SpecialTech";
                        break;
                   case CardType.Government:
                    case CardType.Leader:
                        spriteName += Enum.GetName(typeof(CardType),card.CardType);
                        break;
                    case CardType.ResourceTechFarm:
                        case CardType.ResourceTechMine:
                        spriteName += "Production";
                        break;
                    default:
                        spriteName += "Leader";
                        break;
                }


                var mSp = Instantiate(handCardPrefeb);
                mSp.transform.SetParent(frame.transform);
                mSp.transform.localPosition = new Vector3(0,y);

                var sp = UnityResources.GetSprite(spriteName);
                var rend = mSp.GetComponent<SpriteRenderer>();
                rend.sprite = sp;

                var tMesh=mSp.FindObject("Text").GetComponent<TextMesh>();

                tMesh.text = Enum.GetName(typeof(Age), card.CardAge)+" - " + card.CardName;

                y = y - 0.25f;
            }


        }

        public void DisplayActions(TtaGame game)
        {
            UnknownActionDropDown.options=new List<Dropdown.OptionData>();
            foreach (var pa in game.PossibleActions.FindAll(t=>t.ActionType== PlayerActionType.Unknown))
            {
                UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = pa.Data[0].ToString();

                UnknownActionDropDown.options.Add(op);                
            }
        }

        #endregion

        

        #region Control Part

        public void SwitchBoard(String no)
        {
            int playerNo = Convert.ToInt32(no);
            if (SceneTransporter.CurrentGame.Boards.Count <= playerNo)
            {
                return;
            }

            DisplayPlayerBoard(SceneTransporter.CurrentGame,playerNo);
        }

        public void TakeCard(int Position)
        {
            //先进行一些检查
            var item = SceneTransporter.CurrentGame.CardRow[Position];
            if ((!item.CanTake)&&(!item.CanPutBack))
            {
                Debug.Log("Error:Can't take/put this card");
                return;
            }
            //然后提交给服务器
            var actionFound =
                SceneTransporter.CurrentGame.PossibleActions.Find(
                    t =>
                        (t.ActionType == PlayerActionType.TakeCardFromCardRow ||
                         t.ActionType == PlayerActionType.PutBackCard) && t.Data[1].ToString() == Position.ToString());

            LoadingGo.SetActive(true);
            StartCoroutine(SceneTransporter.Server.TakeAction(SceneTransporter.CurrentGame, actionFound,
                BackgroundRefresh));
        }

        public void PopupDialog(String dialog)
        {
            //Debug.Log("Open Dialog - "+dialog);
            GameObject dialogFrame = null;
            switch (dialog)
            {
                case "Actions":
                    dialogFrame = ActionsDialogFrame;
                    break;
                case "CivilCards":
                    dialogFrame = CivilCardsDialogFrame;
                    break;
            }
            if (dialogFrame == null)
            {
                Debug.Log("Can't Find Dialog - "+dialog);
                return;
            }

            //dialogFrame.transform.localPosition=new Vector3(0,0,0);
            dialogFrame.GetComponent<Animator>().SetTrigger("Expand");


        }

        #endregion

        [UsedImplicitly]
        public void BackButton_Clicked()
        {
            SceneManager.LoadScene("Scene/LobbyScene");
        }
        [UsedImplicitly]
        public void GoButton_Clicked()
        {
            String str = UnknownActionDropDown.options[UnknownActionDropDown.value].text;
            var actionFound =
                SceneTransporter.CurrentGame.PossibleActions.Find(
                    t =>
                        (t.ActionType == PlayerActionType.Unknown) && t.Data[0].ToString() == str);

            if (actionFound != null)
            {
                Debug.Log("Go!Action:"+ str);
                LoadingGo.SetActive(true);
                StartCoroutine(SceneTransporter.Server.TakeAction(SceneTransporter.CurrentGame, actionFound,
                    BackgroundRefresh));
            }
        }

        private void BackgroundRefresh()
        {
            Debug.Log("Background Refresh");
            //StartCoroutine(RefreshBoard());
            DisplayGameBoard(SceneTransporter.CurrentGame);
            LoadingGo.SetActive(false);
        }
    }
}
