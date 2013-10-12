using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class FindBestStrategyForFirstGame
    {
        static void Run()
        {
            StrategyOptimizer.FindBestStrategyForGame(GameSets.FirstGame);
        }      

        static void FindAndCompareBestStrategy()
        {
            EvaulateBestStrategyForFirstGame();
            Run();

            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),                                   
        }

        static void EvaulateBestStrategyForFirstGame()
        {
            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),
            var player1 = new PlayerAction("Player 1", 1,
                new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Gold.card),
                    CardAcceptance.For(CardTypes.Market.card, gameState => Strategies.CountAllOwned(CardTypes.Market.card, gameState) < 1),
                    CardAcceptance.For(CardTypes.Duchy.card, gameState => Strategies.CountAllOwned(CardTypes.Duchy.card, gameState) < 2),
                    CardAcceptance.For(CardTypes.Militia.card, gameState => Strategies.CountAllOwned(CardTypes.Militia.card, gameState) < 2),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Estate.card, gameState => Strategies.CountAllOwned(CardTypes.Militia.card, gameState) < 1)
                    ));
            Program.ComparePlayers(player1, Strategies.BigMoneySimple.Player(2), showVerboseScore: true);
            Program.ComparePlayers(player1, Strategies.BigMoney.Player(2), showVerboseScore: true);
            Program.ComparePlayers(player1, Strategies.BigMoneySingleSmithy.Player(2), showVerboseScore: true);
        }        
    }
}
