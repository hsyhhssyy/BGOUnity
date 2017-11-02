using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Actions.Handlers.ActionPhaseHandler;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Effect
{
    public static class EffectExecutor
    {
        /// <summary>
        /// 为某位玩家执行一个特定效果（可能会受到其他效果的影响而产生不同效果）
        /// InternalAction不是真正的Action，他只是逻辑上的Action，不一定被所有服务器支持，因此其相关内容并不会出现在Executor中
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <param name="effect"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static List<GameMove> ExecuteEffect(GameLogicManager manager,int playerNo,CardEffect effect,PlayerAction action)
        {
            var currentGame = manager.CurrentGame;
            var board = manager.CurrentGame.Boards[playerNo];

            var result = new List<GameMove>();

            switch (effect.FunctionId)
            {
                //改变[0]类型的属性[1]点
                case CardEffectType.E100:
                {
                    var resourceType = (ResourceType)effect.Data[0];
                    var value = (int)effect.Data[1];
                    var oldValue = board.Resource[resourceType];
                    board.Resource[resourceType] = oldValue+value;
                    result.Add(GameMove.Resource(resourceType, oldValue,oldValue+value));
                        break;
                }
                //生产闲置工人后返还粮食[0]
                case CardEffectType.E401:
                {
                    result.AddRange(IncreasePopulationActionHandler.IncreasePopulation(playerNo));

                    var oldValue = board.Resource[ResourceType.Food];
                    board.Resource[ResourceType.Food] = oldValue + effect.Data[0];
                    result.Add(GameMove.Resource(ResourceType.Food, oldValue, oldValue + effect.Data[0]));
                    break;
                }
                //建造一层奇迹减少资源点消耗[0]
                case CardEffectType.E407:
                {
                    var cost = BuildWonderActionHandler.ResourceCost(1, board) - effect.Data[0];
                    Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
                    manager.SimSpendResource(playerNo, BuildingType.Mine, ResourceType.ResourceIncrement, cost, markers);
                    result.Add(GameMove.Production(ResourceType.Resource, 0 - cost, markers));
                    manager.PerformMarkerChange(playerNo, markers);

                    result.AddRange(BuildWonderActionHandler.IncreaseConstructionStep(1, board));
                    break;
                }
                //根据军力高于自己的其他玩家的数量*[1]，改变[0]类型的属性
                case CardEffectType.E232:
                {
                    var myStrength = board.Resource[ResourceType.MilitaryForce];
                    int stronger = 0;
                    foreach (var otherBoard in currentGame.Boards)
                    {
                            if (otherBoard == board) { continue;}
                        if (otherBoard.Resource[ResourceType.MilitaryForce] > myStrength)
                        {
                            stronger++;
                        }
                    }
                    var resourceType = (ResourceType)effect.Data[0];
                    var value = stronger*(int)effect.Data[1];
                    var oldValue = board.Resource[resourceType];
                    board.Resource[resourceType] = oldValue + value;
                    result.Add(GameMove.Resource(resourceType, oldValue, oldValue + value));
                    break;
                }//根据文化高于自己的其他玩家的数量*[1]，改变[0]类型的属性
                case CardEffectType.E233:
                {
                    var myStrength = board.Resource[ResourceType.Culture];
                    int stronger = 0;
                    foreach (var otherBoard in currentGame.Boards)
                    {
                        if (otherBoard == board) { continue; }
                        if (otherBoard.Resource[ResourceType.Culture] > myStrength)
                        {
                            stronger++;
                        }
                    }
                    var resourceType = (ResourceType)effect.Data[0];
                    var value = stronger * (int)effect.Data[1];
                    var oldValue = board.Resource[resourceType];
                    board.Resource[resourceType] = oldValue + value;
                    result.Add(GameMove.Resource(resourceType, oldValue, oldValue + value));
                    break;
                }
            }
            return result;
        }
        
        /// <summary>
        /// 检查一个立即效果是否能被执行，这主要用于执行对ActionCard，主动激活的的领袖和奇迹是否合法的检查。
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        public static bool CheckEffect(GameLogicManager manager, int playerNo, CardEffect effect)
        {
            var currentGame = manager.CurrentGame;
            var board = manager.CurrentGame.Boards[playerNo];

            switch (effect.FunctionId)
            {
                case CardEffectType.E401:
                {
                    var resource = board.Resource[ResourceType.Food];
                    int cost = IncreasePopulationActionHandler.CalcuatedFoodCost(playerNo);
                    if (resource < cost)
                    {
                        return false;
                    }
                    break;
                }
                case CardEffectType.E407:
                {
                    var resource = board.Resource[ResourceType.Resource];
                    var cost= BuildWonderActionHandler.ResourceCost(1, board) - effect.Data[0];
                    if (resource < cost)
                    {
                        return false;
                    }
                    break;
                }
            }
            return true;
        }
    }
}
