using System;
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
        public TtaStandard0200RuleBook()
        {
            RuleFlags.Add(TtaRuleBook.ReturnCivilCostOnReplaceLeader);
        }

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

        public override bool IsMilitary(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.MilitaryTechAirForce:
                case CardType.MilitaryTechArtillery:
                case CardType.MilitaryTechCavalry:
                case CardType.MilitaryTechInfantry:
                    return true;
                    default:
                        return false;
            }
        }

        public override bool IsTechCard(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.MilitaryTechAirForce:
                case CardType.MilitaryTechArtillery:
                case CardType.MilitaryTechCavalry:
                case CardType.MilitaryTechInfantry:

                case CardType.UrbanTechLab:
                case CardType.UrbanTechArena:
                case CardType.UrbanTechLibrary:
                case CardType.UrbanTechTemple:
                case CardType.UrbanTechTheater:

                case CardType.Government:

                case CardType.SpecialTechCivil:
                case CardType.SpecialTechEngineering:
                case CardType.SpecialTechExploration:
                case CardType.SpecialTechMilitary:

                case CardType.ResourceTechFarm:
                case CardType.ResourceTechMine:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsSpecialTech(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.SpecialTechCivil:
                case CardType.SpecialTechEngineering:
                case CardType.SpecialTechExploration:
                case CardType.SpecialTechMilitary:
                    return true;
                default:
                    return false;
            }
        }
        public override bool IsUrban(CardInfo card)
        {
            switch (card.CardType)
            {

                case CardType.UrbanTechLab:
                case CardType.UrbanTechArena:
                case CardType.UrbanTechLibrary:
                case CardType.UrbanTechTemple:
                case CardType.UrbanTechTheater:
                    return true;
                default:
                    return false;
            }
        }
        public override BuildingType GetBuildingType(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.ResourceTechFarm:
                    return BuildingType.Farm;
                case CardType.ResourceTechMine:
                    return BuildingType.Mine;

                case CardType.UrbanTechLab:
                    return BuildingType.Lab;
                case CardType.UrbanTechArena:
                    return BuildingType.Arena;
                case CardType.UrbanTechLibrary:
                    return BuildingType.Library;
                case CardType.UrbanTechTemple:
                    return BuildingType.Temple;
                case CardType.UrbanTechTheater:
                    return BuildingType.Theater;

                case CardType.MilitaryTechAirForce:
                    return BuildingType.AirForce;
                case CardType.MilitaryTechArtillery:
                    return BuildingType.Artillery;
                case CardType.MilitaryTechCavalry:
                    return BuildingType.Cavalry;
                case CardType.MilitaryTechInfantry:
                    return BuildingType.Infantry;
                default:
                    return BuildingType.Unknown;
            }
        }
    }
}
