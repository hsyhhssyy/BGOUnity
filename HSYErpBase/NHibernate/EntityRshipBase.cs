using System;

namespace HSYErpBase.NHibernate
{
    public class EntityRshipBase
    {
        public virtual int Id { get; set; }
        public virtual int BaseStructureId { get; set; }
        public virtual int BaseEntityId { get; set; }

        public virtual int RelatedStructureId { get; set; }
        public virtual int RelatedEntityId { get; set; }

        public virtual String Tag { get; set; }

        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}
