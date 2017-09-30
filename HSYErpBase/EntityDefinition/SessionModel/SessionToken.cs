using System;

namespace HSYErpBase.EntityDefinition.SessionModel
{
    public class SessionToken
    {
        public virtual int SessionRId { get; set; }

        public virtual String SessionGuid { get; set; }

        public virtual DateTime? GenerationTime { get; set; }

        public virtual int? User { get; set; }

        public virtual DateTime? LastOperationTime { get; set; }

        public virtual String LastOperation { get; set; }
    }
}
