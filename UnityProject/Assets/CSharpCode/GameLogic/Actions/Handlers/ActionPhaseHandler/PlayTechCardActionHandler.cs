using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions.Handlers
{
    class PlayTechCardActionHandler : ActionHandler
    {
        public PlayTechCardActionHandler(GameLogicManager manager) : base(manager)
        {
        }

        public override List<PlayerAction> GenerateAction(int playerNo)
        {

            if (Manager.CurrentGame.CurrentPhase != TtaPhase.ActionPhase ||
                Manager.CurrentGame.CurrentPlayer != playerNo)
            {
                return null;
            }


            var board = Manager.CurrentGame.Boards[playerNo];
            var whiteMarker = board.Resource[ResourceType.WhiteMarker];
            if (whiteMarker <= 0)
            {
                return null;
            }

            var result = new List<PlayerAction>();
            var ruleBook = Manager.Civilopedia.GetRuleBook();
            var science = board.Resource[ResourceType.Science];
            var scienceForMili = board.Resource[ResourceType.ScienceForMilitary];
            var scienceForSpec = board.Resource[ResourceType.ScienceForSpecialTech];

            foreach (var handInfo in board.CivilCards)
            {
                var actionType = CanDevelopThisTechCard(handInfo, ruleBook, board, science, scienceForSpec, scienceForMili);

                if (actionType== PlayerActionType.DevelopTechCard
                   )
                {
                    PlayerAction action = new PlayerAction { ActionType = PlayerActionType.DevelopTechCard };
                    action.Data[0] = handInfo.Card;
                    action.Data[1] = handInfo.Card.ResearchCost[0];
                    result.Add(action);
                }else if (actionType == PlayerActionType.Revolution)
                {
                    PlayerAction action = new PlayerAction { ActionType = PlayerActionType.Revolution };
                    action.Data[0] = handInfo.Card;
                    action.Data[1] = handInfo.Card.ResearchCost[1];
                    result.Add(action);
                }
            }

            return result;
        }

        public static PlayerActionType CanDevelopThisTechCard(HandCardInfo handInfo, TtaRuleBook ruleBook, TtaBoard board, int science,
            int scienceForSpec, int scienceForMili)
        {
            var card = handInfo.Card;
            if (ruleBook.IsTechCard(card))
            {
                //注意检查是否已经拥有

                if (card.CardType == CardType.Government)
                {
                    if (card == board.Government)
                    {
                        return PlayerActionType.Unknown;
                    }
                    if (science >= card.ResearchCost[0])
                    {
                        return PlayerActionType.DevelopTechCard;
                    }

                    if (science >= card.ResearchCost[1])
                    {
                        return PlayerActionType.Revolution;
                    }
                }
                else if (ruleBook.IsSpecialTech(card))
                {
                    if (board.SpecialTechs.Contains(card))
                    {
                        //TODO 不能更换为更早的特殊科技
                        return PlayerActionType.Unknown;
                    }
                    if (science + scienceForSpec >= card.ResearchCost[0])
                    {
                        return PlayerActionType.DevelopTechCard;
                    }
                }
                else
                {
                    var hasBuilding = board.AggregateOnBuildingCell(false, (b, cell) =>
                    {
                        if (cell.Card == card)
                        {
                            return true;
                        }
                        return b;
                    });
                    if (hasBuilding)
                    {
                        return PlayerActionType.Unknown;
                    }
                    if (ruleBook.IsMilitary(card))
                    {
                        if (science + scienceForMili >= card.ResearchCost[0])
                        {
                            return PlayerActionType.DevelopTechCard;
                        }
                    }
                    else if (science >= card.ResearchCost[0])
                    {
                        return PlayerActionType.DevelopTechCard;
                    }
                }
            }
            return PlayerActionType.Unknown;
        }

        public override ActionResponse PerfromAction(int playerNo, PlayerAction action, Dictionary<string, object> data)
        {
            if (action.ActionType == PlayerActionType.DevelopTechCard ||
                action.ActionType == PlayerActionType.Revolution)
            {
                var board = Manager.CurrentGame.Boards[playerNo];
                var response = new ActionResponse();
                var ruleBook = Manager.Civilopedia.GetRuleBook();
                response.Type = ActionResponseType.ChangeList;

                var card = (CardInfo) action.Data[0];

                int cost0 = (int)action.Data[1];
                int originalScience = board.Resource[ResourceType.Science];
                var originalScienceForMili = board.Resource[ResourceType.ScienceForMilitary];
                var originalScienceForSpec = board.Resource[ResourceType.ScienceForSpecialTech];

                int destScience = originalScience;
                int destScienceForMili = originalScienceForMili;
                int destScienceForSpec = originalScienceForSpec;

                //计算科技消耗的方式（优先消耗专用资源）
                if (ruleBook.IsMilitary(card))
                {
                    if (originalScienceForMili < cost0)
                    {
                        cost0 -= originalScienceForMili;
                        destScienceForMili = 0;
                    }
                    else
                    {
                        destScienceForMili -= cost0;
                        cost0 = 0;
                    }
                }
                if (ruleBook.IsSpecialTech(card))
                {
                    if (originalScienceForSpec < cost0)
                    {
                        cost0 -= originalScienceForSpec;
                        destScienceForSpec = 0;
                    }
                    else
                    {
                        destScienceForSpec -= cost0;
                        cost0 = 0;
                    }
                }

                destScience = destScience - cost0;

                if (destScienceForSpec != originalScienceForSpec)
                {
                    response.Changes.Add(GameMove.Resource(ResourceType.ScienceForSpecialTech,
                        originalScienceForSpec, destScienceForSpec));
                    board.Resource[ResourceType.ScienceForSpecialTech] = destScienceForSpec;
                }

                if (destScienceForMili != originalScienceForMili)
                {
                    response.Changes.Add(GameMove.Resource(ResourceType.ScienceForMilitary, 
                        originalScienceForMili, destScienceForMili));
                    board.Resource[ResourceType.ScienceForMilitary] = destScienceForMili;
                }
                if (destScience != originalScience)
                {
                    response.Changes.Add(GameMove.Resource(ResourceType.Science,
                        originalScience, destScience));
                    board.Resource[ResourceType.Science] = destScience;
                }

                if (card.CardType == CardType.Government)
                {
                    board.Government = card;
                    response.Changes.Add(GameMove.Resource(ResourceType.Science,
                        originalScience, destScience));
                }

                //从手牌里找到最早的一张同名牌
                var infoList=board.CivilCards.Where(info=>info.Card==card).ToList();
                infoList.Sort((a,b)=>a.TurnTaken.CompareTo(b.TurnTaken));
                var handInfo = infoList.FirstOrDefault();
                
                response.Changes.Add(GameMove.CardUsed(handInfo));

                //打出来这张牌

                if (card.CardType == CardType.Government)
                {
                    if (board.Government != null)
                    {
                        board.EffectPool.RemoveCardInfo(board.Government);
                    }
                    board.Government = card;
                    board.EffectPool.AddCardInfo(card);
                    response.Changes.Add(GameMove.ReplaceGovernment(card));
                }
                else if (ruleBook.IsSpecialTech(card))
                {
                    //判断是否是重复的Effect
                    var oldSpCard = board.SpecialTechs.FirstOrDefault(spCard => spCard.CardType == card.CardType);

                    if (oldSpCard != null)
                    {
                        board.EffectPool.RemoveCardInfo(oldSpCard);
                        board.SpecialTechs.Remove(oldSpCard);
                        response.Changes.Add(GameMove.ReplaceSpecialTech(oldSpCard,card));
                    }
                    else
                    {
                        response.Changes.Add(GameMove.DevelopSpecialTech(card));
                    }
                    board.SpecialTechs.Add(card);
                    board.EffectPool.AddCardInfo(card);
                }
                else
                {
                    var cell = new BuildingCell
                    {
                        Card = card,
                        Storage = 0,
                        Worker = 0
                    };

                    var buildingType=ruleBook.GetBuildingType(card);
                    if (!board.Buildings.ContainsKey(buildingType))
                    {
                        board.Buildings.Add(buildingType,new Dictionary<Age, BuildingCell>());
                    }
                    var cellDict = board.Buildings[buildingType];
                    cellDict.Add(card.CardAge,cell);

                    response.Changes.Add(GameMove.DevelopBuilding(card));
                }

                //消耗1白
                var originalWhite = board.Resource[ResourceType.WhiteMarker];
                board.Resource[ResourceType.WhiteMarker] = originalWhite - 1;
                response.Changes.Add(GameMove.Resource(ResourceType.WhiteMarker, originalWhite, originalWhite - 1));


                return response;
            }
            return null;
        }
    }
}
