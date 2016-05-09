using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia
{
    public class CardInfo
    {
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
        public String InternalId;
        public CardType CardType;
        public Age CardAge;
        #endregion

        #region 文本
        public String CardName;
        public String Description;
        #endregion

        public String Package;

        #region 游戏性数据

        public List<int> ResearchCost=new List<int>();
        public List<int> BuildCost = new List<int>();
        public List<int> RedMarkerCost    =new List<int>();

        public List<CardEffect> ImmediateEffects = new List<CardEffect>();

        public List<CardEffect> OneTimeEffects = new List<CardEffect>();
        public List<CardEffect> SustainedEffects=new List<CardEffect>();

        public List<CardEffect> WinnerEffects = new List<CardEffect>();
        public List<CardEffect> LoserEffects = new List<CardEffect>();

        public List<int> TacticComposition=new List<int>();
        public List<int> TacticValue = new List<int>();
        
        public List<CardEffect> LeaderActiveSkill = new List<CardEffect>();
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

        public override bool Equals(object obj)
        {
            if (!(obj is CardInfo))
            {
                return false;
            }
            return InternalId.Equals(((CardInfo)obj).InternalId);
        }
    }
    
}
