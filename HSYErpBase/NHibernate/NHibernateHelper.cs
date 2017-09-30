using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace HSYErpBase.NHibernate
{
    public abstract class NHibernateHelper
    {
        public static NHibernateHelper CurrentHelper { get; set; }
        /// <summary>
        /// 根据服务器的设定打开一个Session
        /// </summary>
        /// <returns></returns>
        public abstract ISession OpenSession();
    }
}
