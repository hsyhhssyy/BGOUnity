using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.Civilpedia.RuleBook;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic
{
    public partial class GameManager
    {
        protected GameManager()
        {
            Handlers=new List<ActionHandler>();
        }

        

        public bool LoadFromPesistance(GameRoom room)
        {
            SetupNewGame(room);
            return true;
        }

        public void SaveToPesistance()
        {
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

        private TtaGame CurrentGame;
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