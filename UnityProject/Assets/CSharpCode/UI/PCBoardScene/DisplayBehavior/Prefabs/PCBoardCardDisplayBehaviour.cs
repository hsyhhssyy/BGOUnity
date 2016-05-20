using System;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.UI.Util;

namespace Assets.CSharpCode.UI.PCBoardScene
{
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public class PCBoardCardDisplayBehaviour : MonoBehaviour
    {
        private bool _refreshRequired = true;

        private GameObject _normalImagePopup;
        public CardInfo Card { get; private set; }

        [UsedImplicitly]
        public bool SmallCard = false;

        public bool DisablePopup = true;

        public GameObject ActionCardFrame;
        public GameObject BuildingCardFrame;
        public GameObject LeaderCardFrame;
        public GameObject WonderCardFrame;
        public GameObject GovernmentCardFrame;
        public GameObject TacticCardFrame;
        public GameObject EventColonyPactCardFrame;
        public GameObject WarAggressionCardFrame;
        public GameObject DefendCardFrame;

        #region Popup部分的代码

        /// <summary>
        /// 这是用于Popup预览图的，预览图是一个根据大图Prefab现场生成
        /// </summary>
        [UsedImplicitly]
        public void OnMouseEnter()
        {
            if (DisablePopup != true && Card != null)
            {
                _normalImagePopup = Instantiate(Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Normal"));

                var beh = _normalImagePopup.GetComponent<PCBoardCardDisplayBehaviour>();
                beh.DisablePopup = true;
                beh.Bind(Card);

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

                _normalImagePopup.transform.position = pos;

            }
        }

        /// <summary>
        /// 这是用于Popup预览图的，预览图是一个根据大图Prefab现场生成
        /// </summary>
        [UsedImplicitly]
        public void OnMouseExit()
        {
            if (_normalImagePopup != null)
            {
                Destroy(_normalImagePopup);
            }
        }

        [UsedImplicitly]
        public void OnMouseUpAsButton()
        {
            if (_normalImagePopup != null)
            {
                Destroy(_normalImagePopup);
            }
        }

        #endregion

        #region 展示部分的代码-公用

        /// <summary>
        /// 每当卡牌变更的时候，重新激活一次刷新
        /// </summary>
        [UsedImplicitly]
        public void Update()
        {
            if (_refreshRequired)
            {

                _refreshRequired = false;
                if (ActionCardFrame != null)
                {
                    Refresh();
                }
            }
        }


        public void Bind(CardInfo card, Transform parentTransform, Vector3 newLocation)
        {
            Card = card;
            gameObject.transform.SetParent(parentTransform);
            gameObject.transform.localPosition = newLocation;
            _refreshRequired = true;
        }

        public void Bind(CardInfo card)
        {
            Card = card;
            _refreshRequired = true;
        }


        private void Refresh()
        {
            if (Card == null)
            {
                return;
            }

            ActionCardFrame.SetActive(false);
            BuildingCardFrame.SetActive(false);
            LeaderCardFrame.SetActive(false);
            WonderCardFrame.SetActive(false);
            GovernmentCardFrame.SetActive(false);
            TacticCardFrame.SetActive(false);
            EventColonyPactCardFrame.SetActive(false);
            WarAggressionCardFrame.SetActive(false);
            DefendCardFrame.SetActive(false);


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
                case CardType.ResourceTechFarm:
                case CardType.ResourceTechMine:
                case CardType.MilitaryTechAirForce:
                case CardType.MilitaryTechArtillery:
                case CardType.MilitaryTechCavalry:
                case CardType.MilitaryTechInfantry:
                case CardType.SpecialTechCivil:
                case CardType.SpecialTechEngineering:
                case CardType.SpecialTechExploration:
                case CardType.SpecialTechMilitary:
                    DisplayBuilding();
                    break;
                case CardType.Leader:
                    DisplayLeader();
                    break;
                case CardType.Wonder:
                    DisplayWonder();
                    break;
                case CardType.Government:
                    DisplayGovernment();
                    break;
                case CardType.Tactic:
                    DisplayTactic();
                    break;
                case CardType.Colony:
                    DisplayColony();
                    break;
                case CardType.Pact:
                case CardType.Event:
                    DisplayEventAndPact();
                    break;
                case CardType.War:
                case CardType.Aggression:
                    DisplayAggressionAndWar();
                    break;
                    case CardType.Defend:
                        DisplayDefend();
                    break;
                default:
                   Assets.CSharpCode.UI.Util.LogRecorder.Log("Unknown type to display:" + Card.CardType.ToString()+" "+Card.CardName);
                    DisplayAction();
                    break;
            }
        }

        public T ChooseParam<T>(T small, T normal)
        {
            if (SmallCard)
            {
                return small;
            }

            return normal;
        }

        #endregion

        #region 展示部分的代码-具体对应到每张卡

        private void DisplaySustainedEffects(GameObject frame, List<CardEffect> effects, float widthLimit)
        {
            foreach (Transform child in frame.transform)
            {
                Destroy(child.gameObject);
            }

            // Unity Can't handle this type of codes
            // ReSharper disable once ConvertClosureToMethodGroup
            var effectGo = effects.Select(effect => CreateFrameItem(effect)).Where(go => go != null).ToList();

            if (effectGo.Count == 1)
            {
                GameObject go = effectGo[0];
                go.transform.parent = frame.transform;
                go.FindObject("Image").transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                go.transform.localPosition = new Vector3(widthLimit / 2, 0f, 0f);
            }
            else if (effectGo.Count > 1)
            {
                float incr = widthLimit / (effectGo.Count - 1);
                for (int i = 0; i < effectGo.Count; i++)
                {
                    GameObject go = effectGo[i];
                    go.transform.parent = frame.transform;
                    go.transform.localPosition = new Vector3(0f + incr * i, 0f, 0f);
                }
            }

        }

        private GameObject CreateFrameItem(CardEffect effect)
        {
            switch (effect.FunctionId)
            {
                case CardEffectType.ResourceOfTypeXChangedY:

                    String iconImg;
                    switch ((ResourceType)effect.Data[0])
                    {
                        case ResourceType.Resource:
                        case ResourceType.ResourceIncrement:
                            iconImg = "SpriteTile/UI/icon-resource";
                            break;
                        case ResourceType.Food:
                        case ResourceType.FoodIncrement:
                            iconImg = "SpriteTile/UI/icon-food";
                            break;
                        case ResourceType.Culture:
                        case ResourceType.CultureIncrement:
                            iconImg = "SpriteTile/UI/icon-culture";
                            break;
                            case ResourceType.Science:
                        case ResourceType.ScienceIncrement:
                            iconImg = "SpriteTile/UI/icon-science";
                            break;
                        case ResourceType.MilitaryForce:
                            iconImg = "SpriteTile/UI/icon-military";
                            break;
                        case ResourceType.HappyFace:
                            iconImg = effect.Data[0] >= 0 ? "SpriteTile/UI/icon-happy" : "SpriteTile/UI/icon-unhappy";
                            break;
                        case ResourceType.WhiteMarkerMax:
                            iconImg = "SpriteTile/UI/icon-white";
                            break;
                        case ResourceType.RedMarkerMax:
                            iconImg = "SpriteTile/UI/icon-red";
                            break;
                        case ResourceType.YellowMarker:
                            iconImg = "SpriteTile/UI/icon-yellow";
                            break;
                        case ResourceType.BlueMarker:
                            iconImg = "SpriteTile/UI/icon-blue";
                            break;
                        default:
                            return null;
                    }

                    switch ((ResourceType)effect.Data[0])
                    {
                        case ResourceType.Food:
                        case ResourceType.FoodIncrement:
case ResourceType.Resource:
                        case ResourceType.ResourceIncrement:
                        case ResourceType.Science:
                        case ResourceType.ScienceIncrement:
                        case ResourceType.Culture:
                        case ResourceType.CultureIncrement:
                        case ResourceType.MilitaryForce:
                        case ResourceType.Exploration:
                            {
                                GameObject go = new GameObject("ProductionNumbered");

                                var icon = new GameObject("Image", typeof(SpriteRenderer));
                                icon.transform.parent = go.transform;


                                icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite(iconImg);

                                icon.transform.localScale = new Vector3(ChooseParam(0.3f, 0.8f), ChooseParam(0.3f, 0.8f),
                                    1);
                                icon.transform.localPosition = new Vector3(0f, 0f, 0f);

                                var text = new GameObject("Text", typeof(TextMesh));
                                text.AddComponent<TextOutline>();
                                text.GetComponent<TextMesh>().color = Color.white;
                                text.GetComponent<TextMesh>().text = effect.Data[1].ToString();
                                text.GetComponent<TextMesh>().fontSize = ChooseParam(14, 30);

                                text.transform.parent = go.transform;
                                text.transform.localScale = new Vector3(0.1f, 0.1f, 1);
                                text.transform.localPosition = new Vector3(ChooseParam(0f, 0.024f),
                                    ChooseParam(0.059f, 0.112f), -0.01f);

                                return go;
                            }

                        case ResourceType.HappyFace:
                        case ResourceType.WhiteMarkerMax:
                        case ResourceType.RedMarkerMax:
                        case ResourceType.YellowMarker:
                        case ResourceType.BlueMarker:
                            {
                                GameObject go = new GameObject("ProductionStacked");
                                for (int i = 0; i < Math.Abs(effect.Data[1]); i++)
                                {
                                    var icon = new GameObject("Image", typeof(SpriteRenderer));
                                    icon.transform.parent = go.transform;

                                    icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite(iconImg);

                                    icon.transform.localScale = new Vector3(ChooseParam(0.3f, 0.8f),
                                        ChooseParam(0.3f, 0.8f), 1);
                                    icon.transform.localPosition = new Vector3(0f + ChooseParam(0.02f, 0.1f) * i, 0f, 0f);

                                }
                                if (effect.Data[1] < 0 && (effect.Data[0] != (int)ResourceType.HappyFace))
                                {
                                    var text = new GameObject("Text", typeof(TextMesh));
                                    text.AddComponent<TextOutline>();
                                    text.GetComponent<TextMesh>().color = Color.white;
                                    text.GetComponent<TextMesh>().text = "-";
                                    text.GetComponent<TextMesh>().fontSize = ChooseParam(14, 30);

                                    text.transform.parent = go.transform;
                                    text.transform.localScale = new Vector3(0.1f, 0.1f, 1);
                                    text.transform.localPosition = new Vector3(ChooseParam(-0.15f, -0.24f),
                                        ChooseParam(0.059f, 0.2f), -0.01f);
                                }
                                return go;
                            }
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        private void DisplayAction()
        {
            //Get action background
            var sp = UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-action-background");

            ActionCardFrame.SetActive(true);
            ActionCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            ActionCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            ActionCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 6));
            if (!SmallCard)
            {
                ActionCardFrame.FindObject("Description").GetComponent<TextMesh>().text = Card.Description.WordWrap(8);
            }


        }

        private void DisplayLeader()
        {
            //Get action background
            var sp = UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-leader-background");
            LeaderCardFrame.SetActive(true);
            LeaderCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            LeaderCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            LeaderCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 6));
            if (!SmallCard)
            {
                LeaderCardFrame.FindObject("Description").GetComponent<TextMesh>().text = Card.Description.WordWrap(10);
            }

            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                LeaderCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }

        }


        private void DisplayBuilding()
        {
            //Get action background
            String typeStr = "urban";
            switch (Card.CardType)
            {

                case CardType.ResourceTechFarm:
                case CardType.ResourceTechMine:
                    typeStr = "production";
                    break;
                case CardType.MilitaryTechAirForce:
                case CardType.MilitaryTechArtillery:
                case CardType.MilitaryTechCavalry:
                case CardType.MilitaryTechInfantry:
                    typeStr = "military";
                    break;

                case CardType.SpecialTechCivil:
                case CardType.SpecialTechEngineering:
                case CardType.SpecialTechExploration:
                case CardType.SpecialTechMilitary:
                    typeStr = "special";
                    break;
            }
            var sp = UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-" + typeStr + "-background");
            BuildingCardFrame.SetActive(true);
            BuildingCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            BuildingCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            BuildingCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 5));

            if (!SmallCard)
            {
                if (Card.ResearchCost.Count > 0 && Card.ResearchCost[0] > 0)
                {
                    BuildingCardFrame.FindObject("TechCost").GetComponent<TextMesh>().text =
                        Card.ResearchCost[0].ToString();
                }
                else
                {
                    BuildingCardFrame.FindObject("TechCost").SetActive(false);
                    BuildingCardFrame.FindObject("TechCostBackground").SetActive(false);
                }
                if (Card.BuildCost.Count > 0 && Card.BuildCost[0] > 0)
                {
                    BuildingCardFrame.FindObject("ResourceCost").GetComponent<TextMesh>().text =
                        Card.BuildCost[0].ToString();
                    BuildingCardFrame.FindObject("ResourceCost").SetActive(true);
                    BuildingCardFrame.FindObject("ResourceCostBackground").SetActive(true);
                }
                else
                {
                    BuildingCardFrame.FindObject("ResourceCost").SetActive(false);
                    BuildingCardFrame.FindObject("ResourceCostBackground").SetActive(false);
                }

                if (Card.Description != null)
                {
                    BuildingCardFrame.FindObject("Description").SetActive(true);
                    BuildingCardFrame.FindObject("Description").GetComponent<TextMesh>().text =
                        Card.Description.WordWrap(10);
                }
                else
                {
                    BuildingCardFrame.FindObject("Description").SetActive(false);
                }

                var imageSp = UnityResources.GetSprite(Card.NormalImage);
                if (imageSp != null)
                {
                    BuildingCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
                    BuildingCardFrame.FindObject("NormalImage").transform.localScale =
                        new Vector3(ChooseParam(0.333f, 1f), ChooseParam(0.333f, 1f), 1f);
                }

                DisplaySustainedEffects(BuildingCardFrame.FindObject("ProductionFrame"), Card.SustainedEffects,
                    ChooseParam(0.44f, 1.40f));
            }
        }

        private void DisplayWonder()
        {
            var sp = UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-wonder-background");
            WonderCardFrame.SetActive(true);
            WonderCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            WonderCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            WonderCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 7));
            if (!SmallCard)
            {
                WonderCardFrame.FindObject("Description").GetComponent<TextMesh>().text = Card.Description.WordWrap(10);
                WonderCardFrame.FindObject("Step").GetComponent<TextMesh>().text = Card.BuildCost.Aggregate("", (current, s) => current + (s + "  ")).Trim();
            }

            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                WonderCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }

            DisplaySustainedEffects(WonderCardFrame.FindObject("ProductionFrame"), Card.SustainedEffects, ChooseParam(0.44f, 1.40f));
        }

        private void DisplayGovernment()
        {
            var sp =
                UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-government-background");
            GovernmentCardFrame.SetActive(true);
            GovernmentCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            GovernmentCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            GovernmentCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 5));

            if (Card.ResearchCost.Count > 0 && Card.ResearchCost[0] > 0)
            {
                GovernmentCardFrame.FindObject("TechCost").GetComponent<TextMesh>().text = Card.ResearchCost[0].ToString();
                GovernmentCardFrame.FindObject("RevoCost").GetComponent<TextMesh>().text = Card.ResearchCost[1].ToString();

            }
            else
            {
                GovernmentCardFrame.FindObject("TechCost").SetActive(false);
                GovernmentCardFrame.FindObject("TechCostBackground").SetActive(false);
                GovernmentCardFrame.FindObject("RevoCost").SetActive(false);
                GovernmentCardFrame.FindObject("RevoCostBackground").SetActive(false);
            }
            
            int whiteMarker = 0;
            int redMarker = 0;
            int urbanLimit = 0;
            List<CardEffect> effects = new List<CardEffect>();
            foreach (var effect in Card.SustainedEffects)
            {
                if (effect.FunctionId == CardEffectType.ResourceOfTypeXChangedY)
                {
                    switch ((ResourceType)effect.Data[0])
                    {
                        case ResourceType.WhiteMarkerMax:
                            whiteMarker = effect.Data[1];
                            continue;
                        case ResourceType.RedMarkerMax:
                            redMarker = effect.Data[1];
                            continue;
                        case ResourceType.UrbanLimit:
                            urbanLimit = effect.Data[1];
                            continue;
                    }
                }

                effects.Add(effect);

            }

            if (!SmallCard)
            {

                var frame = GovernmentCardFrame.FindObject("WhiteMarkerFrame");
                foreach (Transform child in frame.transform)
                {
                    Destroy(child.gameObject);
                }
                for (int i = 0; i < whiteMarker; i++)
                {
                    var icon = new GameObject("Image", typeof(SpriteRenderer));
                    icon.transform.parent = frame.transform;

                    icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("SpriteTile/UI/icon-white");

                    icon.transform.localScale = new Vector3(ChooseParam(0.3f, 0.8f), ChooseParam(0.3f, 0.8f), 1);
                    icon.transform.localPosition = new Vector3(0f + ChooseParam(0.02f, 0.1f) * i, 0f, -0.01f);

                }

                frame = GovernmentCardFrame.FindObject("RedMarkerFrame");
                foreach (Transform child in frame.transform)
                {
                    Destroy(child.gameObject);
                }
                for (int i = 0; i < redMarker; i++)
                {
                    var icon = new GameObject("Image", typeof(SpriteRenderer));
                    icon.transform.parent = frame.transform;

                    icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("SpriteTile/UI/icon-red");

                    icon.transform.localScale = new Vector3(ChooseParam(0.3f, 0.8f), ChooseParam(0.3f, 0.8f), 1);
                    icon.transform.localPosition = new Vector3(0f + ChooseParam(0.02f, 0.1f) * i, 0f, -0.01f);

                }
            }
            GovernmentCardFrame.FindObject("WhiteCount").GetComponent<TextMesh>().text = whiteMarker.ToString();
            GovernmentCardFrame.FindObject("RedCount").GetComponent<TextMesh>().text = redMarker.ToString();
            GovernmentCardFrame.FindObject("UrbanLimit").GetComponent<TextMesh>().text = urbanLimit.ToString();

            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                GovernmentCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }

            DisplaySustainedEffects(GovernmentCardFrame.FindObject("ProductionFrame"), effects,
                ChooseParam(0.44f, 1f));
        }

        public void DisplayTactic()
        {
            var sp =
                UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-tactic-background");
            TacticCardFrame.SetActive(true);
            TacticCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            TacticCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            TacticCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 5));

            if (!SmallCard)
            {
                var frame = TacticCardFrame.FindObject("TacticContent");
                foreach (Transform child in frame.transform)
                {
                    Destroy(child.gameObject);
                }

                float start = Card.TacticComposition.Count * -0.18f + 0.18f;

                for (int i = 0; i < Card.TacticComposition.Count; i++)
                {
                    var type = new[] { "infantry", "cavalry", "artillery" }[10-Card.TacticComposition[i]];


                    var icon = new GameObject("Image", typeof(SpriteRenderer));
                    icon.transform.parent = frame.transform;

                    icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("pc-board-card-tactic-icon-" + type);

                    icon.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    icon.transform.localPosition = new Vector3(start + 0.36f * i, 0f, -0.01f);

                }

                TacticCardFrame.FindObject("TacticValue").GetComponent<TextMesh>().text = Card.TacticValue[0].ToString();
                if (Card.TacticValue.Count > 1)
                {
                    TacticCardFrame.FindObject("OutdatedTacticValue").SetActive(true);
                    TacticCardFrame.FindObject("OutdatedTacticValueBackground").SetActive(true);
                    TacticCardFrame.FindObject("OutdatedTacticValue").GetComponent<TextMesh>().text = Card.TacticValue[1].ToString();
                }
                else
                {
                    TacticCardFrame.FindObject("OutdatedTacticValue").SetActive(false);
                    TacticCardFrame.FindObject("OutdatedTacticValueBackground").SetActive(false);
                }

                var imageSp = UnityResources.GetSprite(Card.NormalImage);
                if (imageSp != null)
                {
                    TacticCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
                }
            }
        }


        private void DisplayEventAndPact()
        {
            var sp =
               UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-" +
               (Card.CardType == CardType.Event ? "event" : "pact") + "-background");
            EventColonyPactCardFrame.SetActive(true);
            EventColonyPactCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            EventColonyPactCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            EventColonyPactCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 7));

            if (!SmallCard)
            {
                EventColonyPactCardFrame.FindObject("Description").SetActive(true);
                EventColonyPactCardFrame.FindObject("Description").GetComponent<TextMesh>().text = Card.Description.WordWrap(10);

                EventColonyPactCardFrame.FindObject("InstantEffects").SetActive(false);

                EventColonyPactCardFrame.FindObject("ProductionFrame").SetActive(false);

            }

            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                EventColonyPactCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }

        }

        private void DisplayAggressionAndWar()
        {
            var sp =
              UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-" +
              (Card.CardType == CardType.War ? "war" : "aggression") + "-background");
            WarAggressionCardFrame.SetActive(true);
            WarAggressionCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            WarAggressionCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            WarAggressionCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 7));

            if (!SmallCard)
            {
                WarAggressionCardFrame.FindObject("Description").GetComponent<TextMesh>().text =
                    Card.Description.WordWrap(10);

                var frame = WarAggressionCardFrame.FindObject("RedDots");
                foreach (Transform child in frame.transform)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < Card.RedMarkerCost[0]; i++)
                {
                    var icon = new GameObject("Image", typeof (SpriteRenderer));
                    icon.transform.parent = frame.transform;

                    icon.GetComponent<SpriteRenderer>().sprite = UnityResources.GetSprite("SpriteTile/UI/icon-red");

                    icon.transform.localScale = new Vector3(1f, 1f, 1f);
                    icon.transform.localPosition = new Vector3(0 + 0.15f*i, 0f, -0.01f);

                }
            }


            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                WarAggressionCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }
        }

        private void DisplayColony()
        {
            var sp =
               UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-event-background");
            EventColonyPactCardFrame.SetActive(true);
            EventColonyPactCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            EventColonyPactCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            EventColonyPactCardFrame.FindObject("NameText").GetComponent<TextMesh>().text = Card.CardName.WordWrap(ChooseParam(4, 7));

            if (!SmallCard)
            {
                EventColonyPactCardFrame.FindObject("Description").SetActive(false);

                EventColonyPactCardFrame.FindObject("InstantEffects").SetActive(true);
                DisplaySustainedEffects(EventColonyPactCardFrame.FindObject("InstantEffects"), Card.ImmediateEffects, 2f);

                EventColonyPactCardFrame.FindObject("ProductionFrame").SetActive(true);
                DisplaySustainedEffects(EventColonyPactCardFrame.FindObject("ProductionFrame"), Card.SustainedEffects,1.3f);

            }

            var imageSp = UnityResources.GetSprite(Card.NormalImage);
            if (imageSp != null)
            {
                EventColonyPactCardFrame.FindObject("NormalImage").GetComponent<SpriteRenderer>().sprite = imageSp;
            }   
        }

        private void DisplayDefend()
        {
            var sp =
               UnityResources.GetSprite("pc-board-card-" + ChooseParam("small", "normal") + "-defend-background");
            DefendCardFrame.SetActive(true);
            DefendCardFrame.GetComponent<SpriteRenderer>().sprite = sp;

            DefendCardFrame.FindObject("AgeText").GetComponent<TextMesh>().text = Card.CardAge.ToString();
            DefendCardFrame.FindObject("Defend").GetComponent<TextMesh>().text = (((int)Card.CardAge) * 2).ToString();
            DefendCardFrame.FindObject("Exploration").GetComponent<TextMesh>().text = (((int)Card.CardAge)).ToString();


        }

        #endregion
    }
}
