using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Civilopedia
{
    public  abstract class TtaRuleBook
    {
        public abstract int CountColonizeForceValue(List<CardInfo> SelectedCard, CardInfo tacticInfo);
    }
}
