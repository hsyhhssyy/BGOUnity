using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    public class TakeCardFromCardRowActionHandler:ActionHandler
    {
        public TakeCardFromCardRowActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
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
                if (info.CanTake)
                {
                    if (cost[index] !=-1 && cost[index] <= board.Resource[ResourceType.WhiteMarker])
                    {
                        PlayerAction action = new PlayerAction {ActionType = PlayerActionType.TakeCardFromCardRow};
                        action.Data[0] = info;
                        action.Data[1] = index;
                        actions.Add(action);
                    }

                }
                else if (info.CanPutBack)
                {
                    PlayerAction action = new PlayerAction {ActionType = PlayerActionType.PutBackCard};
                    action.Data[0] = info;
                    action.Data[1] = index;
                    actions.Add(action);
                }
            }

            return actions;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            ActionResponse response=new ActionResponse();
               var board = Manager.CurrentGame.Boards[playerNo];
            if (action.ActionType == PlayerActionType.TakeCardFromCardRow)
            {
                CardRowInfo info= action.Data[0] as CardRowInfo;
                if (info == null)
                {
                    throw new InvalidOperationException("错误的ActionData");
                }

                var card = info.Card;
                info.CanPutBack = true;
                info.CanTake = false;
                board.Resource[ResourceType.WhiteMarker] -= info.CivilActionCost;
                response.Changes.Add(
                    GameMove.Resource(ResourceType.WhiteMarker, board.Resource[ResourceType.WhiteMarker] + info.CivilActionCost,
                        board.Resource[ResourceType.WhiteMarker]));
                if (card.CardType != CardType.Wonder)
                {
                    board.CivilCards.Add(new HandCardInfo(card,Manager.CurrentGame.CurrentRound));
                    response.Changes.Add(GameMove.TakeCard((int)action.Data[1],card));
                }
                else
                {
                    board.ConstructingWonder = card;
                    board.ConstructingWonderSteps = 0;

                    response.Changes.Add(GameMove.TakeWonder((int)action.Data[1], card));
                }

                //结算Flag（能从卡牌列拿到的卡牌，Flag时机都是拿取时）
                board.EffectPool.AttachFlags(card.FlagEffects);

                return response;
            }
            else if (action.ActionType == PlayerActionType.PutBackCard)
            {
                CardRowInfo info = action.Data[0] as CardRowInfo;
                if (info == null)
                {
                    throw new InvalidOperationException("错误的ActionData");
                }

                var card = info.Card;
                info.CanPutBack = false;
                info.CanTake = true;
                board.Resource[ResourceType.WhiteMarker] += info.CivilActionCost;
                response.Changes.Add(
                    GameMove.Resource(
                        ResourceType.WhiteMarker, board.Resource[ResourceType.WhiteMarker] - info.CivilActionCost,
                        board.Resource[ResourceType.WhiteMarker]));
                if (card.CardType != CardType.Wonder)
                {
                    var handInfo=board.CivilCards.FirstOrDefault(hinfo=> hinfo.Card == card);
                    board.CivilCards.Remove(handInfo);
                    response.Changes.Add(GameMove.PutBackCard((int)action.Data[1], card));
                }
                else
                {
                    board.ConstructingWonder = null;
                    board.ConstructingWonderSteps = 0;
                    response.Changes.Add(GameMove.PutBackWonder((int)action.Data[1], card));
                }

                //立刻移除所有flags
                board.EffectPool.DettachFlags(card.FlagEffects);

                return response;
            }
            else
            {
                //注意，特别的，当玩家执行其他action时，在这里将会设置CanPutback，但是仍然返回null，表示不拦截Action
                foreach (var cardRowCardInfo in Manager.CurrentGame.CardRow)
                {
                    cardRowCardInfo.CanPutBack = false;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回对应于卡牌列每一张卡的内政消耗，-1表示不可以拿
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public static List<int> CalcuateCardRowCost(GameLogicManager manager,int playerNo)
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
                        if (board.CivilCards.Any(hInfo=> hInfo.Card==info.Card))
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