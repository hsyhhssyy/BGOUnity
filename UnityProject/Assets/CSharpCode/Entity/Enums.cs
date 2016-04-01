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
        Food, Ore, Science, Culture, MilitaryForce, Exploration,HappyFace, UnhappyFace,
        OreForMilitary, ScienceForMilitary,
        YellowMarker,BlueMarker, WhiteMarker,RedMarker,
        ExtraWhiteMarker,ExtraRedMarker,
        WorkerPool,
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
        Event, Colony, Tactic, War, Aggression, Pact,
        Unknown,
    }

    public enum PlayerActionType
    {
        Unknown,

        #region 常见内政行动
        TakeCardFromCardRow, PutBackCard,
        #endregion

        #region 常见政治行动
        PlayEvent, PlayeColony,
        #endregion
    }
}
