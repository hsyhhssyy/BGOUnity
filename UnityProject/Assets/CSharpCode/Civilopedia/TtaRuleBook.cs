using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Civilopedia
{
    /// <summary>
    /// Rulebook里面的函数和给出的数值，都不会计算其他卡牌的效果，仅仅返回基础规则的值。
    /// </summary>
    public  abstract class TtaRuleBook
    {
        public abstract int CountColonizeForceValue(List<CardInfo> selectedCard, CardInfo tacticInfo);

        /// <summary>
        /// 返回在蓝点为X时的腐败值，不计算其他卡牌和效果的影响。
        /// </summary>
        /// <param name="blueMarker"></param>
        /// <returns></returns>
        public abstract int CorruptionValue(int blueMarker);
        /// <summary>
        /// 返回在黄点为X时，回合结束消耗的粮食，不计算其他卡牌和效果的影响。
        /// </summary>
        /// <param name="yellowMarker"></param>
        /// <returns></returns>
        public abstract int ConsumptionValue(int yellowMarker);

        /// <summary>
        /// 返回在黄点为X时，拉一个消耗的基础粮食，不计算其他卡牌和效果对其的影响。
        /// </summary>
        /// <param name="yellowMarker"></param>
        /// <returns></returns>
        public abstract int FoodToIncreasePopulation(int yellowMarker);
    }
}
