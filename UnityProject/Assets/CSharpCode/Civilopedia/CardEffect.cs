using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Civilopedia
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
        Unknown=0,
        //-------------E100------
       ResourceOfTypeXChangedY = 100,

        //E408
        DevelopATechThenChangeYofResourceX=408,//研究科技后改变[0]类型的属性[1]点

        //-----------Program Effect ---------
        ChooseOne = 1000,
    }
}
