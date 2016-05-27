using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    [UsedImplicitly]
    public class ColonyController:SimpleClickUIController
    {
        public GameObject ColonyFrame;

        protected override void Refresh()
        {
            var board = Manager.CurrentGame.Boards[Manager.CurrentDisplayingBoardNo];

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

        protected override string GetUIKey()
        {
            return "PCBoard.Colony." + Guid;
        }
    }
}
