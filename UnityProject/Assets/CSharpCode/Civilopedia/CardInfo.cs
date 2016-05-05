using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia
{
    public class CardInfo
    {
        /// <summary>
        /// 0-Frugality
        /// 1-Frederick Barbarossa
        /// 2-Napoleon Bonaparte
        /// 3-Impact of Architecture
        /// </summary>
        public String InternalId;

        public Age CardAge;
        public String CardName;
        public CardType CardType;

        //Arts
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

        //ServerData
        public Dictionary<String,Object> ServerData=new Dictionary<string, object>();



        public CardInfo Clone()
        {
            CardInfo clone = (CardInfo)MemberwiseClone();
            clone.ServerData = new Dictionary<string, object>();
            return clone;
        }
    }
}
