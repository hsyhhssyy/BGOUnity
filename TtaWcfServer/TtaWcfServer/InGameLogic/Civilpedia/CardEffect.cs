﻿using System;
using System.Collections.Generic;

namespace TtaWcfServer.InGameLogic.Civilpedia
{
    public class CardEffect
    {
        public CardEffectType FunctionId { get; set; }

        public List<int> Data=new List<int>();
    }

    public class ChooseOneCardEffect: CardEffect
    {
        public ChooseOneCardEffect()
        {
            FunctionId = CardEffectType.ChooseOne;
        }

        public List<CardEffect> Candidate=new List<CardEffect>();
    }

    /// <summary>
    /// 为了执行编译/运行时类型检查，强制对int做范围校验
    /// </summary>
    public enum CardEffectType
    {
        Unknown = 0,
        //-------------E100------
        [CardEffectDetail("改变[0]类型的属性[1]点")] E100 = 100,
        [CardEffectDetail("建造资源建筑减少资源点消耗[0]")] E402 = 402,
        [CardEffectDetail("建造城市建筑减少资源点消耗[0]")] E403 = 403,
        [CardEffectDetail("升级资源建筑减少资源点消耗[0]")] E404 = 404,
        [CardEffectDetail("升级城市建筑减少资源点消耗[0]")] E405 = 405,
        [CardEffectDetail("研究科技后改变[0]类型的属性[1]点")] E408 = 408,

        //-----------Program Effect ---------
        ChooseOne = 1000,
    }

    public class CardEffectDetailAttribute : Attribute
    {
        public CardEffectDetailAttribute(String desc)
        {
            
        }
    }

    
}
