using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using NHibernate;
using TtaPesistanceLayer.NHibernate;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.OffGameLogic.Ranking;

namespace TtaWcfServer.ServerApi.ServerInitialize
{
    public class ServerHostMain
    {
        public static void Start()
        {
            //初始化NHibernateHelper
            TtaNHibernateHelper helper=new TtaNHibernateHelper();
            NHibernateHelper.CurrentHelper = helper;

            //启动RankingManager
            RankingManager.StartRankingManager();
        }

        public static void Shutdown(WcfContext shutdownServerContext = null)
        {
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                if (shutdownServerContext == null)
                {
                    shutdownServerContext = new WcfContext("Shutdown!", hibernateSession);
                }
                else
                {
                    shutdownServerContext.Session = "Shutdown!";
                }

                GameManager.PesistAllMatch(shutdownServerContext);
            }
        }
    }
}