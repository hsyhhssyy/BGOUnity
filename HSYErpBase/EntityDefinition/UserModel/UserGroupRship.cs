using System;
using HSYErpBase.NHibernate;

namespace HSYErpBase.EntityDefinition.UserModel
{
    public class UserGroupRship:EntityRshipBase
    {
        public virtual int GroupId => BaseEntityId;
        public virtual int UserId => RelatedEntityId;
        public virtual String Title { get; set; }
        public virtual String Nickname { get; set; }

    }
}
