using System;
using System.Collections.Generic;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.PCBoardScene.Effects;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class HandCardChildController: TtaUIControllerMonoBehaviour
    {
        public HandCardParentController ParentController;
        public CardInfo Card;

        public bool Highlight { get; set; }

        private PCBoardCardSmallHighlightEffect highligtGo ;

        [UsedImplicitly]
        public void Start()
        {
            UIKey = "PCBoard."+ ParentController.HandType+ ".Child." + Guid;
            ParentController.Manager.Regiseter(this);
            highligtGo=gameObject.AddComponent<PCBoardCardSmallHighlightEffect>();
        }

        public void Update()
        {
            if (Highlight)
            {
                highligtGo.Highlight = true;
                return;
            }
            if (ParentController.Manager.State == GameManagerState.ActionPhaseChooseTarget &&
                ParentController.Manager.StateData.ContainsKey("HighlightElement"))
            {
                var dict = ParentController.Manager.StateData["HighlightElement"] as Dictionary<String, List<CardInfo>>;
                if (dict != null && dict.ContainsKey("HandCivilCard"))
                {
                    if (dict["HandCivilCard"].Contains(Card))
                    {
                        highligtGo.Highlight = true;
                        return;
                    }
                }
            }

            highligtGo.Highlight = false;
        }

        protected override void OnSubscribedGameEvents(Object sender, GameUIEventArgs args)
        {
            if (!args.UIKey.Contains(Guid))
            {
                return;
            }

            //含有自己的GUID，是发给自己的
            if (args.EventType == GameUIEventType.AllowSelect)
            {
                Highlight = true;
            }
        }

        public override bool OnTriggerEnter()
        {
            var args = new ControllerGameUIEventArgs(GameUIEventType.TrySelect, UIKey);

            //附加Card
            args.AttachedData["Card"] = Card;

            Channel.Broadcast(args);
            return true;
        }

        public override bool OnTriggerExit()
        {
            Highlight = false;
            return true;
        }

        public override bool OnTriggerClick()
        {
            if (!Highlight) return false;

            var args = new ControllerGameUIEventArgs(GameUIEventType.Selected, UIKey);

            //附加Card
            args.AttachedData["Card"] = Card;

            Channel.Broadcast(args);

            return true;
        }
    }
}
