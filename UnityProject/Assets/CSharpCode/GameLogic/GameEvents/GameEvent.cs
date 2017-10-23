using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.GameEvents
{
    /// <summary>
    /// 表示游戏面板的一次变动，是GameJournal的更细分对象，GameJournal理论上就应由数个BoardChange组成.
    /// (实际上ActionResponse也应该由数个GameBoardChange组成）
    /// </summary>
    [DataContract]
    public class GameMove
    {
        [DataMember]
        public GameMoveType Type;
        /// <summary>
        /// 表示执行该操作的玩家编号
        /// </summary>
        [DataMember]
        public int PlayerNo;
        [DataMember]
        public Dictionary<int, Object> Data;

        private GameMove()
        {
            //Used for Serialization
        }

        private GameMove(GameMoveType type, Dictionary<int, Object> data)
        {
            Type = type;
            Data = data;
        }
        private GameMove(GameMoveType type, params Object[] data)
        {
            Data=new Dictionary<int, object>();
            Type = type;
            for (int i = 0; i < data.Length; i++)
            {
                Data.Add(i,data[i]);
            }
        }

        public static GameMove Resource(ResourceType type, int from, int to)
        {
            return new GameMove(GameMoveType.Resource, type, from, to);
        }
        public static GameMove TakeCard(int position, CardInfo card)
        {
            return new GameMove(GameMoveType.TakeCard, position, card);
        }
        public static GameMove PutBackCard(int position, CardInfo card)
        {
            return new GameMove(GameMoveType.PutBackCard, position, card);
        }
        public static GameMove TakeWonder(int position, CardInfo card)
        {
            return new GameMove(GameMoveType.TakeWonder, position, card);
        }
        public static GameMove PutBackWonder(int position, CardInfo card)
        {
            return new GameMove(GameMoveType.PutBackWonder, position, card);
        }
        public static GameMove DrawCards(int amount)
        {
            return new GameMove(GameMoveType.DrawCards, amount, new List<CardInfo>());
        }
        public static GameMove Production(ResourceType type,int amount, Dictionary<CardInfo, int> markers)
        {
            var copyMarkers = new Dictionary<CardInfo, int>();
            foreach (var marker in markers)
            {
                copyMarkers.Add(marker.Key,marker.Value);
            }
            return new GameMove(GameMoveType.Production, type, amount, copyMarkers);
        }
        public static GameMove Corruption(int amount, Dictionary<CardInfo, int> markers)
        {
            var copyMarkers = new Dictionary<CardInfo, int>();
            foreach (var marker in markers)
            {
                copyMarkers.Add(marker.Key, marker.Value);
            }
            return new GameMove(GameMoveType.Corruption, amount, copyMarkers);
        }
        public static GameMove Consumption(int amount, Dictionary<CardInfo, int> markers)
        {
            var copyMarkers = new Dictionary<CardInfo, int>();
            foreach (var marker in markers)
            {
                copyMarkers.Add(marker.Key, marker.Value);
            }
            return new GameMove(GameMoveType.Consumption, amount, copyMarkers);
        }
    }

    public enum GameMoveType
    {
        /// <summary>
        /// 表示将资源[0]由[1]变为[2]
        /// </summary>
        Resource,
        /// <summary>
        /// 表示将卡牌列[0]位置的牌Card[1]置入手牌
        /// </summary>
        TakeCard,
        /// <summary>
        /// 表示将卡牌列[0]位置的牌Card[1]置于待建设奇迹区
        /// </summary>
        TakeWonder,
        /// <summary>
        /// 表示将卡牌列[0]位置的牌Card[1]置入手牌
        /// </summary>
        PutBackCard,
        /// <summary>
        /// 表示将卡牌列[0]位置的牌Card[1]从待建设奇迹区退回
        /// </summary>
        PutBackWonder,
        /// <summary>
        /// 表示抽牌[0]张，他们包含在List[1]
        /// </summary>
        DrawCards,
        /// <summary>
        /// 表示弃牌[0]张，他们包含在List[1]
        /// </summary>
        DiscardCards,
        /// <summary>
        /// 表示资源[0]生产了[1]，从Dictionary[2]（key：卡牌）上变化了Value个蓝点
        /// </summary>
        Production,
        /// <summary>
        /// 表示发生了总计[0]点的腐败，从Dictionary[1]（key：卡牌）上变化了Value个蓝点
        /// </summary>
        Corruption,
        /// <summary>
        /// 表示发生了总计[0]点消耗，，从Dictionary[1]（key：卡牌）上变化了Value个蓝点
        /// </summary>
        Consumption,
    }
}
