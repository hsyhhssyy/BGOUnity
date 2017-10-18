using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;

namespace TtaWcfServer.InGameLogic.Effects
{
    public static class EffectPoolStatics
    {
        public static IEnumerable<CardEffect> FilterEffect(this IEnumerable<CardEffect> enumrator,
            CardEffectType functionId, params int[] data)
        {
            return enumrator.Where(p =>
            {
                if (p.FunctionId != functionId)
                {
                    return false;
                }

                for (int index = 0; index < data.Length; index++)
                {
                    var o = data[index];
                    if (p.Data.Count < index)
                    {
                        return false;
                    }

                    if (o != p.Data[index])
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        
    }
}