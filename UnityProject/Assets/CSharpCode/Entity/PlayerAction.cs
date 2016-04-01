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
