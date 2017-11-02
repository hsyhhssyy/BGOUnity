using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Assets.CSharpCode.Entity
{
    public enum Age
    {
        A=0,I=1,II=2,III=3,IV=4
    }

    public enum ResourceType
    {
        Food=1, Resource=2, Science=3, Culture=4,
        FoodIncrement=5, ResourceIncrement=6, ScienceIncrement=7, CultureIncrement=8,
        HappyFace=9, MilitaryForce=10, Exploration=11,
        WhiteMarkerMax=12, RedMarkerMax=13,
        WhiteMarker=14, RedMarker=15,
        YellowMarker=16, BlueMarker=17,
        WorkerPool=18,
        CivilCardLimit=19,MilitaryCardLimit = 20,
        UrbanLimit=21,
        MilitaryDrawLimit=22,
        ResourceForWonderAndProduction=23,
        ResourceForMilitary=24,
        ExtraRedMarker=25,
        ScienceForMilitary =26,
        ScienceForSpecialTech=27,
    }

    public enum BuildingType
    {
        Farm, Mine, Arena, Lab, Library, Temple, Theater, AirForce, Artillery, Cavalry, Infantry,Unknown
    }

    public enum WarningType
    {
        WarOverTerritory=1,
        WarOverCulture=2,
        WarOverTechnology=3,
        Famine,
        LastTurn,
        Corruption,
        CivilDisorder,
        CivilActionRemain,
    }

    public enum TtaPhase
    {
        PoliticalPhase,
        ActionPhase,
        DiscardPhase,
        OtherPhase,
        EventResolution,
        Colonize,
        SendColonists,
        /// <summary>
        /// 游戏第一回合的行动阶段
        /// </summary>
        FirstTurnActionPhase,
        /// <summary>
        /// 游戏第一回合的生产阶段
        /// </summary>
        FirstTurnProductionPhase,
        /// <summary>
        /// 生产阶段
        /// </summary>
        ProductionPhase,
    }

    public enum CardType
    {
        Unknown = 0,
        Action =1, Aggression=2, Colony=3, Event=4, Government=5, Leader=6,
        MilitaryTechAirForce=7, MilitaryTechArtillery=8, MilitaryTechCavalry=9, MilitaryTechInfantry=10,
        Pact=11,
        ResourceTechFarm=12,ResourceTechMine=13,
        SpecialTechCivil=14, SpecialTechEngineering=15, SpecialTechExploration=16, SpecialTechMilitary=17,
        Tactic=18,
        UrbanTechArena=19, UrbanTechLab=20, UrbanTechLibrary=21, UrbanTechTemple=22, UrbanTechTheater=23,
        War=24,
        Wonder=25,
        Defend=26,
    }

    public enum PlayerActionType
    {
        Unknown, ProgramDelegateAction,

        #region 常见内政行动
        /// <summary>
        /// 玩家可以从卡牌列上拿一张卡
        /// 0. CardRowInfo
        /// 1. 此卡在卡牌列上的位置，0开始
        /// </summary>
        TakeCardFromCardRow,
        /// <summary>
        /// 玩家可以从卡牌列上放回一张卡
        /// 0. CardRowInfo
        /// 1. 此卡在卡牌列上的位置，0开始
        /// </summary>
        PutBackCard,
        /// <summary>
        /// 玩家可以增加一个人口
        /// 0. 需要的Food
        /// </summary>
        IncreasePopulation,
        /// <summary>
        /// 玩家可以建造一个建筑物（包括士兵）
        /// 0. CardInfo
        /// 1. Price
        /// 2. 相关CardInfo（例如城市发展ActionCard）
        /// </summary>
        BuildBuilding,
        /// <summary>
        /// 玩家可以升级一个建筑物
        /// 0. CardInfo (from)
        /// 1. CardInfo (to)
        /// 2. Price
        /// 3. 相关CardInfo（例如城市发展ActionCard）
        /// </summary>
        UpgradeBuilding,
        /// <summary>
        /// 解除一个士兵（用红点）
        /// 0. CardInfo
        /// </summary>
        Disband,
        /// <summary>
        /// 摧毁一座建筑物
        /// 0. CardInfo
        /// </summary>
        Destory,
        /// <summary>
        /// 重置内政行动，如果政治行动未做任何事，则回到政治行动选择处。
        /// 无数据
        /// </summary>
        ResetActionPhase,
        //---------打出一张内政卡---------
        /// <summary>
        /// 0. CardInfo
        /// 1. 相关CardInfo（例如突破ActionCard）
        /// </summary>
        DevelopTechCard,
        /// <summary>
        /// 0. CardInfo
        /// </summary>
        PlayActionCard,
        /// <summary>
        /// 0. CardInfo
        /// </summary>
        ElectLeader,
        /// <summary>
        /// 0. CardInfo
        /// </summary>
        Revolution,
        /// <summary>
        /// 建造奇迹
        /// 0. CardInfo
        /// 1. 这个行动建造的步数
        /// 2. 总的资源消耗
        /// 3. 总的白点消耗
        /// </summary>
        BuildWonder,
        //--------掉红点的行动------
        /// <summary>
        /// 设置一个阵型
        /// 0. CardInfo
        /// </summary>
        SetupTactic,
        /// <summary>
        /// 设置一个阵型
        /// 0. CardInfo
        /// </summary>
        AdoptTactic,
        /// <summary>
        /// 结束内政行动
        /// </summary>
        EndActionPhase,
        #endregion

        #region 常见政治行动
        /// <summary>
        /// 打出事件牌
        /// 0.CardInfo
        /// </summary>
        PlayEvent,
        /// <summary>
        /// 打出殖民地
        /// 0.CardInfo
        /// </summary>
        PlayColony,
        PassPoliticalPhase,
        Resign,
        /// <summary>
        /// 发动战争，有Interaction版和非Interaction版
        /// Interaction版：
        /// 0.CardInfo
        /// 非Interaction版：
        /// 0. CardInfo
        /// 1. 目标玩家
        /// 2. 需要红点
        /// </summary>
        DeclareWar,
        /// <summary>
        /// 发动战争，有Interaction版和非Interaction版
        /// Interaction版：
        /// 0.CardInfo
        /// 非Interaction版：
        /// 0. CardInfo
        /// 1. 目标玩家
        /// 2. 需要红点
        /// </summary>
        Aggression,
        /// <summary>
        /// 响应事件
        /// 0.CardInfo（产生事件的Card）
        /// 1.OptionText
        /// </summary>
        ResolveEventOption,
        /// <summary>
        /// 出价殖民
        /// 0.CardInfo（产生事件的Card）
        /// 1.OptionText
        /// </summary>
        ColonizeBid,
        /// <summary>
        /// 派出殖民者
        /// 0.CardInfo(要出兵的殖民地)
        /// 1.需要提供的军力值
        /// 2.（用户提供，不能写入只能读取）要提供的部队和卡牌
        /// </summary>
        SendColonists,
        #endregion

        #region InterAction
        CancelInterAction,
        #endregion

    }
}
