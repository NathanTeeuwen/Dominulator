using System;
using System.Linq;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    static class StrategyOptimizer
    {
        public static void FindBestStrategyForGame(GameConfig gameConfig)
        {
            var initialDescription = new PickByPriorityDescription(new CardAcceptanceDescription[]
            {
                new CardAcceptanceDescription( CardTypes.Province.card, new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)}),
                new CardAcceptanceDescription( CardTypes.Gold.card, new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)}),
                new CardAcceptanceDescription( CardTypes.Silver.card, new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)})
            });

            Random random = new Random();

            Card[] supplyCards = gameConfig.GetSupplyPiles(2, random).Select(pile => pile.ProtoTypeCard).ToArray();

            var initialPopulation = Enumerable.Range(0, 10).Select(index => initialDescription).ToArray();
            var algorithm = new GeneticAlgorithm<PickByPriorityDescription, MutatePickByPriorityDescription, CompareStrategies>(
                initialPopulation,
                new MutatePickByPriorityDescription(random, supplyCards),
                new CompareStrategies(),
                new Random());

            for (int i = 0; i < 1000; ++i)
            {
                System.Console.WriteLine("Generation {0}", i);
                System.Console.WriteLine("==============", i);
                for (int j = 0; j < 10; ++j)
                {
                    algorithm.currentMembers[j].Write(System.Console.Out);
                    System.Console.WriteLine();
                }

                algorithm.RunOneGeneration();

                System.Console.WriteLine();
            }
        }                                  

    }
}
