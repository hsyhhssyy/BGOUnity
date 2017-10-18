using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.GameJournals
{
    /// <summary>
    /// 表示游戏面板的一次变动，是GameJournal的更细分对象，GameJournal理论上就应由数个BoardChange组成.
    /// (实际上ActionResponse也应该由数个GameBoardChange组成）
    /// </summary>
    [DataContract]
    [KnownType(typeof(CardInfo))]
    [KnownType(typeof(CardRowInfo))]
    [KnownType(typeof(Dictionary<CardInfo,int>))]
    public class GameMove
    {
        [DataMember]
        public GameMoveType Type;
        [DataMember]
        public Dictionary<int, Object> Data;

        public GameMove(GameMoveType type, params Object[] data)
        {
            Data = new Dictionary<int, object>();
            Type = type;
            for (int i = 0; i < data.Length; i++)
            {
                Data.Add(i, data[i]);
            }
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
        /// 表示发生了总计[0]点的腐败，从Dictionary[1]（key：卡牌）上扣除了Value个蓝点
        /// </summary>
        Corruption,
        /// <summary>
        /// 表示发生了总计[0]点消耗，，从Dictionary[1]（key：卡牌）上扣除了Value个蓝点
        /// </summary>
        Consumption,
    }
}