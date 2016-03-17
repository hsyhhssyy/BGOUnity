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
        YellowMarker, RedMarker, BlueMarker, WhiteMarker
    }
}
