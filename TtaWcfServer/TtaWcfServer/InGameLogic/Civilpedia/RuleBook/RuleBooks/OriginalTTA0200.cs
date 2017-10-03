using System.Collections.Generic;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.Civilpedia.RuleBook.RuleBooks
{
	public class OriginalTta0200 : TtaRuleBook
	{
	    private TtaCivilopedia civilopedia;

	    public override TtaGame SetupNewGame(GameRoom room)
	    {
	        TtaGame game = new TtaGame();
            game.Boards = new List<TtaBoard>();
            for (int i = 0; i < room.PlayerMax; i++)
            {
                var board = SetupNewBoard();
                game.Boards.Add(board);
            }

            //洗牌
	        var deck=GetCivilDeckForAge(Age.A).ShuffleCards();

            //发13张牌
            game.CardRow = new List<CardInfo>();
            for (int i = 0; i < 13; i++)
	        {
                game.CardRow.Add(deck.DrawCard());
            }

	        game.CivilCardsDeck = deck;
			//Age A 没有MilitaryCard
			game.MilitaryCardsDeck=new List<CardInfo>();

			game.CurrentAge= Age.A;
	        game.CurrentRound = 1;

			//洗入玩家人数相当的卡牌
	        var eventdeck = new List<CardInfo>()
            {
                civilopedia.GetCardInfoById("4001"),
                civilopedia.GetCardInfoById("4002"),
                civilopedia.GetCardInfoById("4003"),
                civilopedia.GetCardInfoById("4004"),
                civilopedia.GetCardInfoById("4005"),
                civilopedia.GetCardInfoById("4006"),
                civilopedia.GetCardInfoById("4007"),
                civilopedia.GetCardInfoById("4008"),
                civilopedia.GetCardInfoById("4009"),
                civilopedia.GetCardInfoById("4010"),
            }.ShuffleCards();
			game.CurrentEventDeck=new List<CardInfo>();
	        for (int i = 0; i < room.PlayerMax + 2; i++)
	        {
                game.CurrentEventDeck.Add(eventdeck.DrawCard());
            }

			game.FutureEventDeck=new List<CardInfo>();
			game.DiscardedMilitaryCardsDeck=new List<CardInfo>();
			game.PastEventDeck=new List<CardInfo>();

			//Phase不设置，这个由GameManager设置
			game.SharedTactics=new List<CardInfo>();
	        game.Version = room.GameRuleVersion;

	        return game;
	    }

	    private TtaBoard SetupNewBoard()
	    {
            TtaBoard board=new TtaBoard();

			//初始建筑物和建筑科技
			board.Buildings=new Dictionary<BuildingType, Dictionary<Age, BuildingCell>>();
			board.Buildings[BuildingType.Farm].Add(Age.A, new BuildingCell()
			{
			    Card = civilopedia.GetCardInfoById("12001")//Agriculture
				,Worker = 2,
				Storage = 0
            });
            board.Buildings[BuildingType.Mine].Add(Age.A, new BuildingCell()
            {
                Card = civilopedia.GetCardInfoById("13001")//Bronze
                ,
                Worker = 2,
                Storage = 0
            });
            board.Buildings[BuildingType.Lab].Add(Age.A, new BuildingCell()
            {
                Card = civilopedia.GetCardInfoById("20001")//Philosophy
                ,
                Worker = 1,
                Storage = 0
            });
            board.Buildings[BuildingType.Temple].Add(Age.A, new BuildingCell()
            {
                Card = civilopedia.GetCardInfoById("22001")//Religion
                ,
                Worker = 2,
                Storage = 0
            });
            board.Buildings[BuildingType.Infantry].Add(Age.A, new BuildingCell()
            {
                Card = civilopedia.GetCardInfoById("10001")//Warrior
                ,
                Worker = 1,
                Storage = 0
            });


			//初始为空的那些东西们
			board.CivilCards=new List<CardInfo>();
			board.Colonies=new List<CardInfo>();
			board.CompletedWonders=new List<CardInfo>();
	        board.ConstructingWonder = null;
	        board.ConstructingWonderSteps = 0;
			board.CurrentEventPlayed=new List<CardInfo>();
			board.FutureEventPlayed=new List<CardInfo>();

			//专制政体
	        board.Government = civilopedia.GetCardInfoById("5001");//Despotism

	        board.Leader = null;
			board.MilitaryCards=new List<CardInfo>();
			board.CivilCards=new List<CardInfo>();
			board.SpecialTechs=new List<CardInfo>();
	        board.Tactic = null;

			board.Warnings=new List<Warning>();

	        board.InitialBlueMarkerCount = 16;
	        board.InitialBlueMarkerCount = 12;

            return board;
	    }

	    private List<CardInfo> GetCivilDeckForAge(Age age)
	    {
	        switch (age)
	        {
	                case Age.A:
	                return new List<CardInfo>()
                    {
                        civilopedia.GetCardInfoById("6001"),
                        civilopedia.GetCardInfoById("6002"),
                        civilopedia.GetCardInfoById("6003"),
                        civilopedia.GetCardInfoById("6004"),
                        civilopedia.GetCardInfoById("6005"),
                        civilopedia.GetCardInfoById("6006"),
                        civilopedia.GetCardInfoById("6007"),
                        civilopedia.GetCardInfoById("6008"),

                        civilopedia.GetCardInfoById("1001"),
                        civilopedia.GetCardInfoById("1002"),
                        civilopedia.GetCardInfoById("1003"),
                        civilopedia.GetCardInfoById("1004"),
                        civilopedia.GetCardInfoById("1005"),
                        civilopedia.GetCardInfoById("1006"),
                        civilopedia.GetCardInfoById("1007"),

                        civilopedia.GetCardInfoById("25001"),
                        civilopedia.GetCardInfoById("25002"),
                        civilopedia.GetCardInfoById("25003"),
                        civilopedia.GetCardInfoById("25004"),
                    };
	        }
			return new List<CardInfo>();
	    }

	}
}