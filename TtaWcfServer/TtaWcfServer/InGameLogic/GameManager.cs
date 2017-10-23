using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using HSYErpBase.Wcf;
using TtaCommonLibrary.Entities.GameModel;
using TtaPesistanceLayer.NHibernate.Entities.GamePesistance;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.ActionDefinition.Handlers;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.Effects;
using TtaWcfServer.InGameLogic.GameJournals;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic
{
    public partial class GameManager
    {
        protected GameManager()
        {
            _handlers = new List<ActionHandler>
            {
                new EndPhaseActionHandler(this),
                new TakeCardFromCardRowActionHandler(this),
            };
            LastCalcuatedActions=new Dictionary<int, List<PlayerAction>>();
        }

        #region 加载和序列化

        readonly XmlSerializer _serializer = new XmlSerializer(typeof (TtaGame));

        public bool LoadFromPesistance(GameRoom room, WcfContext context)
        {
            Civilopedia = TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);
            
            int relatedMatch = room.RelatedMatchId;
            if (relatedMatch <= 0)
            {
                return false;
            }

            var content = context.HibernateSession.Get<MatchTableContent>(relatedMatch);
            if (content == null)
            {
                return false;
            }

            StringReader sr = new StringReader(content.MatchData);
            CurrentGame = (TtaGame) _serializer.Deserialize(sr);

            sr.Close();


            //放入不能被序列化的东西
            //TODO CivilopediaCheck

            CurrentGame.Room = room;
            foreach (var board in CurrentBoards)
            {
                board.EffectPool = new EffectPool(board, Civilopedia);
            }

            //初始化其他成员
            LastCalcuatedActions=new Dictionary<int, List<PlayerAction>>();
            for (int index = 0; index < room.JoinedPlayer.Count; index++)
            {
                LastCalcuatedActions.Add(index, new List<PlayerAction>());
            }

            //完成
            return true;
        }

        public void SaveToPesistance(WcfContext context)
        {
            StringWriter sw = new StringWriter();

            _serializer.Serialize(sw, CurrentGame);
            String lob = sw.ToString();

            var content = CurrentGame.Id > 0
                ? context.HibernateSession.Get<MatchTableContent>(CurrentGame.Id)
                : new MatchTableContent {Id = CurrentGame.Id};

            content.MatchData = lob;

            if (CurrentGame.Id > 0)
            {
                context.HibernateSession.Update(content);
                CurrentGame.Room.RelatedMatchId = CurrentGame.Id;

                context.HibernateSession.Flush();
            }
            else
            {
                int id = (int) context.HibernateSession.Save(content);
                CurrentGame.Id = id;
                CurrentGame.Room.RelatedMatchId = id;
                context.HibernateSession.Update(CurrentGame.Room);

                context.HibernateSession.Flush();
            }
            sw.Close();
        }

        private void SetupNewGame(GameRoom room)
        {
            var pedia = TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);
            //目前无视其他的设置仅支持2人    
            var rule = pedia.GetRuleBook();
            //设置双方面板
            CurrentGame = rule.SetupNewGame(room);
            
            //未设置的内容包括玩家分配和游戏关联
            //分配玩家
            if (room.AdditionalRules.Contains(CommonRoomRule.RandomSeats))
            {
                room.JoinedPlayer = room.JoinedPlayer.Shuffle();
            }

            for (int index = 0; index < room.JoinedPlayer.Count; index++)
            {
                var player = room.JoinedPlayer[index];
                CurrentGame.Boards[index].PlayerId = player.Id;
            }

            //关联游戏
            CurrentGame.Room = room;

            //设置首动玩家
            CurrentGame.CurrentPhase = TtaPhase.FirstTurnActionPhase;
            CurrentGame.CurrentPlayer = 0;

            //初始化其他成员
            for (int index = 0; index < room.JoinedPlayer.Count; index++)
            {
                LastCalcuatedActions.Add(index, new List<PlayerAction>());
            }

            //完成了
        }

        #endregion

        private readonly List<ActionHandler> _handlers;

        public TtaGame CurrentGame { get; set; }


        private List<TtaBoard> CurrentBoards => CurrentGame.Boards;
        public TtaCivilopedia Civilopedia;

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
                actions.AddRange(handler.CheckAbleToPerform(playerNo));
            }
            LastCalcuatedActions[playerNo]=actions;
            return actions;
        }

        /// <summary>
        /// PlayerNo是座位号（0开始）
        /// </summary>
        /// <param name="playerNo">座位号（0开始）</param>
        /// <returns></returns>
        public ActionResponse TakeAction(int playerNo, PlayerAction action, Dictionary<int, object> data,ActionResponse clientResponse, WcfContext context)
        {
            ActionResponse response = null;
            
            if (!String.IsNullOrEmpty(action.Guid))
            {
                if (LastCalcuatedActions[playerNo] == null)
                {
                    return null;
                }
                action = LastCalcuatedActions[playerNo].FirstOrDefault(a => a.Guid == action.Guid);
                if (action == null)
                {
                    return null;
                }
            }

            //特别的，这里处理一下Reset的事情
            if (action.ActionType == PlayerActionType.ResetActionPhase)
            {
                LoadFromPesistance(CurrentGame.Room, context);
                response=new ActionResponse();
                response.Type = ActionResponseType.ForceRefresh;
                return response;
            }

            foreach (var actionHandler in _handlers)
            {
                response=actionHandler.PerfromAction(playerNo, action, data,context);
                if (response != null)
                {
                    break;
                }
            }

            if (response == null||response.Type== ActionResponseType.Accepted)
            {
                //没有handler接受时，目前暂定执行客户端结果
                //TODO：当每一个Action都有了验证以后，这里要修改当response==null时为报错
                if (clientResponse != null)
                {
                    ExecuteGameMove(playerNo, clientResponse.Changes);
                }
                
            }

            //TODO 在未来阻止该行为
            if (response == null)
            {
                response=new ActionResponse();
                response.Type= ActionResponseType.Accepted;
            }

            //一个小小的特殊处理，如果EndActionPhase被接受，那么就Save
            if (action.ActionType == PlayerActionType.EndActionPhase&&(
                response.Type== ActionResponseType.Accepted|| response.Type == ActionResponseType.ChangeList))
            {
                SaveToPesistance(context);
            }

            return response;
        }

        public void ExecuteGameMove(int playerNo,List<GameMove> moves)
        {
            var board = CurrentGame.Boards[playerNo];
            foreach (var gameMove in moves)
            {
                switch (gameMove.Type)
                {
                    case GameMoveType.Resource:
                        if (board.UncountableResourceCount.ContainsKey((ResourceType) gameMove.Data[0]))
                        {
                            board.UncountableResourceCount[(ResourceType) gameMove.Data[0]] = (int) gameMove.Data[2];
                        }
                        break;
                    case GameMoveType.TakeCard:
                        CurrentGame.CardRow[(int)gameMove.Data[0]].TakenBy = playerNo;
                        board.CivilCards.Add(Civilopedia.GetCardInfoById(((CardInfo)gameMove.Data[1]).InternalId));
                        break;
                    case GameMoveType.TakeWonder:
                        CurrentGame.CardRow[(int)gameMove.Data[0]].TakenBy = playerNo;
                        board.ConstructingWonder=Civilopedia.GetCardInfoById(((CardInfo)gameMove.Data[1]).InternalId);
                        board.ConstructingWonderSteps = 0;
                        break;
                    case GameMoveType.PutBackCard:
                        CurrentGame.CardRow[(int)gameMove.Data[0]].TakenBy = -1;
                        board.CivilCards.Remove((CardInfo)gameMove.Data[1]);
                        break;
                    case GameMoveType.PutBackWonder:
                        CurrentGame.CardRow[(int)gameMove.Data[0]].TakenBy = -1;
                        board.ConstructingWonder = null;
                        break;
                    case GameMoveType.Consumption:
                    case GameMoveType.Corruption:
                        var markers = WcfCastUtil.CastDictionary<CardInfo, int>(gameMove.Data[1]);
                        board.AggregateOnBuildingCell(0, (c, cell) =>
                        {
                            if (markers.ContainsKey(cell.Card))
                            {
                                cell.Storage += markers[cell.Card];
                            }

                            return 0;
                        });
                        break;
                    case GameMoveType.Production:
                        markers = WcfCastUtil.CastDictionary<CardInfo, int>(gameMove.Data[2]);
                        board.AggregateOnBuildingCell(0, (c, cell) =>
                        {
                            if (markers.ContainsKey(cell.Card))
                            {
                                cell.Storage += markers[cell.Card];
                            }

                            return 0;
                        });
                        break;
                }
            }
        }

        public ActionResponse ExecuteProduction(int playerNo)
        {
            //TODO 目前生产阶段的数据完全由服务器来确定
            //这里只做抽牌
            if (CurrentGame.CurrentPhase == TtaPhase.FirstTurnActionPhase)
            {
                CurrentGame.CurrentPhase = TtaPhase.FirstTurnProductionPhase;
            }
            else
            {
                CurrentGame.CurrentPhase = TtaPhase.ProductionPhase;
            }

            var board = CurrentBoards[playerNo];
            var response = new ActionResponse() { Type = ActionResponseType.Accepted };

            int amount = board.UncountableResourceCount[ResourceType.RedMarker] >= 3
                ? 3
                : board.UncountableResourceCount[ResourceType.RedMarker];
            //因为这时候，来自客户端的Change还没被执行，所以红点数目还没复原
            //因此就算其余部分依照客户端计算执行，这里也可以放心用红点数抽牌

            var cards =
                CurrentGame.MilitaryCardsDeck.DrawOrShuffle(
                    amount
                    , CurrentGame.DiscardedMilitaryCardsDeck);

            response.Changes.Add(new GameMove(GameMoveType.DrawCards, playerNo, amount, cards));

            return response;
        }

        /// <summary>
        /// 令游戏进入下一个玩家的回合（或者触发胜利）
        /// 注意，不含上一位玩家的生产和抽牌，仅布置卡牌列和调整CurrentPlayer，会导致时代更替。
        /// </summary>
        /// <param name="discard"></param>
        /// <param name="context"></param>
        public void NextTurn(bool discard,WcfContext context)
        {
            //移动卡牌列
            //弃牌发牌
            int slot= discard?Civilopedia.GetRuleBook().DiscardAmount(CurrentGame.Room):0;


            int index = 0;
            for (; index < CurrentGame.CardRow.Count;)
            {
                var cardRowInfo = CurrentGame.CardRow[slot];

                if (cardRowInfo.TakenBy == -1)
                {
                    CurrentGame.CardRow[index] = cardRowInfo;
                    index++;
                    slot++;
                }
                else
                {
                    slot++;
                }
                cardRowInfo.TakenBy = -1;

                if (slot >= CurrentGame.CardRow.Count)
                {
                    break;
                }

            }
            for (slot = index; slot < CurrentGame.CardRow.Count; slot++)
            {
                var card=CurrentGame.CivilCardsDeck.DrawCard();
                if (card == null)
                {
                    NextAge();
                    card= CurrentGame.CivilCardsDeck.DrawCard();
                }else if (CurrentGame.CivilCardsDeck.Count == 0)
                {
                    NextAge();
                }

                if (card == null)
                {
                    CurrentGame.CardRow[slot] = null;
                }
                else
                {
                    var cardRowInfo=new CardRowInfo();
                    cardRowInfo.Card = card;
                    cardRowInfo.TakenBy = -1;

                    CurrentGame.CardRow[slot] = cardRowInfo;
                }
            }

            //切换玩家
            if (CurrentGame.CurrentPlayer + 1 >= CurrentGame.Room.PlayerMax)
            {
                CurrentGame.CurrentPlayer = 0;
                CurrentGame.CurrentRound++;
            }
            else
            {
                CurrentGame.CurrentPlayer++;
            }

            CurrentGame.CurrentPhase= TtaPhase.PoliticalPhase;
            if (CurrentGame.CurrentRound == 2)
            {
                CurrentGame.CurrentPhase = TtaPhase.ActionPhase;
            }
            //生成政治行动
            //会由QueryBoard自动产生

            //持久化
            SaveToPesistance(context);
        }


        public void NextAge()
        {
            if (CurrentGame.CurrentAge == Age.IV)
            {
                return;
            }
            CurrentGame.CurrentAge = CurrentGame.CurrentAge + 1;
            //领袖死去

            //弃牌

            //更换牌堆
            CurrentGame.CivilCardsDeck.Clear();
            CurrentGame.CivilCardsDeck.AddRange(Civilopedia.GetRuleBook().GetCivilDeckForAge(CurrentGame.Room
                , CurrentGame.CurrentAge).Shuffle());
            CurrentGame.MilitaryCardsDeck.Clear();
            CurrentGame.MilitaryCardsDeck.AddRange(Civilopedia.GetRuleBook().GetMilitaryDeckForAge(CurrentGame.Room
                , CurrentGame.CurrentAge).Shuffle());

        }

        #region ResourceCount计算

        /// <summary>
        /// PlayerNo是座位号（0开始）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="playerNo"></param>
        /// <returns></returns>
        public int ReturnResourceCount(ResourceType type, int playerNo)
        {
            return ReturnResourceCount(type, CurrentBoards[playerNo]);
        }


        private int ReturnResourceCount(ResourceType type, TtaBoard board)
        {
            int resourceValue = 0;
            switch (type)
            {
                case ResourceType.WhiteMarkerMax:
                    resourceValue += board.EffectPool.FilterEffect(CardEffectType.E100, 12).Sum(e => e.Data[1]);
                    break;
                case ResourceType.RedMarkerMax:
                    resourceValue += board.EffectPool.FilterEffect(CardEffectType.E100, 13).Sum(e => e.Data[1]);
                    break;
                case ResourceType.Food:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.Food, board,
                        (current, effect, cell) => current + effect*cell.Storage);
                    break;
                case ResourceType.FoodIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.Food, board,
                        (current, effect, cell) => current + effect*cell.Worker);
                    break;
                case ResourceType.Resource:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.Resource, board,
                        (current, effect, cell) => current + effect*cell.Storage);
                    break;
                case ResourceType.ResourceIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.Resource, board,
                        (current, effect, cell) => current + effect*cell.Worker);
                    break;
                case ResourceType.HappyFace:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.HappyFace, board,
                        (current, effect, cell) => current + effect*cell.Worker);
                    break;
                case ResourceType.ScienceIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.Science, board,
                        (current, effect, cell) => current + effect*cell.Worker);
                    break;
                case ResourceType.YellowMarker:
                    var workerUsed = board.AggregateOnBuildingCell(0, (current, cell) => current + cell.Worker);
                    workerUsed += board.UncountableResourceCount[ResourceType.WorkerPool];
                    resourceValue = board.InitialYellowMarkerCount - workerUsed;

                    break;
                case ResourceType.BlueMarker:
                    var blueMarkerUsed = board.AggregateOnBuildingCell(0, (current, cell) => current + cell.Storage);
                    blueMarkerUsed += board.ConstructingWonderSteps;

                    resourceValue = board.InitialBlueMarkerCount - blueMarkerUsed;
                    break;
                case ResourceType.Science:
                case ResourceType.Culture:
                case ResourceType.ResourceForMilitary:
                case ResourceType.ResourceForWonderAndProduction:
                case ResourceType.ScienceForMilitary:
                case ResourceType.ScienceForSpecialTech:
                case ResourceType.WhiteMarker:
                case ResourceType.RedMarker:
                case ResourceType.WorkerPool:
                    resourceValue = board.UncountableResourceCount[type];
                    break;
            }

            return resourceValue;

        }



        private int AggregateCountResourceOnBuildingCell(
            ResourceType type, TtaBoard board, Func<int, int, BuildingCell, int> aggregate)
        {
            int count = 0;
            foreach (var buildingPair in board.Buildings)
            {
                foreach (var cellPair in buildingPair.Value)
                {
                    var cell = cellPair.Value;

                    var e =
                        cell.Card.CivilpediaCheck(Civilopedia)
                            .SustainedEffects.FilterEffect(CardEffectType.E100, (int) type)
                            .FirstOrDefault();
                    if (e != null)
                    {
                        int value = e.Data[1];
                        count = aggregate(count, value, cell);
                    }
                }
            }

            return count;
        }

        #endregion
    }
}