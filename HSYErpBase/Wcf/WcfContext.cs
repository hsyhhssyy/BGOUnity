using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace HSYErpBase.Wcf
{
    public class WcfContext
    {
        public WcfContext(string sessionString, ISession hibernateSession)
        {
            this.Session = sessionString;
            this.HibernateSession = hibernateSession;
        }

        public ISession HibernateSession { get; set; }

        public String Session { get; set; }
    }
}
