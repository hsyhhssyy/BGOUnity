using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Actions;
using Assets.CSharpCode.GameLogic.Actions.Handlers;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.GameLogic.GameEvents;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI;

namespace Assets.CSharpCode.GameLogic
{
    public class GameLogicManager
    {
        public static GameLogicManager CurrentManager = new GameLogicManager();

        protected GameLogicManager()
        {
            _handlers = new List<ActionHandler>
            {
                new EndPhaseActionHandler(this),
                new TakeCardFromCardRowActionHandler(this),
            };
            LastCalcuatedActions = new Dictionary<int, List<PlayerAction>>();

            _channel.GameEvent += OnSubscribedGameEvents;
        }


        private GameEventChannel _channel = SceneTransporter.CurrentChannel;

        public TtaGame CurrentGame { get { return SceneTransporter.CurrentGame; } }
        
        public TtaCivilopedia Civilopedia { get { return SceneTransporter.CurrentCivilopedia; } }

        private readonly List<ActionHandler> _handlers;

        public Dictionary<int, List<PlayerAction>> LastCalcuatedActions;

        /// <summary>
        /// PlayerNo是座位号（0开始）
        /// </summary>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public List<PlayerAction> GetPossibleActions(int playerNo)
        {
            var actions = new List<PlayerAction>();
            foreach (var handler in _handlers)
            {
                var returnActions = handler.CheckAbleToPerform(playerNo);
                if (returnActions != null)
                {
                    actions.AddRange(returnActions);
                }
            }
            LastCalcuatedActions[playerNo] = actions;
            return actions;
        }

        #region 消息接收与初始化
        
        private void OnSubscribedGameEvents(object sender, GameUIEventArgs args)
        {
            if (args.EventType == GameUIEventType.Refresh)
            {
                ReSyncStatus();
            }
        }

        public void ReSyncStatus()
        {
            foreach (var board in CurrentGame.Boards)
            {
                board.EffectPool.ReSyncStatus();
            }
        }

        #endregion


        public List<ActionResponse> SubmittedActionResponses = new List<ActionResponse>();
        public List<ActionResponse> DiscardedActionResponses = new List<ActionResponse>();

        /// <summary>
        /// 尝试在本地执行一个action并发送刷新消息，如果不能处理，则返回null，否则返回处理结果
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionResponse TakeAction(PlayerAction action)
        {
            if (CurrentGame == null)
            {
                return null;
            }

            foreach (var actionHandler in _handlers)
            {
                var response = actionHandler.PerfromAction(SceneTransporter.CurrentGame.MyPlayerIndex,action,action.Data);
                if (response != null)
                {
                    //action被处理掉了
                    //进行事件触发
                    foreach (var change in response.Changes)
                    {
                        TriggerChangeEvent(response,change);
                    }

                    //处理动画
                    _channel.Broadcast(new ServerGameUIEventArgs(GameUIEventType.Refresh, "GameLogicManager"));
                    
                    //重新计算action
                    var actions=GetPossibleActions(SceneTransporter.CurrentGame.MyPlayerIndex);
                    CurrentGame.PossibleActions.Clear();
                    CurrentGame.PossibleActions.AddRange(actions);

                    //不需要在这里通知界面刷新，因为TakeAction的callback会判断是否需要刷新
                    SubmittedActionResponses.Add(response);
                    return response;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据产生的Change来触发游戏事件。本函数被TakeAction函数调用
        /// </summary>
        /// <param name="response"></param>
        /// <param name="change"></param>
        private void TriggerChangeEvent(ActionResponse response, GameMove change)
        {
            var msg = new LogicGameUIEventArgs(GameUIEventType.LogicGameChanged, "GameLogicManager");
            msg.AttachedData.Add("Response", response);
            msg.AttachedData.Add("Change", change);
            _channel.Broadcast(msg);
        }

        #region 处理延迟到达的服务器响应

        /// <summary>
        /// 处理服务器返回的真实回调，如果需要，则生成刷新消息并丢弃中间的ActionResponse
        /// </summary>
        /// <param name="gameLogicResponse"></param>
        /// <param name="serverResponse"></param>
        /// <returns>指示协程是否需要阻止原始回调函数获取该结果，默认为true表示阻止</returns>
        public bool ProcessServerCallback(ActionResponse gameLogicResponse,ActionResponse serverResponse)
        {
            //已被丢弃的操作返回了
            if (DiscardedActionResponses.Contains(gameLogicResponse))
            {
                return true;
            }
            //直接比较response的区别就行
            if (serverResponse.Type== ActionResponseType.ForceRefresh|| serverResponse.Type== ActionResponseType.InvalidAction)
            {
                //丢弃所有改动
                DiscardedActionResponses.AddRange(SubmittedActionResponses);
                SubmittedActionResponses.Clear();

                //需要通知界面刷新
                _channel.Broadcast(new ControllerGameUIEventArgs(GameUIEventType.ForceRefresh, "GameLogicManager"));
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 执行一个不包含抽牌的生产阶段
        /// </summary>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public ActionResponse ExecuteProduction(int playerNo)
        {

            if (CurrentGame.CurrentPhase == TtaPhase.FirstTurnActionPhase)
            {
                CurrentGame.CurrentPhase = TtaPhase.FirstTurnProductionPhase;
            }
            else
            {
                CurrentGame.CurrentPhase = TtaPhase.ProductionPhase;
            }

            //执行一个不包含抽牌的生产阶段
            ActionResponse response = new ActionResponse();
            response.Type = ActionResponseType.ChangeList;

            var board = CurrentGame.Boards[playerNo];
            

            //生产分数和科技
            var origin = board.Resource[ResourceType.Culture];
            board.Resource[ResourceType.Culture] += board.Resource[ResourceType.CultureIncrement];
            if (board.Resource[ResourceType.Culture] < 0)
            {
                board.Resource[ResourceType.Culture] = 0;
            }
            response.Changes.Add(GameMove.Resource(ResourceType.Culture, origin, board.Resource[ResourceType.Culture]));

            origin = board.Resource[ResourceType.Science];
            board.Resource[ResourceType.Science] += board.Resource[ResourceType.ScienceIncrement];
            if (board.Resource[ResourceType.Science] < 0)
            {
                board.Resource[ResourceType.Science] = 0;
            }
            response.Changes.Add(GameMove.Resource(ResourceType.Science, origin, board.Resource[ResourceType.Science]));

            //计算腐败
            int currentBlueMarker = board.Resource[ResourceType.BlueMarker];
            int corruptionValue = Civilopedia.GetRuleBook().CorruptionValue(currentBlueMarker);

            if (corruptionValue > 0)
            {
                Dictionary<CardInfo, int> corruptionMarkers = new Dictionary<CardInfo, int>();
                int left = SpendResource(playerNo, BuildingType.Mine, ResourceType.ResourceIncrement, corruptionValue,
                    corruptionMarkers);
                if (left > 0)
                {
                    left = SpendResource(playerNo, BuildingType.Farm, ResourceType.FoodIncrement, left,
                        corruptionMarkers);
                    if (left > 0)
                    {
                        //扣分
                        origin = board.Resource[ResourceType.Culture];
                        board.Resource[ResourceType.Culture] -= left*4;
                        if (board.Resource[ResourceType.Culture] < 0)
                        {
                            board.Resource[ResourceType.Culture] = 0;
                        }
                        response.Changes.Add(GameMove.Resource(ResourceType.Culture, origin,
                            board.Resource[ResourceType.Culture]));
                    }

                }
                PerformMarkerChange(playerNo, corruptionMarkers);
                response.Changes.Add(GameMove.Corruption(corruptionValue, corruptionMarkers));
            }


            //生产粮食
            Dictionary<CardInfo, int> markers = new Dictionary<CardInfo, int>();
            int amount = board.Resource[ResourceType.FoodIncrement];
            ProduceResource(playerNo, BuildingType.Farm, ResourceType.FoodIncrement, amount, markers);
            PerformMarkerChange(playerNo, markers);

            response.Changes.Add(GameMove.Production(ResourceType.Food, amount, markers));

            //消耗粮食
            int currentYellowMarker = board.Resource[ResourceType.YellowMarker];
            int consumptionValue = Civilopedia.GetRuleBook().ConsumptionValue(currentYellowMarker);

            markers.Clear();
            int spend = SpendResource(playerNo, BuildingType.Farm, ResourceType.FoodIncrement, consumptionValue, markers);
            if (spend > 0)
            {
                //扣分
                origin = board.Resource[ResourceType.Culture];
                board.Resource[ResourceType.Culture] -= spend*4;
                if (board.Resource[ResourceType.Culture] < 0)
                {
                    board.Resource[ResourceType.Culture] = 0;
                }
                response.Changes.Add(GameMove.Resource(ResourceType.Culture, origin,
                    board.Resource[ResourceType.Culture]));
            }
            PerformMarkerChange(playerNo, markers);
            response.Changes.Add(GameMove.Consumption(amount, markers));

            //生产矿物
            markers.Clear();
            amount = board.Resource[ResourceType.ResourceIncrement];
            ProduceResource(playerNo, BuildingType.Mine, ResourceType.ResourceIncrement, amount, markers);
            PerformMarkerChange(playerNo, markers);

            response.Changes.Add(GameMove.Production(ResourceType.Resource, amount, markers));
            
            //抽牌，这个客户端做不了，但是要通知服务器，要抽多少张
            response.Changes.Add(
                GameMove.DrawCards(board.Resource[ResourceType.RedMarker] >= 3
                    ? 3
                    : board.Resource[ResourceType.RedMarker]));

            //复原红白点
            board.Resource[ResourceType.RedMarker] = board.Resource[ResourceType.RedMarkerMax];
            board.Resource[ResourceType.WhiteMarker] = board.Resource[ResourceType.WhiteMarkerMax];
            response.Changes.Add(GameMove.Resource(ResourceType.RedMarker, origin, board.Resource[ResourceType.RedMarkerMax]));
            response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, origin, board.Resource[ResourceType.WhiteMarkerMax]));

            return response;
        }

        /// <summary>
        /// 返回没能消耗掉的资源数量
        /// </summary>
        /// <param name="playerNo"></param>
        /// <param name="value"></param>
        /// <param name="marker"></param>
        /// <returns></returns>
        public int SpendResource(int playerNo, BuildingType buildingType, ResourceType rType, int value,Dictionary<CardInfo, int> marker)
        {
            //从小往大拿
            var board = CurrentGame.Boards[playerNo];
            var buildings = board.Buildings[buildingType];

            int currentAmount = value;
            for (int i = 0; i < 4; i++)
            {
                Age age = (Age) i;
                if (buildings.ContainsKey(age))
                {
                    var cell = buildings[age];
                    if (!marker.ContainsKey(cell.Card))
                    {
                        marker.Add(cell.Card, 0);
                    }

                    var effect=
                        cell.Card.SustainedEffects.FirstOrDefault(e => e.FunctionId == CardEffectType.E100&&e.Data[0]==(int)rType);
                    if (effect == null)
                    {
                        continue;
                    }
                    var cellPrice = effect.Data[1];
                    int need = (int) Math.Round(1d*currentAmount/cellPrice);
                    if (need > cell.Worker)
                    {
                        marker[cell.Card]+= 0 - cell.Storage;
                        currentAmount -= cell.Storage * cellPrice;
                    }
                    else
                    {
                        marker[cell.Card] += 0 - need;
                        currentAmount -= need * cellPrice;
                    }
                    if (currentAmount == 0)
                    {
                        break;
                    }
                    if (currentAmount < 0)
                    {
                        ProduceResource(playerNo, buildingType, rType,0 - currentAmount, marker);
                        break;
                    }
                }
            }

            return currentAmount;
        }

        /// <summary>
        /// 返回没能消耗掉的资源数量
        /// </summary>
        /// <param name="playerNo"></param>
        /// <param name="value"></param>
        /// <param name="marker"></param>
        /// <returns></returns>
        public void ProduceResource(int playerNo, BuildingType buildingType,ResourceType rType, int value, Dictionary<CardInfo, int> marker)
        {
            //从大往小放
            var board = CurrentGame.Boards[playerNo];
            var buildings = board.Buildings[buildingType];

            int currentAmount = value;
            for (int i = 4; i >=0; i--)
            {
                Age age = (Age)i;
                if (buildings.ContainsKey(age))
                {
                    var cell = buildings[age];
                    if (!marker.ContainsKey(cell.Card))
                    {
                        marker.Add(cell.Card, 0);
                    }

                    var effect =
                        cell.Card.SustainedEffects.FirstOrDefault(e => e.FunctionId == CardEffectType.E100 && e.Data[0] == (int)rType);
                    if (effect == null)
                    {
                        continue;
                    }
                    var cellPrice = effect.Data[1];

                    if (cellPrice < currentAmount)
                    {
                        int need = currentAmount / cellPrice;
                        marker[cell.Card] += need;
                        currentAmount -= need * cellPrice;
                    }
                    if (currentAmount == 0)
                    {
                        break;
                    }
                }
            }

            return;
        }

        public void PerformMarkerChange(int playerNo, Dictionary<CardInfo, int> markers)
        {
            var board = CurrentGame.Boards[playerNo];
            int currentBlueMarker = board.Resource[ResourceType.BlueMarker];
            int total = 0;
            foreach (var marker in markers)
            {
                total += marker.Value;
            }
            if (total > currentBlueMarker)
            {
                int distance = total - currentBlueMarker;
                var lest = markers.Keys.ToList();
                lest.Sort((a, b) => a.CardAge.CompareTo(b.CardAge));
                foreach (var cardInfo in lest)
                {
                    if (markers[cardInfo] > 0)
                    {
                        if (distance > markers[cardInfo])
                        {
                            distance = distance - markers[cardInfo];
                            markers[cardInfo] = 0;
                        }
                        else
                        {
                            markers[cardInfo]-= distance;
                            break;
                        }
                    }
                }
            }

            board.AggregateOnBuildingCell(0, (c, cell) =>
            {
                if (markers.ContainsKey(cell.Card))
                {
                    cell.Storage += markers[cell.Card];
                }

                return 0;
            });
        }
    }
}
