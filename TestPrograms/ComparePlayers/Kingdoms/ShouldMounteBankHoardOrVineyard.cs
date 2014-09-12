using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Program.Kingdoms
{
    static class ShouldMounteBankHoardOrVineyard
    {        
        public static void Run(TestOutput testOutput)
        {
            GameConfig gameConfig = GameConfigBuilder.Create(
                StartingCardSplit.Split43,
                Cards.Bishop,
                Cards.FarmingVillage,
                Cards.GrandMarket,
                Cards.Hamlet,
                Cards.Hoard,
                Cards.Monument,
                Cards.Mountebank,
                Cards.PhilosophersStone,
                Cards.ScryingPool,
                Cards.Vineyard
                );

            testOutput.ComparePlayers(Strategies.MountebankMonumentHamletVineyard.Player(), Strategies.BigMoney.Player(), gameConfig);
            testOutput.ComparePlayers(Strategies.MountebankHoard.Player(), Strategies.BigMoney.Player(), gameConfig);
            testOutput.ComparePlayers(Strategies.MountebankMonumentHamletVineyard.Player(), Strategies.MountebankHoard.Player(), gameConfig);            
        }
    }
}
