using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic;
using Assets.CSharpCode.Network.Wcf.Entities;
using Assets.CSharpScripts.Helper;

namespace Assets.CSharpCode.Network.Wcf
{
    public class WcfActionFormater
    {
        public static void Format(WcfGame game, JSONObject json,TtaCivilopedia civilopedia)
        {
            game.PossibleActions =new List<PlayerAction>();
            
            var serverActions = new List<PlayerAction>();

            foreach (var jsonObject in json.list)
            {
                serverActions.Add(CreateActionFromJson(jsonObject));
            }

            FormatJsonActions(serverActions,game, civilopedia);

            var clientActions=GameLogicManager.CurrentManager.GetPossibleActions(game.MyPlayerIndex);
            game.PossibleActions.AddRange(clientActions);

        }

        private static PlayerAction CreateActionFromJson(JSONObject json)
        {
            WcfPlayerAction action=new WcfPlayerAction();
            action.ActionType = (PlayerActionType) json.TryGetField("ActionType").i;
            action.Internal = json.TryGetField("Internal").b;
            //处理Guid
            action.Guid = json.TryGetField("Guid").str;
            //处理Data
            var dataJson = json.TryGetField("Data");//是Key Value Pair 的 Array
            action.JsonData=new Dictionary<int, JSONObject>();
            foreach (var jsonObject in dataJson.list)
            {
                action.JsonData.Add((int) jsonObject.TryGetField("Key").i,jsonObject.TryGetField("Value"));
            }

            return action;
        }

        private static void FormatJsonActions(List<PlayerAction> actions,WcfGame game, TtaCivilopedia civilopedia)
        {
            foreach (var possibleAction in actions)
            {
                var wcfAction = possibleAction as WcfPlayerAction;
                if (wcfAction == null) { continue;}

                switch (wcfAction.ActionType)
                {
                    /*下面这些都交给GameLogic了
                    //不需要Format的Action包括
                    case PlayerActionType.EndActionPhase:
                        case PlayerActionType.PassPoliticalPhase:
                        break;
                    case PlayerActionType.TakeCardFromCardRow:
                        wcfAction.Data[0] = game.CardRow[(int)wcfAction.JsonData[1].i];
                        wcfAction.Data[1] = (int)wcfAction.JsonData[1].i;
                        break;
                    case PlayerActionType.PutBackCard:
                        wcfAction.Data[0] = game.CardRow[(int)wcfAction.JsonData[1].i];
                        wcfAction.Data[1] = (int)wcfAction.JsonData[1].i;
                        break;*/
                        /*需要的就放进去
                     game.PossibleActions.Add(wcfAction);    
                     */
                }
            }
        }
    }
}
