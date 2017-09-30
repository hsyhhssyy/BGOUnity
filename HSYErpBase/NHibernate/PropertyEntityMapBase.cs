using System;

namespace HSYErpBase.NHibernate
{
    public class PropertyEntityMapBase
    {
        public virtual int Id { get; set; }

        public virtual int EntityId { get; set; }

        public virtual int PropertyId { get; set; }

        public virtual String Value { get; set; }

        public virtual DateTime? BeginDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
    }


}
