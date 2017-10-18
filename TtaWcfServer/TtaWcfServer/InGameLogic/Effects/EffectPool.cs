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
    public class EffectPool:IEnumerable<CardEffect>
    {
        public EffectPool()
        {
            
        }

        public EffectPool(TtaBoard board, TtaCivilopedia civilopedia)
        {
            Civilopedia = civilopedia;
            _localPool.Add(board.Government.CivilpediaCheck(Civilopedia));
            
            RecalcuatePool();
        }

        public TtaCivilopedia Civilopedia;
        private readonly List<CardInfo> _localPool=new List<CardInfo>();

        public List<SettledCardEffect> SettledImmediateEffects;


        private void RecalcuatePool()
        {
            _calcuatedPool = new List<CardEffect>();
            foreach (var localPool in _localPool)
            {
                _calcuatedPool.AddRange(localPool.CivilpediaCheck(Civilopedia).SustainedEffects);
            }
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
            }else
            {
                return new List<CardEffect>().GetEnumerator();
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public bool AddCardInfo(CardInfo info)
        {
            _localPool.Add(info);
            _calcuatedPool.AddRange(info.SustainedEffects);
            return true;
        }

        [Obsolete]
        public IEnumerable<CardEffect> FilterEffects(params CardEffectType[] functionIds)
        {
            List<CardEffect> es=new List<CardEffect>();
            foreach (var effect in this)
            {
                foreach (var id in functionIds)
                {
                    if (effect.FunctionId == id)
                    {
                        es.Add(effect);
                        break;
                    }
                }
            }

            return es;
        }
    }

    public class SettledCardEffect
    {
        public CardInfo RelatedCard;
        public CardEffect Effect;
    }
}