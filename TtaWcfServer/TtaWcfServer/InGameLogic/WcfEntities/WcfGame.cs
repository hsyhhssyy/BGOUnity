using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TtaWcfServer.InGameLogic.ActionDefinition;
using TtaWcfServer.InGameLogic.Civilpedia;
using TtaWcfServer.InGameLogic.TtaEntities;
using TtaWcfServer.Util;

namespace TtaWcfServer.InGameLogic.WcfEntities
{
    [DataContract]
    public class WcfGame
    {
        public const int PlayerNumberObserver = -1;
        public const int PlayerNumberReferee = -2;
        public const int PlayerNumberNotPaticpated = -3;

        /// <summary>
        /// Player是座位号（0开始）
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <param name="manager"></param>
        public WcfGame(TtaGame game, int player, GameManager manager)
        {
            //-1是Observer

            //首先展示所有的元素
            CurrentAge = game.CurrentAge;
            CurrentRound = game.CurrentRound;

            CurrentEventAge = game.CurrentEventDeck.PeekCard().CardAge;
            CurrentEventCard = null;//CardInfo.UnknownMilitaryCard(game.CurrentEventDeck.PeekCard().CardAge); //TODO 贞德相关
            CurrentEventCount = CountAge(game.CurrentEventDeck);

            FutureEventAge = game.FutureEventDeck.PeekCard()?.CardAge??Age.A;
            FutureEventCount = CountAge(game.FutureEventDeck);

            CivilCardsRemain = game.CivilCardsDeck.Count;
            MilitaryCardsRemain = game.MilitaryCardsDeck.Count;

            CurrentPhase = game.CurrentPhase;
            MyPlayerIndex = player;
            CurrentPlayer = game.CurrentPlayer;

            SharedTactics = new List<CardInfo>();
            SharedTactics.AddRange(game.SharedTactics);

            CardRow=new List<WcfCardRowInfo>();
            for (int index = 0; index < game.CardRow.Count; index++)
            {
                var rowInfo = game.CardRow[index];
                if (rowInfo == null)
                {
                    WcfCardRowInfo info = new WcfCardRowInfo
                    {
                        Card = null,
                        AlreadyTaken = true,
                        CanPutBack=false,
                        CivilCount = 0
                    };
                    CardRow.Add(info);
                }
                else
                {
                    WcfCardRowInfo info = new WcfCardRowInfo
                    {
                        Card = rowInfo.Card,
                        AlreadyTaken = rowInfo.TakenBy != -1
                    };
                    info.CanPutBack = info.AlreadyTaken && rowInfo.TakenBy == player;
                    info.CivilCount = index < 5 ? 1 : (index < 9 ? 2 : 3);
                    CardRow.Add(info);
                }
            }

            PossibleActions = new List<PlayerAction>();
            if (player != PlayerNumberReferee && player != PlayerNumberObserver)
            {
                PossibleActions.AddRange(manager.GetPossibleActions(player));
            }

            //Board
            Boards = new List<WcfBoard>();
            for (int index = 0; index < game.Boards.Count; index++)
            {
                WcfBoard board = new WcfBoard(game, index, manager,
                    player != index && player != PlayerNumberReferee);
                Boards.Add(board);
            }
        }

        #region 要传递过去的内容

        [DataMember]
        public Age CurrentAge;
        [DataMember]
        public int CurrentRound;


        [DataMember]
        public Age CurrentEventAge;
        [DataMember]
        public CardInfo CurrentEventCard;
        [DataMember]
        public String CurrentEventCount;

        [DataMember]
        public Age FutureEventAge;
        [DataMember]
        public String FutureEventCount;

        [DataMember]
        public int CivilCardsRemain;
        [DataMember]
        public int MilitaryCardsRemain;

        [DataMember]
        public int MyPlayerIndex = -1;
        [DataMember]
        public int CurrentPlayer = -1;

        [DataMember]
        public TtaPhase CurrentPhase;

        [DataMember]
        public List<CardInfo> SharedTactics;

        [DataMember]
        public List<WcfCardRowInfo> CardRow;

        [DataMember]
        public List<PlayerAction> PossibleActions;

        [DataMember]
        public String Version = "2.0";

        [DataMember]
        public List<WcfBoard> Boards;

#endregion

        private String CountAge(List<CardInfo> infos)
        {
            int[] ages=new int[4];
            foreach (var cardInfo in infos)
            {
                ages[(int) cardInfo.CardAge]++;
            }
            return ages[0] + "+" + ages[1] + "+" + ages[2] + "+" + ages[3];
        }
    }
}