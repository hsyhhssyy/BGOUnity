using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.Civilpedia
{
    [DataContract]
    public class CardInfo : IEquatable<CardInfo>
    {
        public static String UnknownInternalId = "Unknown!";

        /// <summary>
        /// 返回一张不知道是什么内容的特定时代军事牌（用于网络传输）
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        public static CardInfo UnknownMilitaryCard(Age age)
        {
            return new CardInfo
            {
                CardName =
                    "Unknown Military Card",
                InternalId = ((int)(age)).ToString()+"-Unknown",
                CardType = CardType.Unknown,
                CardAge = age
            };
        }

        #region 基本数据
        [DataMember]
        public String InternalId;
        public CardType CardType;
        public Age CardAge;
        #endregion

        #region 文本
        public String CardName;
        public String Description;
        #endregion

        [XmlIgnore] public bool FromSerialization = true;

        public String Package;

        #region 游戏性数据

        public List<int> ResearchCost=new List<int>();
        public List<int> BuildCost = new List<int>();
        public List<int> RedMarkerCost    =new List<int>();

        [XmlIgnore]
        public List<CardEffect> ImmediateEffects = new List<CardEffect>();
        [XmlIgnore]
        public List<CardEffect> SustainedEffects=new List<CardEffect>();

        public List<int> AffectedTrget=new List<int>();
        [XmlIgnore]
        public List<CardEffect> WinnerEffects = new List<CardEffect>();
        [XmlIgnore]
        public List<CardEffect> LoserEffects = new List<CardEffect>();

        [XmlIgnore]
        public List<int> TacticComposition=new List<int>();
        [XmlIgnore]
        public List<int> TacticValue = new List<int>();

        [XmlIgnore]
        public List<CardEffect> LeaderActiveSkill = new List<CardEffect>();
        [XmlIgnore]
        public List<CardEffect> LeaderPassiveSkill = new List<CardEffect>();

        #endregion

        #region 美术
        /// <summary>
        /// 这张牌的牌面的70*105缩小版Sprite
        /// </summary>
        public String SmallImage;
        /// <summary>
        /// 这张牌的牌面全尺寸图片Sprite
        /// </summary>
        public String NormalImage;
        /// <summary>
        /// 这张牌的特殊牌面：<br/>
        /// 奇迹：奇迹的横条状图片<br/>
        /// 领袖：领袖的圆环形图片<br/>
        /// 殖民地：殖民地的横条图片<br/>
        /// 其他：空字符串
        /// </summary>
        public String SpecialImage;
        #endregion
        
        public CardInfo Clone()
        {
            CardInfo clone = (CardInfo)MemberwiseClone();
            return clone;
        }

        #region Equality Members

        public bool Equals(CardInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (InternalId == CardInfo.UnknownInternalId|| other.InternalId == CardInfo.UnknownInternalId)
            {
                return string.Equals(CardName  , other.CardName)&&CardAge.Equals(other.CardAge);
            }
            return string.Equals(InternalId, other.InternalId);
        }

        public override bool Equals(Object other)
        {
            if (!(other is CardInfo))
            {
                return false;
            }

            return Equals((CardInfo) other);
        }


        public override int GetHashCode()
        {
            return (InternalId != null ? InternalId.GetHashCode() : 0);
        }

        public static bool operator ==(CardInfo left, CardInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CardInfo left, CardInfo right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Requery

        public CardInfo CivilpediaCheck(TtaCivilopedia civilopedia)
        {
            if (civilopedia == null)
            {
                return this;
            }
            if (this.FromSerialization)
            {
                var cloneCard =civilopedia.GetCardInfoById(this.InternalId);

                return cloneCard;
            }
            return this;
        }
        #endregion
    }

}
