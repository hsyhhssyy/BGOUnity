using HSYErpBase.NHibernate;
using TtaPesistanceLayer.NHibernate;
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
    }
}