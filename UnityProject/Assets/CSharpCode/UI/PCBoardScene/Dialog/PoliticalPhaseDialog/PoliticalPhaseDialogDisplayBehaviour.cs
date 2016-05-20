using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.ActionBinder;
using Assets.CSharpCode.UI.Util;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhaseDialog
{
    public class DialogItem
    {
        public GameObject CardObject;
        public CardInfo Card;
        public PlayerAction Action;
    }

    //对Political Dialog，ActionBinder和DisplayBehavior放一起比较好
    //共享一些内存数据
    public class PoliticalPhaseDialogDisplayBehaviour: MonoBehaviour,TtaActionBinder
    {

        #region 卡牌们

        public GameObject PlayEventFrame;
        private List<DialogItem> PlayEventDialogItems=new List<DialogItem>();

        public GameObject PlayColonyFrame;
        private List<DialogItem> PlayColonyDialogItems = new List<DialogItem>();

        public GameObject AggressionFrame;
        private List<DialogItem> AggressionDialogItems = new List<DialogItem>();

        public GameObject PactFrame;
        private List<DialogItem> PactDialogItems = new List<DialogItem>();

        public GameObject WarFrame;
        private List<DialogItem> WarDialogItems = new List<DialogItem>();

        public GameObject TearPactFrame;
        private List<DialogItem> TearPactDialogItems = new List<DialogItem>();

        public GameObject LeaderPowerFrame;

        #endregion

        public PCBoardBindedActionClickTrigger PassPoliticalPhaseButton;

        public PCBoardBehavior Behavior;
        public List<PlayerAction> Actions=new List<PlayerAction>(); 

        private bool _refreshRequired=false;
        
        public void Update()
        {
            if (_refreshRequired)
            {
                _refreshRequired = false;
                BindAndRefresh();
            }
        }
        
        public void Refresh()
        {
            _refreshRequired = true;

            float incr = 0;
            float start = 0;

            var militaryCards =
                SceneTransporter.CurrentGame.Boards[SceneTransporter.CurrentGame.MyPlayerIndex].MilitaryCards;

            //---第一行

            var eventCards = militaryCards.Where(c => c.CardType == CardType.Event).ToList();
            var colonyCards = militaryCards.Where(c => c.CardType == CardType.Colony).ToList();

            if (eventCards.Count + colonyCards.Count > 0)
            {
                incr = 5f / (eventCards.Count + colonyCards.Count);
            }
            incr = incr > 0.7f ? 0.7f : incr;

            start = FillFrameWithCard(PlayEventFrame, eventCards, PlayEventDialogItems, start, incr);
            FillFrameWithCard(PlayColonyFrame, colonyCards, PlayColonyDialogItems, start, incr);

            //第二行

            var aggressionCards = militaryCards.Where(c => c.CardType == CardType.Aggression).ToList();
            var pactCards = militaryCards.Where(c => c.CardType == CardType.Pact).ToList();

            start = 0;
            if (aggressionCards.Count + pactCards.Count > 0)
            {
                incr = 5f / (aggressionCards.Count + pactCards.Count);
            }
            incr = incr > 0.7f ? 0.7f : incr;

            start = FillFrameWithCard(AggressionFrame, aggressionCards, AggressionDialogItems, start, incr);
            FillFrameWithCard(PactFrame, pactCards, PactDialogItems, start, incr);

            LeaderPowerFrame.SetActive(false);

            //第三行
            var warCards = militaryCards.Where(c => c.CardType == CardType.War).ToList();
            start = 0;
            if (warCards.Count > 0)
            {
                incr = 5f / (warCards.Count);
            }
            incr = incr > 0.7f ? 0.7f : incr;
            FillFrameWithCard(WarFrame, warCards, WarDialogItems, start, incr);

            TearPactFrame.SetActive(false);

        }

        public void BindAction(List<PlayerAction> actions, PCBoardBehavior boardBehavior)
        {
            Actions = actions;
            Behavior = boardBehavior;
            _refreshRequired = true;

            //Buttons
            PassPoliticalPhaseButton.Bind(
                Actions.FirstOrDefault(a => a.ActionType == PlayerActionType.PassPoliticalPhase), Behavior);

        }

        public void Unbind()
        {
            //Do nothing
        }



        public void BindAndRefresh()
        {
        }

        private float FillFrameWithCard(GameObject frame,List<CardInfo> cards,List<DialogItem> dialogList,float start,float incr)
        {
            if (cards.Count == 0)
            {
                frame.SetActive(false);
                return start;
            }
            else
            {
                frame.SetActive(true);
            }

            dialogList.Clear();
            var cardFrame = frame.FindObject("CardFrame");
            foreach (Transform t in cardFrame.transform)
            {
                Destroy(t.gameObject);
            }


            var prefab = Resources.Load<GameObject>("Dynamic-PC/PCBoardCard-Small");
            int index = 0;
            for (; index < cards.Count; index++)
            {
                var card = cards[index];
                DialogItem item = new DialogItem {Card = card, CardObject = Instantiate(prefab)};

                item.CardObject.transform.parent = cardFrame.transform;
                item.CardObject.transform.localPosition = new Vector3(start+index*incr,0f,-0.1f);
                item.CardObject.GetComponent<PCBoardCardDisplayBehaviour>().Bind(item.Card);

                dialogList.Add(item);
            }

            return index * incr;
        }

        private void BindAction(List<PlayerAction> allActions)
        {
            //发动战争和发动侵略都是Internal的
            //发进去以后，会回复可以使用的玩家名单

        }
    }
}
