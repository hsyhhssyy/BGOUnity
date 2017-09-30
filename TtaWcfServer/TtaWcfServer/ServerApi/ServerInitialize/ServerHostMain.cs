using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HSYErpBase.NHibernate;
using TtaPesistanceLayer.NHibernate;

namespace TtaWcfServer.ServerApi.ServerInitializeApi
{
    public class ServerHostMain
    {
        public static void Start()
        {
            //初始化NHibernateHelper
            TtaNHibernateHelper helper=new TtaNHibernateHelper();
            NHibernateHelper.CurrentHelper = helper;
        }
    }
}