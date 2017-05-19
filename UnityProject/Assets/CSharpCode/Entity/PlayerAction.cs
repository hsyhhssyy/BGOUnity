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

        /// <summary>
        /// 表明该Action不代表一个具体的真实游戏存在的Action，而是向服务器发送一个指令。
        /// 目前用于选择类卡牌的效果。
        /// 注意internal action不能改变游戏面板，因为也因此，执行internal action不需要在得到结果后refresh board
        /// </summary>
        public bool Internal = false;

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
            String actionString;
            switch (ActionType)
            {
                case PlayerActionType.TakeCardFromCardRow:
                    actionString = "TakeCardFromCardRow:" + (Data[0] as CardInfo).InternalId+ " at "+Data[1];
                    break;
                default:
                    actionString = ""+Enum.GetName(typeof (PlayerActionType), ActionType);
                    break;
            }
            return actionString;
        }
    }
}
