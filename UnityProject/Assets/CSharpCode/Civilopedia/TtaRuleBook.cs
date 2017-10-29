using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia
{
    /// <summary>
    /// Rulebook里面的函数和给出的数值，都不会计算其他卡牌的效果，仅仅返回基础规则的值。
    /// </summary>
    public  abstract class TtaRuleBook
    {
        #region Rule Flags
        public const String ReturnCivilCostOnReplaceLeader = "ReturnCivilCostOnReplaceLeader";
        #endregion

        public readonly List<String> RuleFlags=new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedCard"></param>
        /// <param name="tacticInfo"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 判断一个建筑物是否是城市建筑物，从而决定建筑的数量是否受到政体的限制
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public abstract bool IsUrban(CardInfo card);
        /// <summary>
        /// 判断一个建筑物是否是军事建筑物，从而决定建筑的建造是否消耗红点
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public abstract bool IsMilitary(CardInfo card);
        /// <summary>
        /// 判断一张卡牌是否是科技牌（打出是否消耗科技）
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public abstract bool IsTechCard(CardInfo card);
        /// <summary>
        /// 判断一张卡牌是否是特殊科技牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public abstract bool IsSpecialTech(CardInfo card);
        /// <summary>
        /// 判断一张卡牌在放置于建筑区时应当置于哪一个位置
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public abstract BuildingType GetBuildingType(CardInfo card);

    }
}
