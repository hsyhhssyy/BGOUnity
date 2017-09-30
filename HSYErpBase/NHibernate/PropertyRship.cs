namespace HSYErpBase.NHibernate
{
    public class PropertyRship
    {
        public virtual int Id { get; set; }
        public virtual int FromEntityId { get; set; }
        public virtual int ToEntityId { get; set; }
        public virtual int RelationshipTypeId { get; set; }
    }
}
