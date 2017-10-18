using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.Actions;
using Assets.CSharpCode.GameLogic.Effect;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Network.Wcf.Entities;
using Assets.CSharpScripts.Helper;
using UnityEngine;

namespace Assets.CSharpCode.Network.Wcf
{
    /// <summary>
    /// 本类是一个帮助类，用来将Json的数据，转化成Unity的游戏实体，不涉及具体网络传输的内容
    /// </summary>
    public static class WcfJsonPageProvider
    {
        public const String BgoBaseUrl = "http://www.boardgaming-online.com/";

        public static WcfGame ParseTtaGameWithRoomJson(JSONObject room)
        {
            WcfGame game = new WcfGame();
            game.Name = room.GetField("RoomName").str;
            game.RoomId = (int)room.GetField("Id").i;

            return game;
        }

        public static void ParseTtaGameWithGameJson(WcfGame game, JSONObject json)
        {
            var civilopedia = TtaCivilopedia.GetCivilopedia(game.Version);

            game.CurrentAge = (Age)json.TryGetField("CurrentAge").i;
            game.CurrentRound = (int)json.TryGetField("CurrentRound").i;

            game.CurrentEventAge = (Age)json.TryGetField("CurrentEventAge").i;
            game.CurrentEventCard = ParseCardInfoFromJson(civilopedia, json.TryGetField("CurrentEventCard"));
            game.CurrentEventCount = (String)json.TryGetField("CurrentEventCount").str;

            game.FutureEventAge = (Age)json.TryGetField("FutureEventAge").i;
            game.FutureEventCount = (String)json.TryGetField("FutureEventCount").str;

            game.CivilCardsRemain = (int)json.TryGetField("CivilCardsRemain").i;
            game.MilitaryCardsRemain = (int)json.TryGetField("MilitaryCardsRemain").i;
            
            game.CurrentPhase = (TtaPhase)json.TryGetField("CurrentPhase").i;
            game.CurrentPlayer = (int)json.TryGetField("CurrentPlayer").i;
            game.MyPlayerIndex = (int)json.TryGetField("MyPlayerIndex").i;

            game.SharedTactics = ParseCardInfoListFromJson(civilopedia, json.TryGetField("SharedTactics"));

            var cardRowJson=json.TryGetField("CardRow");
            game.CardRow=new List<CardRowCardInfo>();

            foreach (var cardRowInfoJson in cardRowJson.list)
            {
                game.CardRow.Add(ParseCardRowCardInfoFromJson(civilopedia,cardRowInfoJson));
            }

            // game.Boards;
            var boardsList= json.TryGetField("Boards");
            game.Boards=new List<TtaBoard>();
            foreach (var jsonObject in boardsList.list)
            {
                var board = ParseTtaBoardWithBoardJson(civilopedia,game, jsonObject);
                game.Boards.Add(board);
            }



            WcfActionFormater.Format(game, json.TryGetField("PossibleActions"),civilopedia);
            WcfJournalFormater.Format(game, json.TryGetField("Journals"), civilopedia);
        }

        public static TtaBoard ParseTtaBoardWithBoardJson(TtaCivilopedia civilopedia, WcfGame game, JSONObject json)
        {
            TtaBoard board=new TtaBoard();
            
            board.PlayerName = json.TryGetField("PlayerName").str;

            board.CompletedWonders = ParseCardInfoListFromJson(civilopedia, json.TryGetField("CompletedWonders"));

            board.ConstructingWonder=ParseCardInfoFromJson(civilopedia, json.TryGetField("ConstructingWonder"));
            board.ConstructingWonderSteps= json.TryGetField("ConstructingWonderSteps").list.Select(j=>j.i).Select(q=>q.ToString()).ToList();


            board.SpecialTechs = ParseCardInfoListFromJson(civilopedia, json.TryGetField("SpecialTechs"));
            board.Colonies = ParseCardInfoListFromJson(civilopedia, json.TryGetField("Colonies"));


            board.Government = ParseCardInfoFromJson(civilopedia, json.TryGetField("Government"));
            board.Leader = ParseCardInfoFromJson(civilopedia, json.TryGetField("Leader"));

            board.Tactic = ParseCardInfoFromJson(civilopedia, json.TryGetField("Tactic"));

            board.Warnings=new List<Warning>();//TODO Wanings
            //public List<Warning> Warnings;

            board.CivilCards=ParseCardInfoListFromJson(civilopedia, json.TryGetField("CivilCards"));

            board.MilitaryCards= ParseCardInfoListFromJson(civilopedia, json.TryGetField("MilitaryCards"));

            board.CurrentEventPlayed = ParseCardInfoListFromJson(civilopedia, json.TryGetField("CurrentEventPlayed"));
            board.FutureEventPlayed = ParseCardInfoListFromJson(civilopedia, json.TryGetField("FutureEventPlayed"));

            board.InitialBlueMarkerCount = (int)json.TryGetField("InitialBlueMarkerCount").i;
            board.InitialYellowMarkerCount = (int)json.TryGetField("InitialYellowMarkerCount").i;

            board.Buildings=new Dictionary<BuildingType, Dictionary<Age, BuildingCell>>();
            var buildingJson = json.TryGetField("Buildings");
            foreach (var pair in buildingJson.list)
            {
                int type = (int)pair.TryGetField("Key").i;

                JSONObject value = pair.TryGetField("Value");
                Dictionary<Age, BuildingCell> dict=new Dictionary<Age, BuildingCell>();

                foreach (var cellPair in value.list)
                {
                    int age = (int)cellPair.TryGetField("Key").i;

                    var cellJson = cellPair.TryGetField("Value");
                    BuildingCell cell=new BuildingCell();
                    cell.Card= ParseCardInfoFromJson(civilopedia, cellJson.TryGetField("Card"));
                    cell.Storage = (int)cellJson.TryGetField("Storage").i;
                    cell.Worker = (int)cellJson.TryGetField("Worker").i;

                    dict.Add((Age)age, cell);
                }

                board.Buildings.Add((BuildingType) type,dict);

            }

            var resJson= json.TryGetField("Resource");
            foreach (var pair in resJson.list)
            {
                int key = (int)pair.TryGetField("Key").i;
                int value = (int)pair.TryGetField("Value").i;
                board.Resource.Add((ResourceType)key,value);
            }


            if (board.EffectPool == null)
            {
                board.EffectPool = new EffectPool(game, board, civilopedia);
            }

            return board;
        }

        public static CardRowCardInfo ParseCardRowCardInfoFromJson(TtaCivilopedia pedia, JSONObject json)
        {
            CardRowCardInfo info=new CardRowCardInfo();
            String internalId = json.TryGetPath("Card").TryGetField("InternalId").str;
            info.Card=pedia.GetCardInfoById(internalId);
            info.CanPutBack = json.TryGetField("CanPutBack").b;
            info.CanTake= !json.TryGetField("AlreadyTaken").b;
            info.CivilActionCost= (int)json.TryGetField("CivilCount").i;

            return info;
        }
        
        public static CardInfo ParseCardInfoFromJson(TtaCivilopedia pedia, JSONObject json)
        {
            if (json == null || json.IsNull)
            {
                return null;
            }
            String internalId = json.TryGetField("InternalId").str;
            return pedia.GetCardInfoById(internalId);
        }

        public static List<CardInfo> ParseCardInfoListFromJson(TtaCivilopedia pedia, JSONObject json)
        {
            return json.list.Select(card => ParseCardInfoFromJson(pedia, card)).ToList();
        }


        public static ActionResponse ParseActionResponse(JSONObject json)
        {
            return new ActionResponse();
        }
    }
}
