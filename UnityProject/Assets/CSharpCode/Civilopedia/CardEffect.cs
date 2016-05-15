using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Civilopedia
{
    public class CardEffect
    {
        public CardEffectType FunctionId;

        public List<int> Data;
    }

    /// <summary>
    /// 为了执行编译/运行时类型检查，强制对int做范围校验
    /// </summary>
    public enum CardEffectType
    {
        //-------------E100------
       ResourceOfTypeXChangedY = 100,
       
    }
}
