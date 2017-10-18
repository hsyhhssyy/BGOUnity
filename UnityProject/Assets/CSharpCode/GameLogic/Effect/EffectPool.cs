using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.GameLogic.Effect
{
    /// <summary>
    /// 效果池
    /// </summary>
    public class EffectPool : IEnumerable<CardEffect>
    {
        protected EffectPool()
        {
            FlagEffects=new List<CardEffect>();
               SettledImmediateEffects =new List<SettledCardEffect>();
        }

        public EffectPool(TtaGame game,TtaBoard board, TtaCivilopedia civilopedia):this()
        {
            Civilopedia = civilopedia;
            _relatedGame = game;
            _relatedBoard = board;
            
            ReSyncStatus();
        }


        public TtaCivilopedia Civilopedia;
        private TtaGame _relatedGame;
        private readonly TtaBoard _relatedBoard;

        /// <summary>
        /// 记录所有玩家面板上的卡牌产生的持续效果
        /// </summary>
        private readonly List<CardInfo> _localCardPool = new List<CardInfo>();
        
        /// <summary>
        /// 旗标Effect，这些Effect会被服务器传递过来，因此需要进行保存，而且会被计算考虑进去。
        /// </summary>
        public List<CardEffect> FlagEffects;

        /// <summary>
        /// 记录已经结算过的ImmediateEffect，这些effect不会再被计算考虑，单纯就是记录
        /// </summary>
        public List<SettledCardEffect> SettledImmediateEffects;


        private void RecalcuatePool()
        {
               _calcuatedPool = new List<CardEffect>();
            foreach (var localPool in _localCardPool)
            {
                _calcuatedPool.AddRange(localPool.SustainedEffects);
            }
            _calcuatedPool.AddRange(FlagEffects);
        }

        #region 实现IEnumerable<CardEffect>

        private List<CardEffect> _calcuatedPool;

        public IEnumerator<CardEffect> GetEnumerator()
        {
            if (_calcuatedPool == null)
            {
                RecalcuatePool();
            }
            if (_calcuatedPool != null)
            {
                return _calcuatedPool.GetEnumerator();
            }
            else
            {
                return new List<CardEffect>().GetEnumerator();
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region LocalCardPool

        /// <summary>
        /// 将一张卡牌的SustainedEffects效果加入效果池，SustainedEffects将立刻可以被EffectPool遍历到。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool AddCardInfo(CardInfo info)
        {
            _localCardPool.Add(info);
            _calcuatedPool.AddRange(info.SustainedEffects);
            return true;
        }

        public bool RemoveCardInfo(CardInfo info)
        {
            if (_localCardPool.Contains(info))
            {
             
            _localCardPool.Remove(info);
            foreach (var effect in info.SustainedEffects)
            {
                _calcuatedPool.Remove(effect);
            }
            }
            return true;
        }

        #endregion      

        public void ReSyncStatus()
        {
            _localCardPool.Clear();
            if (_relatedBoard.Government != null)
            {
                _localCardPool.Add(_relatedBoard.Government);
            }

            RecalcuatePool();
        }

        #region Flags

        public bool AttachFlags(params CardEffect[] effects)
        {
            _calcuatedPool.AddRange(effects);
            FlagEffects.AddRange(effects);
            return true;
        }
        public bool AttachFlags(List<CardEffect> effects)
        {
            _calcuatedPool.AddRange(effects);
            FlagEffects.AddRange(effects);
            return true;
        }

        public bool DettachFlags(params CardEffect[] effects)
        {
            foreach (var cardEffect in effects)
            {
                if (FlagEffects.Contains(cardEffect))
                {
                    FlagEffects.Remove(cardEffect);
                    _calcuatedPool.Remove(cardEffect);
                }
            }
            return true;
        }
        public bool DettachFlags(List<CardEffect> effects)
        {
            foreach (var cardEffect in effects)
            {
                if (FlagEffects.Contains(cardEffect))
                {
                    FlagEffects.Remove(cardEffect);
                    _calcuatedPool.Remove(cardEffect);
                }
            }
            return true;
        }

        public bool ClearFlags()
        {
            ReplaceFlags(new List<CardEffect>());
            return true;
        }

        public bool ReplaceFlags(List<CardEffect> effects)
        {
            FlagEffects.Clear();
            FlagEffects.AddRange(effects);

            _calcuatedPool = null;
            return true;
        }

        public List<CardEffect> GetFlags()
        {
            var flags=new List<CardEffect   >();
            flags.AddRange(FlagEffects);
            return flags;
        } 

        #endregion

    }

    public class SettledCardEffect
    {
        public CardInfo RelatedCard;
        public CardEffect Effect;
    }
}
