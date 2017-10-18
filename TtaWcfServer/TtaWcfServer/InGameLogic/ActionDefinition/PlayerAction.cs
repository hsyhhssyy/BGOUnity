using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using NHibernate.Criterion;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.ActionDefinition
{
    [DataContract]
    [KnownType(typeof(PlayerAction))]
    [KnownType(typeof(CardRowInfo))]
    [KnownType(typeof(CardInfo))]
    [KnownType(typeof(Dictionary<CardInfo, int>))]
    public class PlayerAction
    {
        public PlayerAction()
        {
            Guid = System.Guid.NewGuid().ToString("N").ToUpper();
        }

        [DataMember]
        public readonly String Guid;

        [DataMember]
        public PlayerActionType ActionType;

        /// <summary>
        /// 注意，该类型因为需要进行Wcf传递，因此每一个可能的类型，都需要在KnownType中登记
        /// </summary>
        [DataMember]
        public Dictionary<Int32, Object> Data = new Dictionary<int, object>();

        /// <summary>
        /// 表明该Action不代表一个具体的真实游戏存在的Action，而是向服务器发送一个指令。
        /// 目前用于选择类卡牌的效果。
        /// 注意internal action不能改变游戏面板，因为也因此，执行internal action不需要在得到结果后refresh board
        /// </summary>
        [DataMember]
        public bool Internal = false;

        public override bool Equals(object obj)
        {
            return Equals((PlayerAction)obj);
        }

        protected bool Equals(PlayerAction other)
        {
            return string.Equals(Guid, other.Guid);
        }

        public override int GetHashCode()
        {
            return (Guid != null ? Guid.GetHashCode() : 0);
        }
    }
}