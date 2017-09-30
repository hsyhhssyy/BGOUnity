using System;
using System.IO;
using HSYErpBase.EntityDefinition;
using HSYErpBase.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using TtaCommonLibrary.Entities.ClientUpdateModel;

namespace TtaPesistanceLayer.NHibernate
{
    public class TtaNHibernateHelper:NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    configuration.AddAssembly(typeof(NHibernateHelper).Assembly);//引入HSYErpBase
                    configuration.AddAssembly(typeof(VersionInfo).Assembly);//引入TtaCommonLibrary
                    configuration.AddAssembly(typeof(TtaNHibernateHelper).Assembly);//引入TtaPesistanceLayer

                    _sessionFactory = configuration.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
        
        private static string GetDefaultConfigurationFilePath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
            return Path.Combine(binPath, "hibernate_gis.cfg.xml");
        }

        public override ISession OpenSession()
        {
            var session= SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }
        
    }
}
