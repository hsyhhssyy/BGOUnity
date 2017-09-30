using System;
using System.IO;
using NHibernate;
using NHibernate.Cfg;

namespace TtaPesistanceLayer.NHibernate
{
    public class NHibernateHelper
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
                    //configuration.AddAssembly(typeof(ErpPropertyInfo).Assembly);//引入RailwayERPCommonLib
                    configuration.AddAssembly(typeof(NHibernateHelper).Assembly);//引入RailwayERPDevicePlatform

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

        public static ISession OpenSession()
        {
            var session= SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }
        
    }
}
