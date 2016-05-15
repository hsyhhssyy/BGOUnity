using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.Entity
{

    public class PlayerAction
    {
        public PlayerActionType ActionType;

        public Dictionary<Int32,Object> Data=new Dictionary<int, object>();

        public PlayerAction()
        {
            
        }

        public PlayerAction(Action act)
        {
            ActionType = PlayerActionType.ProgramDelegateAction;
            Data[0] = act;
        }

        public String GetDescription()
        {
            switch (ActionType)
            {
                case PlayerActionType.BuildBuilding:
                    return "建造[" + ((CardInfo) Data[0]).CardName + "]";
                case PlayerActionType.UpgradeBuilding:
                    return "升级[" + ((CardInfo) Data[0]).CardName + "] -> [" + ((CardInfo) Data[1]).CardName + "]";
                case PlayerActionType.Destory:
                    return "摧毁[" + ((CardInfo) Data[0]).CardName + "]";
                case PlayerActionType.Disband:
                    return "拆除[" + ((CardInfo)Data[0]).CardName + "]";
                default:
                    return this.ToString();
            }
        }

        public override string ToString()
        {
            String actionString = "";
            switch (ActionType)
            {
                case PlayerActionType.TakeCardFromCardRow:
                    actionString = "TakeCardFromCardRow:" + (Data[0] as CardInfo).InternalId+ " at "+Data[1];
                    break;
                default:
                    actionString = "Unknown Action:" + Enum.GetName(typeof (PlayerActionType), ActionType);
                    break;
            }
            return actionString;
        }
    }
}
