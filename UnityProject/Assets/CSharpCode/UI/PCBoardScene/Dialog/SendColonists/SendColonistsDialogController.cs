using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.Util.UnityEnhancement;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.SendColonists
{
    public class SendColonistsDialogController : DialogController
    {
        public PCBoardCardDisplayBehaviour ColonyCard;
        public TextMesh ForceNeededTextMesh;
        public TextMesh ForceCurrentTextMesh;
        public DialogButtonController ConfirmButton;

        public GameObject CardFrame;

        private GameObject _smallCardPrefeb;

        public override void Start()
        {
            ConfirmButton.Data = this;
            base.Start();
        }
        

        public int NeedValue { get; private set; }
        public int CurrentValue { get; private set; }

        protected override void Refresh()
        {
            if (_smallCardPrefeb == null)
            {
                _smallCardPrefeb = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");
            }
            var action=Manager.CurrentGame.PossibleActions.FirstOrDefault(a => a.ActionType == PlayerActionType.SendColonists);
            if (action != null)
            {
                ColonyCard.Bind(action.Data[0] as CardInfo);

                //显示你需要的出价
                ForceNeededTextMesh.text = "您需要提供（"+action.Data[1] + "）点军力";
                NeedValue = Convert.ToInt32(action.Data[1]);

                //显示部队
                CardFrame.RemoveAllTransformChildren();

                List<CardInfo> cardsToDisplay=new List<CardInfo>();
                var board=this.Manager.CurrentGame.Boards[Manager.CurrentGame.MyPlayerIndex];
                AddMilitaryForceToArray(cardsToDisplay, board, BuildingType.AirForce);
                AddMilitaryForceToArray(cardsToDisplay, board, BuildingType.Artillery);
                AddMilitaryForceToArray(cardsToDisplay, board, BuildingType.Cavalry);
                AddMilitaryForceToArray(cardsToDisplay, board, BuildingType.Infantry);

                //显示防御卡

                cardsToDisplay.AddRange(board.MilitaryCards.Where(card=>card.CardType== CardType.Defend));

                //每行6张卡
                //从-1到2.5 +0.8
                //从-0.35到-1.5

                int row = cardsToDisplay.Count/6;
                float height = row < 2 ? -1.15f : -1.15f / (row-1);
                for (int y = 0;; y++)
                {
                    if (y * 6 >= cardsToDisplay.Count) { break; }
                    for (int x = 0; x < 6; x++)
                    {
                        if (y * 6 +x >= cardsToDisplay.Count) { break; }

                        GameObject mSp = Instantiate(_smallCardPrefeb);
                        mSp.transform.SetParent(CardFrame.transform);
                        mSp.transform.localPosition = new Vector3(-1f+x*0.8f, -0.35f+y*height);
                        mSp.transform.localScale = new Vector3(1f, 1f, 1f);
                        mSp.GetComponent<PCBoardCardDisplayBehaviour>().Bind(cardsToDisplay[x+y*6]);
                        var cardCtrl =
                            mSp.AddComponent<SendColonistsCardSelectionController>();
                        cardCtrl.Manager = Manager;
                    }
                }

                //显示你已经选择的部队的军力
            }
        }

        public List<CardInfo> CollectSelectedCards()
        {
            var cards = new List<CardInfo>();
            foreach (Transform t in CardFrame.transform)
            {
                var go = t.gameObject;
                if (go.GetComponent<SendColonistsCardSelectionController>().Selected)
                {
                    cards.Add(go.GetComponent<PCBoardCardDisplayBehaviour>().Card);
                }
            }
            return cards;
        }

        public void UpdateColonizeForceValue()
        {
            var pedia = TtaCivilopedia.GetCivilopedia(Manager.CurrentGame.Version);
            var ruleBook = pedia.GetRuleBook();

            var cards = CollectSelectedCards();

            if (!cards.Any(cardInfo => cardInfo.CardType == CardType.MilitaryTechAirForce ||
                                       cardInfo.CardType == CardType.MilitaryTechArtillery
                                       || cardInfo.CardType == CardType.MilitaryTechCavalry ||
                                       cardInfo.CardType == CardType.MilitaryTechInfantry))
            {
                ForceCurrentTextMesh.text = "至少牺牲一支部队";
                CurrentValue = -1;
            }
            else
            {
                var value = ruleBook.CountColonizeForceValue(cards, Manager.CurrentGame.Boards[Manager.CurrentGame.MyPlayerIndex].Tactic);

                ForceCurrentTextMesh.text = "当前（" + value + "）点军力";
                CurrentValue = value;
            }

            return;
        }

        private void AddMilitaryForceToArray(List<CardInfo> cardsToDisplay, TtaBoard board, BuildingType type)
        {
            if (!board.Buildings.ContainsKey(type))
            {
                return;
            }
            var building = board.Buildings[type];
            foreach (var pair in building)
            {
                for(int i= 0; i < pair.Value.Worker;i++)
                {
                    cardsToDisplay.Add(
                    pair.Value.Card);
                }
            }
        }
    }
}
