using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using HSYErpBase.Wcf;
using NHibernate;
using TtaCommonLibrary.Entities.GameModel;
using TtaPesistanceLayer.NHibernate.Entities.GamePesistance;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.InGameLogic.WcfEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic
{
    public partial class GameManager
    {
        protected GameManager()
        {
            Handlers=new List<ActionHandler>();
        }


        readonly XmlSerializer _serializer = new XmlSerializer(typeof(TtaGame));

        public bool LoadFromPesistance(GameRoom room, WcfContext context)
        {
            int relatedMatch = room.RelatedMatchId;
            if (relatedMatch <= 0)
            {
                return false;
            }

            var content= context.HibernateSession.Get<MatchTableContent>(relatedMatch);
            if (content == null)
            {
                return false;
            }

            StringReader sr = new StringReader(content.MatchData);
            CurrentGame=(TtaGame) _serializer.Deserialize(sr);
            
            sr.Close();
            CurrentGame.Room = room;

            return true;
        }

        public void SaveToPesistance(WcfContext context)
        {
            CurrentGame.Room.RelatedMatchId = 1;
            StringWriter sw=new StringWriter();
            _serializer.Serialize(sw,this.CurrentGame);
            String lob = sw.ToString();
            MatchTableContent content = new MatchTableContent
            {
                Id = CurrentGame.Id,
                MatchData = lob
            };


            if (CurrentGame.Id > 0)
            {
                 context.HibernateSession.Update(content);
            }
            else
            {
                int id=(int) context.HibernateSession.Save(content);
                CurrentGame.Id = id;
                CurrentGame.Room.RelatedMatchId = id;
                context.HibernateSession.Update(CurrentGame.Room);

                context.HibernateSession.Flush();
            }
            sw.Close();
        }

        private void SetupNewGame(GameRoom room)
        {
            var pedia=TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);
            //目前无视其他的设置仅支持2人    
            var rule = pedia.GetRuleBook();
            //设置双方面板
            CurrentGame = rule.SetupNewGame(room);

            //未设置的内容包括玩家分配和游戏关联
            //分配玩家
            if (room.AdditionalRules.Contains(CommonRoomRule.RandomSeats))
            {
                room.JoinedPlayer=room.JoinedPlayer.Shuffle();
            }

            for (int index = 0; index < room.JoinedPlayer.Count; index++)
            {
                var player = room.JoinedPlayer[index];
                CurrentGame.Boards[index].PlayerId = player.Id;
            }

            //关联游戏
            CurrentGame.Room = room;

            //完成了
        }

        private List<ActionHandler> Handlers;

        public TtaGame CurrentGame { get; set; }
        private List<TtaBoard> CurrentBoards => CurrentGame.Boards;
        
        public List<PlayerAction> GetPossibleActions(int playerNo)
        {
            List<PlayerAction> actions=new List<PlayerAction>();
            foreach (var handler in Handlers)
            {
                actions.AddRange(handler.CheckAbleToPerform(playerNo)); 
            }
            return actions;
        }

        public int ReturnResourceCount(ResourceType type, int playerNo)
        {
            return 0;
        }
    }
}