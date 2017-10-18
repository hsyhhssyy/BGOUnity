using System.Collections.Generic;
using System.Linq;
using Assets.CSharpCode.Civilopedia;

namespace Assets.CSharpCode.GameLogic.Effect
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