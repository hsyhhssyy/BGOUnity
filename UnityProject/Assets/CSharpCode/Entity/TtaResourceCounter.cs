using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.GameLogic;
using Assets.CSharpCode.GameLogic.Effect;

namespace Assets.CSharpCode.Entity
{
    public class TtaResourceCounter
    {
        public TtaResourceCounter(GameLogicManager manager,TtaBoard board)
        {
            Manager = manager;
            Board = board;

            //以下这些元素不能根据面板算出，因此需要保存
            UncountableResourceCount.Add(ResourceType.Science, 0);
            UncountableResourceCount.Add(ResourceType.Culture, 0);

            UncountableResourceCount.Add(ResourceType.ResourceForMilitary, 0);
            UncountableResourceCount.Add(ResourceType.ResourceForWonderAndProduction, 0);

            UncountableResourceCount.Add(ResourceType.ScienceForMilitary, 0);
            UncountableResourceCount.Add(ResourceType.ScienceForSpecialTech, 0);

            UncountableResourceCount.Add(ResourceType.WhiteMarker, 0);
            UncountableResourceCount.Add(ResourceType.RedMarker, 0);

            UncountableResourceCount.Add(ResourceType.WorkerPool, 0);
        }

        public void Add(ResourceType type, int value)
        {
            this[type] = value;
        }

        public Dictionary<ResourceType, int> UncountableResourceCount = new Dictionary<ResourceType, int>();


        private TtaBoard Board;
        private GameLogicManager Manager;

        public int this[ResourceType type]
        {
            get
            {
                switch (type)
                {
                    case ResourceType.Science:
                    case ResourceType.Culture:
                    case ResourceType.ResourceForMilitary:
                    case ResourceType.ResourceForWonderAndProduction:
                    case ResourceType.ScienceForMilitary:
                    case ResourceType.ScienceForSpecialTech:
                    case ResourceType.RedMarker:
                    case ResourceType.WorkerPool:
                        return UncountableResourceCount[type];
                    case ResourceType.WhiteMarker:
                        //TODO 拉比的白点在这里直接算
                        return UncountableResourceCount[type];
                    default:
                        return ReturnResourceCount(type);
                }
            }
            set
            {
                switch (type)
                {
                    case ResourceType.Science:
                    case ResourceType.Culture:
                    case ResourceType.ResourceForMilitary:
                    case ResourceType.ResourceForWonderAndProduction:
                    case ResourceType.ScienceForMilitary:
                    case ResourceType.ScienceForSpecialTech:
                    case ResourceType.WhiteMarker:
                    case ResourceType.RedMarker:
                    case ResourceType.WorkerPool:
                        UncountableResourceCount[type] = value;
                        break;
                    default:
                        //Do nothing
                        break;
                }
            }
        }

        private int ReturnResourceCount(ResourceType type)
        {
            int resourceValue = 0;
            switch (type)
            {
                case ResourceType.WhiteMarkerMax:
                    resourceValue += Board.EffectPool.FilterEffect(CardEffectType.E100, 12).Sum(e => e.Data[1]);
                    break;
                case ResourceType.WhiteMarker:
                    resourceValue = UncountableResourceCount[ResourceType.WhiteMarker];
                    break;
                case ResourceType.RedMarkerMax:
                    resourceValue += Board.EffectPool.FilterEffect(CardEffectType.E100, 13).Sum(e => e.Data[1]);
                    break;
                case ResourceType.RedMarker:
                    resourceValue = UncountableResourceCount[ResourceType.RedMarker];
                    break;
                case ResourceType.Food:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.FoodIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Storage);
                    break;
                case ResourceType.FoodIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.FoodIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Worker);
                    break;
                case ResourceType.Resource:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.ResourceIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Storage);
                    break;
                case ResourceType.ResourceIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.ResourceIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Worker);
                    break;
                case ResourceType.HappyFace:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.HappyFace, Board,
                        (current, effect, cell) => current + effect * cell.Worker);
                    break;
                case ResourceType.ScienceIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.ScienceIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Worker);
                    break;
                case ResourceType.CultureIncrement:
                    resourceValue = AggregateCountResourceOnBuildingCell(ResourceType.CultureIncrement, Board,
                        (current, effect, cell) => current + effect * cell.Worker);
                    break;
                case ResourceType.YellowMarker:
                    var workerUsed = Board.AggregateOnBuildingCell(0, (current, cell) => current + cell.Worker);
                    workerUsed += UncountableResourceCount[ResourceType.WorkerPool];
                    resourceValue = Board.InitialYellowMarkerCount - workerUsed;

                    break;
                case ResourceType.BlueMarker:
                    var blueMarkerUsed = Board.AggregateOnBuildingCell(0, (current, cell) => current + cell.Storage);
                    blueMarkerUsed += Board.ConstructingWonderSteps.Count;

                    resourceValue = Board.InitialBlueMarkerCount - blueMarkerUsed;
                    break;
                case ResourceType.Science:
                    resourceValue = UncountableResourceCount[ResourceType.Science];
                    break;
            }

            return resourceValue;

        }

        private int AggregateCountResourceOnBuildingCell(
            ResourceType type, TtaBoard board, Func<int, int, BuildingCell, int> aggregate)
        {
            int count = 0;
            foreach (var buildingPair in board.Buildings)
            {
                foreach (var cellPair in buildingPair.Value)
                {
                    var cell = cellPair.Value;

                    var e =
                        cell.Card
                            .SustainedEffects.FilterEffect(CardEffectType.E100, (int)type)
                            .FirstOrDefault();
                    if (e != null)
                    {
                        int value = e.Data[1];
                        count = aggregate(count, value, cell);
                    }
                }
            }

            return count;
        }

    }
}
