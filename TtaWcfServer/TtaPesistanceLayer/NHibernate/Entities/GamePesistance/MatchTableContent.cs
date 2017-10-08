using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSYErpBase.EntityDefinition;

namespace TtaPesistanceLayer.NHibernate.Entities.GamePesistance
{
    public class MatchTableContent:BasicEntity
    {
        public virtual String MatchData { get; set; }
    }
}
