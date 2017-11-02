using System.Collections.Generic;
using TtaCommonLibrary.Entities.GameModel;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.Civilpedia.RuleBook.RuleBooks
{
	public class OriginalTta0200 : TtaRuleBook
	{
	    private TtaCivilopedia _civilopedia;

	    public override TtaGame SetupNewGame(GameRoom room)
	    {
            _civilopedia=TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);

	        TtaGame game = new TtaGame {Boards = new List<TtaBoard>()};
	        for (int i = 0; i < room.PlayerMax; i++)
            {
                var board = SetupNewBoard();
                board.UncountableResourceCount[ResourceType.WhiteMarker] = i+1;
                game.Boards.Add(board);
            }

            //洗牌
	        var deck=GetCivilDeckForAge(room,Age.A).Shuffle();

            //发13张牌
            game.CardRow =new List<CardRowInfo>();
            for (int i = 0; i < 13; i++)
            {
                var info = new CardRowInfo
                {
                    Card = deck.DrawCard(),
                    TakenBy = -1
                };
                game.CardRow.Add(info);
            }

	        game.CivilCardsDeck = deck;
			//Age A 没有MilitaryCard
			game.MilitaryCardsDeck=new List<CardInfo>();

			game.CurrentAge= Age.A;
	        game.CurrentRound = 1;

			//洗入玩家人数相当的卡牌
	        var eventdeck = new List<CardInfo>()
            {
                _civilopedia.GetCardInfoById("4001"),
                _civilopedia.GetCardInfoById("4002"),
                _civilopedia.GetCardInfoById("4003"),
                _civilopedia.GetCardInfoById("4004"),
                _civilopedia.GetCardInfoById("4005"),
                _civilopedia.GetCardInfoById("4006"),
                _civilopedia.GetCardInfoById("4007"),
                _civilopedia.GetCardInfoById("4008"),
                _civilopedia.GetCardInfoById("4009"),
                _civilopedia.GetCardInfoById("4010"),
            }.Shuffle();
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
	        TtaBoard board = new TtaBoard
	        {
	            Buildings = new XmlSerializableDictionary<BuildingType, XmlSerializableDictionary<Age, BuildingCell>>
	            {
	                {BuildingType.Farm, new XmlSerializableDictionary<Age, BuildingCell>()},
	                {BuildingType.Mine, new XmlSerializableDictionary<Age, BuildingCell>()},
	                {BuildingType.Lab, new XmlSerializableDictionary<Age, BuildingCell>()},
	                {BuildingType.Temple, new XmlSerializableDictionary<Age, BuildingCell>()},
	                {BuildingType.Infantry, new XmlSerializableDictionary<Age, BuildingCell>()}
	            }
	        };

	        //初始建筑物和建筑科技

	        board.Buildings[BuildingType.Farm].Add(Age.A, new BuildingCell()
			{
			    Card = _civilopedia.GetCardInfoById("12001")//Agriculture
				,Worker = 2,
				Storage = 0
            });
            board.Buildings[BuildingType.Mine].Add(Age.A, new BuildingCell()
            {
                Card = _civilopedia.GetCardInfoById("13001")//Bronze
                ,
                Worker = 2,
                Storage = 0
            });
            board.Buildings[BuildingType.Lab].Add(Age.A, new BuildingCell()
            {
                Card = _civilopedia.GetCardInfoById("20001")//Philosophy
                ,
                Worker = 1,
                Storage = 0
            });
            board.Buildings[BuildingType.Temple].Add(Age.A, new BuildingCell()
            {
                Card = _civilopedia.GetCardInfoById("22001")//Religion
                ,
                Worker = 0,
                Storage = 0
            });
            board.Buildings[BuildingType.Infantry].Add(Age.A, new BuildingCell()
            {
                Card = _civilopedia.GetCardInfoById("10001")//Warrior
                ,
                Worker = 1,
                Storage = 0
            });

            //白送一个空闲工人
	        board.UncountableResourceCount[ResourceType.WorkerPool] = 1;

			//初始为空的那些东西们
			board.CivilCards=new List<HandCardInfo>();
			board.Colonies=new List<CardInfo>();
			board.CompletedWonders=new List<CardInfo>();
	        board.ConstructingWonder = null;
	        board.ConstructingWonderSteps = 0;
			board.CurrentEventPlayed=new List<CardInfo>();
			board.FutureEventPlayed=new List<CardInfo>();

			//专制政体
	        board.Government = _civilopedia.GetCardInfoById("5001");//Despotism

	        board.Leader = null;
			board.MilitaryCards=new List<HandCardInfo>();
			board.CivilCards=new List<HandCardInfo>();
			board.SpecialTechs=new List<CardInfo>();
	        board.Tactic = null;

			board.Warnings=new List<Warning>();

	        board.InitialYellowMarkerCount = 24;
	        board.InitialBlueMarkerCount = 16;

            return board;
	    }

	    public override List<CardInfo> GetCivilDeckForAge(GameRoom room,Age age)
	    {
            _civilopedia = TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);

            switch (age)
	        {
	                case Age.A:
#region Age A Civil Deck
                    return new List<CardInfo>()
                    {
                        _civilopedia.GetCardInfoById("6001"),
                        _civilopedia.GetCardInfoById("6002"),
                        _civilopedia.GetCardInfoById("6003"),
                        _civilopedia.GetCardInfoById("6004"),
                        _civilopedia.GetCardInfoById("6005"),
                        _civilopedia.GetCardInfoById("6006"),
                        _civilopedia.GetCardInfoById("6007"),
                        _civilopedia.GetCardInfoById("6008"),

                        _civilopedia.GetCardInfoById("1001"),
                        _civilopedia.GetCardInfoById("1002"),
                        _civilopedia.GetCardInfoById("1003"),
                        _civilopedia.GetCardInfoById("1004"),
                        _civilopedia.GetCardInfoById("1005"),
                        _civilopedia.GetCardInfoById("1006"),
                        _civilopedia.GetCardInfoById("1007"),

                        _civilopedia.GetCardInfoById("25001"),
                        _civilopedia.GetCardInfoById("25002"),
                        _civilopedia.GetCardInfoById("25003"),
                        _civilopedia.GetCardInfoById("25004"),
                    }.Shuffle();
                #endregion
                case Age.I:
                    #region Age I Civil Deck
                    return new List<CardInfo>()
                    {
                        _civilopedia.GetCardInfoById("19101"),
                        _civilopedia.GetCardInfoById("20101"),
                        _civilopedia.GetCardInfoById("20101"),
                        _civilopedia.GetCardInfoById("21101"),
                        _civilopedia.GetCardInfoById("22101"),
                        _civilopedia.GetCardInfoById("23101"),

                        _civilopedia.GetCardInfoById("9101"),
                        _civilopedia.GetCardInfoById("9101"),
                        _civilopedia.GetCardInfoById("10101"),
                        _civilopedia.GetCardInfoById("10101"),

                        _civilopedia.GetCardInfoById("25101"),
                        _civilopedia.GetCardInfoById("25102"),
                        _civilopedia.GetCardInfoById("25103"),
                        _civilopedia.GetCardInfoById("25104"),

                        _civilopedia.GetCardInfoById("6101"),
                        _civilopedia.GetCardInfoById("6102"),
                        _civilopedia.GetCardInfoById("6103"),
                        _civilopedia.GetCardInfoById("6104"),
                        _civilopedia.GetCardInfoById("6105"),
                        _civilopedia.GetCardInfoById("6106"),

                        _civilopedia.GetCardInfoById("5101"),
                        _civilopedia.GetCardInfoById("5102"),

                        _civilopedia.GetCardInfoById("14101"),
                        _civilopedia.GetCardInfoById("15101"),
                        _civilopedia.GetCardInfoById("16101"),
                        _civilopedia.GetCardInfoById("17101"),

                        _civilopedia.GetCardInfoById("1101"),
                        _civilopedia.GetCardInfoById("1101"),
                        _civilopedia.GetCardInfoById("1102"),
                        _civilopedia.GetCardInfoById("1103"),
                        _civilopedia.GetCardInfoById("1204"),
                        _civilopedia.GetCardInfoById("1204"),
                        _civilopedia.GetCardInfoById("1205"),
                        _civilopedia.GetCardInfoById("1206"),
                        _civilopedia.GetCardInfoById("1206"),
                        _civilopedia.GetCardInfoById("1207"),
                        _civilopedia.GetCardInfoById("1207"),
                        _civilopedia.GetCardInfoById("1208"),
                        _civilopedia.GetCardInfoById("1208"),
                    }.Shuffle();
                    #endregion
            }
            return new List<CardInfo>();
	    }

        public override List<CardInfo> GetMilitaryDeckForAge(GameRoom room, Age age)
        {
            _civilopedia = TtaCivilopedia.GetCivilopedia(room.GameRuleVersion);

            switch (age)
            {
                case Age.A:
                    #region Age A Military Deck
                    return new List<CardInfo>()
                    {
                _civilopedia.GetCardInfoById("4001"),
                _civilopedia.GetCardInfoById("4002"),
                _civilopedia.GetCardInfoById("4003"),
                _civilopedia.GetCardInfoById("4004"),
                _civilopedia.GetCardInfoById("4005"),
                _civilopedia.GetCardInfoById("4006"),
                _civilopedia.GetCardInfoById("4007"),
                _civilopedia.GetCardInfoById("4008"),
                _civilopedia.GetCardInfoById("4009"),
                _civilopedia.GetCardInfoById("4010"),
                    }.Shuffle();
                #endregion
                case Age.I:
                    #region Age I Military Deck
                    return new List<CardInfo>()
                    {
                _civilopedia.GetCardInfoById("4101"),
                _civilopedia.GetCardInfoById("4102"),
                _civilopedia.GetCardInfoById("4103"),
                _civilopedia.GetCardInfoById("4104"),
                _civilopedia.GetCardInfoById("4105"),
                _civilopedia.GetCardInfoById("4106"),
                _civilopedia.GetCardInfoById("4107"),
                _civilopedia.GetCardInfoById("4108"),
                _civilopedia.GetCardInfoById("4109"),
                _civilopedia.GetCardInfoById("4110"),
                _civilopedia.GetCardInfoById("4111"),
                _civilopedia.GetCardInfoById("4112"),
                _civilopedia.GetCardInfoById("4113"),
                _civilopedia.GetCardInfoById("4114"),
                _civilopedia.GetCardInfoById("4115"),



                    }.Shuffle();
                    #endregion
            }
            return new List<CardInfo>();
        }

        public override int DiscardAmount(GameRoom room)
        {
            return 5 - room.PlayerMax;
        }

	    public override BuildingType GetBuildingType(CardInfo card)
	    {
	        switch (card.CardType)
	        {
	            case CardType.ResourceTechFarm:
	                return BuildingType.Farm;
	            case CardType.ResourceTechMine:
	                return BuildingType.Mine;

	            case CardType.UrbanTechLab:
	                return BuildingType.Lab;
	            case CardType.UrbanTechArena:
	                return BuildingType.Arena;
	            case CardType.UrbanTechLibrary:
	                return BuildingType.Library;
	            case CardType.UrbanTechTemple:
	                return BuildingType.Temple;
	            case CardType.UrbanTechTheater:
	                return BuildingType.Theater;

	            case CardType.MilitaryTechAirForce:
	                return BuildingType.AirForce;
	            case CardType.MilitaryTechArtillery:
	                return BuildingType.Artillery;
	            case CardType.MilitaryTechCavalry:
	                return BuildingType.Cavalry;
	            case CardType.MilitaryTechInfantry:
	                return BuildingType.Infantry;
	            default:
	                return BuildingType.Unknown;
	        }
	    }
    }
}