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
        public int PlayerNo=-1;
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

        public static GameMove Upgrade(BuildingType type,CardInfo from, CardInfo to, int count)
        {
            return new GameMove(GameMoveType.Upgrade, type, from, to, count);
        }
        public static GameMove Build(CardInfo card, int from, int to)
        {
            return new GameMove(GameMoveType.Build, card, from, to);
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
        public static GameMove CardUsed(HandCardInfo info)
        {
            return new GameMove(GameMoveType.CardUsed, info);
        }
        public static GameMove ReplaceSpecialTech(CardInfo oldInfo,CardInfo newInfo)
        {
            return new GameMove(GameMoveType.ReplaceSpecialTech, oldInfo,newInfo);
        }
        public static GameMove DevelopSpecialTech(CardInfo info)
        {
            return new GameMove(GameMoveType.DevelopSpecialTech, info);
        }
        public static GameMove DevelopBuilding(CardInfo info)
        {
            return new GameMove(GameMoveType.DevelopBuilding, info);
        }
        public static GameMove ElectLeader(CardInfo info)
        {
            return new GameMove(GameMoveType.ElectLeader, info);
        }
        public static GameMove ReplaceGovernment(CardInfo info)
        {
            return new GameMove(GameMoveType.ReplaceGovernment, info);
        }
        public static GameMove ConstructWonder(int step)
        {
            return new GameMove(GameMoveType.ConstructWonder, step);
        }
        public static GameMove WonderComplete(CardInfo info)
        {
            return new GameMove(GameMoveType.WonderComplete, info);
        }
        public static GameMove SetupTactic(CardInfo info)
        {
            return new GameMove(GameMoveType.SetupTactic, info);
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
        /// <summary>
        /// 表示建筑物/部队[0:CardInfo]上的黄点从[1]变到[2]（变少表示拆除）
        /// </summary>
        Build,
        /// <summary>
        /// 表示手牌[0]被用掉了。注意这里的的[0]是HandCardInfo而不是CardInfo。
        /// 从而能在多张同名牌里分出是哪一张。
        /// </summary>
        CardUsed,
        /// <summary>
        /// 表示用牌[0]替换掉当前的政体
        /// </summary>
        ReplaceGovernment,
        /// <summary>
        /// 表示用牌[0]替换掉当前的特殊科技[1]
        /// </summary>
        ReplaceSpecialTech,
        /// <summary>
        /// 表示将牌[0]加入玩家的特殊科技
        /// </summary>
        DevelopSpecialTech,
        /// <summary>
        /// 表示玩家激活科技牌[0]代表的建筑或者部队
        /// </summary>
        DevelopBuilding,
        /// <summary>
        /// 表示玩家将领袖牌[0]作为自己的领袖（不包括手牌变化）
        /// </summary>
        ElectLeader,
        /// <summary>
        /// 表示玩家建造了当前奇迹的[0]步（资源和白点消耗另行通知）
        /// </summary>
        ConstructWonder,
        /// <summary>
        /// 表示玩家当前建造的奇迹[0]已经完成
        /// </summary>
        WonderComplete,
        /// <summary>
        /// 表示建筑物/部队[0:BuildingType]上的黄点从[1:CardInfo]移动到[2:CardInfo]，一共移动[3]个
        /// </summary>
        Upgrade,
        /// <summary>
        /// 表示玩家设置阵型[0]为其当前阵型（不考虑消耗和阵型的来源）
        /// </summary>
        SetupTactic,
    }
}
