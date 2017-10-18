using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.WcfEntities;
using TtaWcfServer.ServerApi.LobbyService;

namespace TtaWcfServer.Service.Test
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“TestNormalService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 TestNormalService.svc 或 TestNormalService.svc.cs，然后开始调试。
    public class TestNormalService : ITestNormalService
    {
        public WcfServicePayload<WcfGame> QueryGameBoard(String sessionString, int roomId)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<WcfGame>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);

                var currentUsr = SessionManager.GetCurrentUser(sessionString);
                GameRoom room = hibernateSession.Get<GameRoom>(roomId);
                if (room != null)
                {
                    room.JoinedPlayer = LobbyServiceApi.CreateUserLites(room.Players, context);
                    room.ObserverPlayer = LobbyServiceApi.CreateUserLites(room.Observers, context);
                    room.RefereePlayer = LobbyServiceApi.CreateUserLites(room.Referees, context);

                    //验证权限
                    int priv = room.JoinedPlayer.FindIndex(p => p.Id == currentUsr.Id);
                    if (priv < 0)
                    {
                        if (room.RefereePlayer.Exists(p => p.Id == currentUsr.Id))
                        {
                            priv = WcfGame.PlayerNumberReferee;
                        }
                        else if (room.ObserverPlayer.Exists(p => p.Id == currentUsr.Id))
                        {
                            priv = WcfGame.PlayerNumberObserver;
                        }
                        else
                        {
                            if (room.AdditionalRules.Contains(CommonRoomRule.PublicObservable))
                            {
                                priv = WcfGame.PlayerNumberObserver;
                            }
                            else
                            {
                                //权限不足
                                return new WcfServicePayload<WcfGame>(null);
                            }
                        }
                    }


                    //给出结果
                    var manager = GameManager.GetManager(room, context);

                    WcfGame game = new WcfGame(manager.CurrentGame, priv, manager);

                    return new WcfServicePayload<WcfGame>(game);
                }

            }


            return new WcfServicePayload<WcfGame>(null);
        }
    }
}
