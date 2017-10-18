using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Effect
{
    public static class EffectExecutor
    {
        /// <summary>
        /// 为某位玩家执行一个特定效果（可能会受到其他效果的影响而产生不同效果）
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="playerNo"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        public static List<GameMove> ExecuteEffect(GameLogicManager manager,int playerNo,CardEffect effect)
        {

            return new List<GameMove>();
        }
    }
}
