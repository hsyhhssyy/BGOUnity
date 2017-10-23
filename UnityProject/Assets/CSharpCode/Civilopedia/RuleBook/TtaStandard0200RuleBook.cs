﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Civilopedia.RuleBook
{
    /// <summary>
    /// 标准TTA2.0的规则书
    /// </summary>
    public class TtaStandard0200RuleBook:TtaRuleBook
    {
        public override int CountColonizeForceValue(List<CardInfo> SelectedCard,CardInfo tacticInfo)
        {
            //先加上Defence
            int force = SelectedCard.Where(cardInfo => cardInfo.CardType == CardType.Defend).Sum(cardInfo => (int) cardInfo.CardAge);

            var military=new Dictionary<CardType, Dictionary<Age, int>>();
            int[] forceArray = new int[] {1, 2, 3, 5};
            //在加上部队
            foreach (var cardInfo in SelectedCard)
            {
                if (cardInfo.CardType != CardType.MilitaryTechAirForce &&
                    cardInfo.CardType != CardType.MilitaryTechArtillery
                    && cardInfo.CardType != CardType.MilitaryTechCavalry &&
                    cardInfo.CardType != CardType.MilitaryTechInfantry)
                {
                    continue;
                }

                if (!military.ContainsKey(cardInfo.CardType))
                {
                    military.Add(cardInfo.CardType,new Dictionary<Age, int>());
                }
                var dict = military[cardInfo.CardType];
                if (!dict.ContainsKey(cardInfo.CardAge))
                {
                    dict.Add(cardInfo.CardAge,0);
                }

                dict[cardInfo.CardAge] += 1;

                force += forceArray[(int) cardInfo.CardAge];
            }

            //再加上阵型

            //再加上库克
            return force;
        }

        public override int CorruptionValue(int blueMarker)
        {
            if (blueMarker <= 0)
            {
                return 6;
            }
            else if (blueMarker <= 5)
            {
                return  4;
            }
            else if (blueMarker <= 10)
            {
                return  2;
            }
            return 0;
        }

        public override int ConsumptionValue(int yellowMarker)
        {
            if (yellowMarker <= 0)
            {
                return 6;
            }
            else if (yellowMarker <= 4)
            {
                return 4;
            }
            else if (yellowMarker <= 8)
            {
                return 3;
            }
            else if (yellowMarker <= 12)
            {
                return 2;
            }
            else if (yellowMarker <= 16)
            {
                return 1;
            }
            return 0;
        }

        public override int FoodToIncreasePopulation(int yellowMarker)
        { if (yellowMarker <= 4)
            {
                return 7;
            }
            else if (yellowMarker <= 8)
            {
                return 5;
            }
            else if (yellowMarker <= 12)
            {
                return 4;
            }
            else if (yellowMarker <= 16)
            {
                return 3;
            }
            return 2;
        }
    }
}
