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

    public enum CardType
    {
        Action,
        SpecialTechMilitary, SpecialTechExploration, SpecialTechCivil, SpecialTechEngineering,
        UrbanTechArena, UrbanTechLab, UrbanTechLibrary, UrbanTechTemple, UrbanTechTheater,
        ResourceTechFarm,
        ResourceTechMine,
        MilitaryTechAirForce, MilitaryTechArtillery, MilitaryTechCavalry, MilitaryTechInfantry,
        Wonder, Leader, Government,
        Event, Colony, Tactic, War, Aggression, Pact,Defend,
        Unknown,
    }

    public enum PlayerActionType
    {
        Unknown, ProgramDelegateAction,

        #region 常见内政行动
        /// <summary>
        /// 玩家可以从卡牌列上拿一张卡
        /// 0. CardInfo
        /// 1. 此卡在卡牌列上的位置，0开始
        /// </summary>
        TakeCardFromCardRow, PutBackCard,
        /// <summary>
        /// 玩家可以增加一个人口
        /// 0. 保留字段
        /// 1. 需要的Food
        /// </summary>
        IncreasePopulation,
        /// <summary>
        /// 玩家可以建造一个建筑物
        /// 0. CardInfo
        /// 1. Price
        /// </summary>
        BuildBuilding,
        /// <summary>
        /// 玩家可以升级一个建筑物
        /// 0. CardInfo (from)
        /// 1. CardInfo (to)
        /// 2. Price
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
        #endregion

        #region 常见政治行动
        PlayEvent, PlayeColony,
        #endregion
    }
}
