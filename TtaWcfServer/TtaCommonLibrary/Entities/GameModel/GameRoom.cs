using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HSYErpBase.EntityDefinition;
using TtaCommonLibrary.Entities.UserModel;

namespace TtaCommonLibrary.Entities.GameModel
{
    [DataContract]
    public class GameRoom:BasicEntity
    {
        [DataMember]
        public virtual String RoomName { get; set; }
        [DataMember]
        public virtual bool HasPassword { get; set; }

        [DataMember]
        public virtual int RelatedMatchId { get; set; }

        [DataMember]
        public virtual bool Ranked { get; set; }

        //房间的密码不能传递给用户（但是可以明文存储）
        public virtual String Password { get; set; }

        [DataMember]
        public virtual UserLite Host { get; set; }

        [DataMember]
        public virtual int PlayerMax { get; set; }
        [DataMember]
        public virtual int ObserverMax { get; set; }
        [DataMember]
        public virtual int RefereeMax { get; set; }
        [DataMember]
        public virtual List<UserLite> JoinedPlayer { get; set; }
        [DataMember]
        public virtual List<UserLite> ObserverPlayer { get; set; }
        [DataMember]
        public virtual List<UserLite> RefereePlayer { get; set; }

        [DataMember]
        public virtual ObserverMode ObserverSetting { get; set; }

        [DataMember]
        public virtual bool AutoStart { get; set; }
        [DataMember]
        public virtual GameRoomSetting TurnSetting { get; set; }
        [DataMember]
        public virtual String GameRuleVersion { get; set; }
        [DataMember]
        public virtual List<String> AdditionalRules { get; set; }

        #region 用于格式化处理的变量
        /// <summary>
        /// 服务器变量
        /// </summary>
        public virtual String Observers { get; set; }
        /// <summary>
        /// 服务器变量
        /// </summary>
        public virtual String Referees { get; set; }
        /// <summary>
        /// 服务器变量
        /// </summary>
        public virtual String Players { get; set; }
        /// <summary>
        /// 服务器变量
        /// </summary>
        public virtual int HostId { get; set; }
        /// <summary>
        /// 服务器变量
        /// </summary>
        public virtual String AdditionalRulesStr { get; set; }
        #endregion
    }

    [Flags]
    public enum ObserverMode
    {
        HideCardRow,HideMilitaryCard,HideEventDeck,ObserverMute, ObserverMuteToPlayer

    }
    public enum GameRoomSetting
    {
        Unlimited,
        TurnLimit24Hr,
        TurnLimit1Hr,
        TurnLimit30Min,
        TurnLimit5Min,
        TurnLimit1Min,
        TotalLimit24Hr,
        TotalLimit12Hr,
        TotalLimit4Hr,
        TotalLimit2Hr,
        TotalLimit1Hr,
        TotalLimit30Min,

    }
}
