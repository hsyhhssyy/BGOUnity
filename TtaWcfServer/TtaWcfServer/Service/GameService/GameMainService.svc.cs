using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using HSYErpBase.NHibernate;
using HSYErpBase.Wcf;
using HSYErpBase.Wcf.CommonApi;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.InGameLogic.WcfEntities;
using TtaWcfServer.ServerApi.LobbyService;

namespace TtaWcfServer.Service.GameService
{
    [ServiceContract(Namespace = "")]
    [WCF_ExceptionBehaviour(typeof(WCF_ExceptionHandler))]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GameMainService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<WcfGame> QueryGameBoard(String sessionString,int roomId)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<WcfGame>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context=new WcfContext(sessionString,hibernateSession);

                var currentUsr=SessionManager.GetCurrentUser(sessionString);
                GameRoom room = hibernateSession.Get<GameRoom>(roomId);
                if (room != null)
                {
                    room.JoinedPlayer = LobbyServiceApi.CreateUserLites(room.Players, context);
                    room.ObserverPlayer = LobbyServiceApi.CreateUserLites(room.Observers, context);
                    room.RefereePlayer = LobbyServiceApi.CreateUserLites(room.Referees, context);

                    //验证权限
                    int priv = room.JoinedPlayer.FindIndex(p => p.Id == currentUsr.Id);
                    if (priv<0)
                    {
                        if (room.RefereePlayer.Exists(p => p.Id == currentUsr.Id))
                        {
                            priv = WcfGame.PlayerNumberReferee;
                        }
                        else if(room.ObserverPlayer.Exists(p => p.Id == currentUsr.Id))
                        {
                            priv = WcfGame.PlayerNumberObserver;
                        }else
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
                    var manager=GameManager.GetManager(room, context);

                    WcfGame game = new WcfGame(manager.CurrentGame, priv, manager);

                    return new WcfServicePayload<WcfGame>(game);
                }
                
            }


            return new WcfServicePayload<WcfGame>(null);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<ActionResponse> PerformAction(String sessionString, int roomId, PlayerAction action,ActionResponse clientResponse)
        {
            var val = ValidateSessionApi.CurrentValidator.ValidateSession<ActionResponse>(sessionString);
            if (val != null)
            {
                return val;
            }

            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                WcfContext context = new WcfContext(sessionString, hibernateSession);
                
                GameRoom room;
                int priv = GetAndValidateRoom(context,roomId,out room);
                if (room != null&&priv>=0)
                {
                    //给出结果
                    var manager = GameManager.GetManager(room, context);

                    var response=manager.TakeAction(priv,action,action.Data, clientResponse,context);
                    return new WcfServicePayload<ActionResponse>(response);
                }

            }


            return new WcfServicePayload<ActionResponse>(null);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<List<TtaBoard>> QueryJournal(String sessionString, int roomId)
        {
            return null;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        public WcfServicePayload<String> SendInGameChat(String sessionString, int matchId)
        {
            return null;
        }

        private int GetAndValidateRoom(WcfContext context, int roomId,out GameRoom room)
        {
            var currentUsr = SessionManager.GetCurrentUser(context.Session);
            room = context.HibernateSession.Get<GameRoom>(roomId);
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
                            priv = WcfGame.PlayerNumberNotPaticpated;
                        }
                    }
                }

                return priv;
            }
            return WcfGame.PlayerNumberNotPaticpated;
        }
    }
}
