using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.StatusBar
{
    class OtherPlayerStatusBarController:DisplayOnlyUIController
    {
        public TextMesh Username;
        public TextMesh PhaseName;

        protected override void Refresh()
        {
            if (Manager.CurrentGame.CurrentPlayer != Manager.CurrentGame.MyPlayerIndex
                &&SceneTransporter.Server.ServerType== ServerType.PassiveServer2Sec)
            {
                this.gameObject.SetActive(true);
                Username.text = Manager.CurrentGame.Boards[Manager.CurrentGame.CurrentPlayer].PlayerName;
                switch (Manager.CurrentGame.CurrentPhase)
                {
                    case TtaPhase.PoliticalPhase:
                        PhaseName.text = "政治行动";
                        break;
                    case TtaPhase.ActionPhase:
                        case TtaPhase.FirstTurnActionPhase:
                        PhaseName.text = "内政行动";
                        break;
                    case TtaPhase.DiscardPhase:
                        PhaseName.text = "弃牌";
                        break;
                    case TtaPhase.ProductionPhase:
                        PhaseName.text = "生产";
                        break;
                    case TtaPhase.EventResolution:
                        PhaseName.text = "事件结算";
                        break;
                    default:
                        PhaseName.text = "执行操作";
                        break;
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
