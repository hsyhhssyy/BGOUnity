using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.Wcf;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.Effects;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.ActionDefinition.Handlers
{
    public class TakeCardFromCardRowActionHandler:ActionHandler
    {
        public TakeCardFromCardRowActionHandler(GameManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> CheckAbleToPerform(int playerNo)
        {
            if (Manager.CurrentGame.CurrentPhase != TtaPhase.ActionPhase &&
                Manager.CurrentGame.CurrentPhase != TtaPhase.FirstTurnActionPhase)
            {
                return new List<PlayerAction>();
            }
            if (Manager.CurrentGame.CurrentPlayer != playerNo)
            {
                return new List<PlayerAction>();
            }

            var actions = new List<PlayerAction>();
            TtaBoard board = Manager.CurrentGame.Boards[playerNo];
            var cost=CalcuateCardRowCost(Manager, playerNo);
            for (int index = 0; index < Manager.CurrentGame.CardRow.Count; index++)
            {
                var info = Manager.CurrentGame.CardRow[index];
                if (info.TakenBy == -1)
                {
                    if (cost[index] !=-1 && cost[index] <= board.UncountableResourceCount[ResourceType.WhiteMarker])
                    {
                        PlayerAction action=new PlayerAction();
                        action.ActionType= PlayerActionType.TakeCardFromCardRow;
                        action.Data[0] = info.Card;
                        action.Data[1] = index;
                        actions.Add(action);
                    }

                }
                else if (info.TakenBy == playerNo)
                {
                    PlayerAction action = new PlayerAction();
                    action.ActionType = PlayerActionType.PutBackCard;
                    action.Data[0] = info.Card;
                    actions.Add(action);
                }
            }

            return actions;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<int, object> data, WcfContext context)
        {
           //不做校验
            return null;
        }

        /// <summary>
        /// 返回对应于卡牌列每一张卡的内政消耗，-1表示不可以拿
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public static List<int> CalcuateCardRowCost(GameManager manager,int playerNo)
        {
            //收集一些公用的数据
            TtaBoard board = manager.CurrentGame.Boards[playerNo];
            int wonderCount = board.CompletedWonders.Count;

            List < int > costs=new List<int>();
            //遍历卡牌列
            for (int index = 0; index < 13; index++)
            {
                CardRowInfo info = manager.CurrentGame.CardRow[index];

                int baseCost = index < 5 ? 1 : (index < 9 ? 2 : 3);

                switch (info.Card.CardType)
                {
                    case CardType.Leader:
                    {
                        //E608禁止拿领袖
                        var count = board.EffectPool.FilterEffect(CardEffectType.E608, (int) info.Card.CardAge).Count();
                        if (count > 0)
                        {
                            baseCost = -1;
                            break;
                        }
                        //E411拿领袖减少白
                        var effects = board.EffectPool.FilterEffect(CardEffectType.E411);
                        foreach (var cardEffect in effects)
                        {
                            baseCost -= cardEffect.Data[0];
                        }
                        if (baseCost < 0)
                        {
                            baseCost = 0;
                        }
                        break;
                    }
                    case CardType.Government:

                    case CardType.MilitaryTechAirForce:
                    case CardType.MilitaryTechCavalry:
                    case CardType.MilitaryTechArtillery:
                    case CardType.MilitaryTechInfantry:

                    case CardType.ResourceTechFarm:
                    case CardType.ResourceTechMine:

                    case CardType.SpecialTechCivil:
                    case CardType.SpecialTechEngineering:
                    case CardType.SpecialTechExploration:
                    case CardType.SpecialTechMilitary:

                    case CardType.UrbanTechLab:
                    case CardType.UrbanTechArena:
                    case CardType.UrbanTechLibrary:
                    case CardType.UrbanTechTemple:
                    case CardType.UrbanTechTheater:
                    {
                        //检查对应位置和手牌
                        if (board.CivilCards.Contains(info.Card))
                        {
                            baseCost = -1;
                            break;
                        }
                        if (board.Government == info.Card)
                        {
                            baseCost = -1;
                            break;
                        }
                        baseCost = board.AggregateOnBuildingCell(baseCost,
                            ((i, cell) => cell.Card == info.Card ? -1 : i));
                        break;
                    }
                    case CardType.Wonder:
                    {
                        if (board.ConstructingWonder != null)
                        {
                            baseCost = -1;
                            break;
                        }
                        var count = board.EffectPool.FilterEffect(CardEffectType.E412, (int) info.Card.CardAge).Count();
                        if (count > 0)
                        {
                            break;
                        }
                        baseCost += wonderCount;
                        break;
                    }
                }

                costs.Add(baseCost);
            }
            return costs;
        }
    }
}