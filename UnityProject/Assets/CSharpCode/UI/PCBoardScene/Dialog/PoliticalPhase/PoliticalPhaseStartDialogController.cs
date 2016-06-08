using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.CommonPrefab;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using Assets.CSharpCode.UI.PCBoardScene.Effects;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Dialog.PoliticalPhase
{
    public class DialogItem
    {
        public GameObject CardObject;
        public CardInfo Card;
        public PlayerAction Action;
    }


    //Political Dialog的Parent Controller，只负责生成各个UI组件，目前不负责任何其他任何事件
    [UsedImplicitly]
    public class PoliticalPhaseStartDialogController: DisplayOnlyUIController
    {

        #region 卡牌区域Frame

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

        private bool display = false;

        public void DisplayDialog()
        {
            display = true;
            RefreshRequired = true;
        }
        public void HideDialog()
        {
            display = false;
        }

        protected override void Refresh()
        {
            this.gameObject.SetActive(display);

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

                var cc=item.CardObject.AddComponent<PoliticalPhaseCardController>();
                cc.Card = item.Card;
                cc.Manager = Manager;

                item.CardObject.AddComponent<PCBoardCardSmallHighlightEffect>();

                dialogList.Add(item);
            }

            return index * incr;
        }


    }
}
