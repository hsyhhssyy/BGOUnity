using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia
{
    [DataContract]
    public class CardInfo : IEquatable<CardInfo>
    {
        public static String UnknownInternalId = "Unknown!";

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

        /// <summary>
        /// 表示所包含的扩展包
        /// </summary>
        public String Package;

        #region 游戏性数据

        /// <summary>
        /// 科技牌的研发消耗，第二个数字专为政体准备，表示革命消耗
        /// </summary>
        public List<int> ResearchCost=new List<int>();
        /// <summary>
        /// 建筑类科技牌的建造消耗（存于[0]），或者奇迹牌每一步的建造消耗（以列表形式，第一步为[0]）
        /// </summary>
        public List<int> BuildCost = new List<int>();
        /// <summary>
        /// 战争牌，侵略牌，阵型牌的红点消耗，保存于[0]
        /// </summary>
        public List<int> RedMarkerCost    =new List<int>();

        /// <summary>
        /// 卡牌在某些情况下立即生效的结果（具体时机由卡牌决定）
        /// </summary>
        public List<CardEffect> ImmediateEffects = new List<CardEffect>();
        /// <summary>
        /// 卡牌效果生效（具体时机由卡牌决定）时，开始对玩家产生影响，并在卡牌被丢弃时失效的效果。
        /// </summary>
        public List<CardEffect> SustainedEffects = new List<CardEffect>();
        /// <summary>
        /// 卡牌被玩家获取后，立刻附加在玩家上的状态效果，在玩家失去卡牌后，效果也不会消失。
        /// FlagEffect能够取消自己或者其他FlagEffect，能够如同正常Effect一样在每个检索阶段被检索到。
        /// 并且Effect的生效和失效可以指定一张卡进行跟踪，具体请查阅各个FlagEffect的具体说明。
        /// </summary>
        public List<CardEffect> FlagEffects = new List<CardEffect>();

        /// <summary>
        /// 表示事件牌影响的的目标，[0]是强大文明数，[1]是弱小文明数
        /// </summary>
        public List<int> AffectedTrget=new List<int>();
        /// <summary>
        /// 表示战争，侵略的获胜方，条约的A，事件影响的强者将结算的效果
        /// </summary>
        public List<CardEffect> WinnerEffects = new List<CardEffect>();
        /// <summary>
        /// 表示战争的落败方，侵略的失败方，条约的B，事件影响的弱者将结算的效果
        /// </summary>
        public List<CardEffect> LoserEffects = new List<CardEffect>();

        /// <summary>
        /// 阵型组成方式，N个数字各自代表兵马炮飞机
        /// </summary>
        public List<int> TacticComposition=new List<int>();
        /// <summary>
        /// 阵型加成奖励，分别代表全部同时代，低一个时代，低两个时代，低三个时代的阵型加成
        /// TODO 目前是代表不低于两个时代，低两个时代，需要修改
        /// </summary>
        public List<int> TacticValue = new List<int>();

        /// <summary>
        /// 主动激活的效果，激活后，该效果将会作为ImmediateEffect被立刻结算
        /// </summary>
        public List<CardEffect> ActivateEffects = new List<CardEffect>();

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
    }

}
