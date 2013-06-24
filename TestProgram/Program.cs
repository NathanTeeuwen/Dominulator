using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class Program
    {

        static void Main()
        {
            DarkAgesBigMoney();
        }

        static void DarkAgesBigMoney()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=6281.0
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Catacombs>.Player(1, 2), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Count>.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.HuntingGrounds>.Player(1), Strategies.BigMoney.Player(2));
        }

        static void HighestWinRateVsBigMoney()
        {
            // goal is to find a strategy that always beats big money.  Haven't found it yet.
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8580.0
            ComparePlayers(Strategies.FishingVillageChapelPoorHouseTalisman.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.FishingVillageChapelPoorHouse.Player(1), Strategies.BigMoney.Player(2));
        }

        static void RebuildResults()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8391.0
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Wharf>.Player(2, 2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Mountebank>.Player(2, 2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(2, 2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.YoungWitch>.Player(2, 2));
        }

        static void GuildsSimulatorResults()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8461.0
            ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Soothsayer>.Player(1), Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(2));
        }

        static void FeodumVsDuke()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=7476.msg212934#msg212934
            ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.FeodumDevelop.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(1), Strategies.FeodumDevelop.Player(2));
        }

        static void FollowersTest()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=6623.0
            System.Console.WriteLine("Followers Cost, Player 1 Win %, Player 2 Win%, Tie%");
            for (int i = 0; i < 16; ++i)
            {
                System.Console.Write("{0}, ", i);
                ComparePlayers(Strategies.FollowersTest.Player(1, i), Strategies.BigMoney.Player(2), showCompactScore: true);
            }
        }

        /*
        static void Main()
        {
            
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCard<CardTypes.Cartographer>.Player(2));
            
            //ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneyDelayed.Player(2));
            
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCard<CardTypes.Smithy>.Player(2));
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCardCartographer<CardTypes.Smithy>.Player(2));

            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCard<CardTypes.Rabble>.Player(2));
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCardCartographer<CardTypes.Rabble>.Player(2));
            
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCard<CardTypes.Torturer>.Player(2));            
            ComparePlayers(Strategies.CaravanBridgeDukeCartographer.Player(1), Strategies.BigMoneySingleCardCartographer<CardTypes.Torturer>.Player(2));                       
        }*/

        static IGameLog GetGameLogForIteration(int gameCount)
        {
            return new HumanReadableGameLog("..\\..\\Results\\GameLog" + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt");
        }


        static void ComparePlayers(PlayerAction player1, PlayerAction player2, bool useShelters = false, bool firstPlayerAdvantage = false, bool showCompactScore = false, bool showDistribution = false)
        {
            int numberOfGames = 10000;

            PlayerAction[] players = new PlayerAction[] { player1, player2 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var countbyBucket = new CountByBucket();

            //for (int gameCount = 0; gameCount < numberOfGames; ++gameCount)

            Parallel.ForEach(Enumerable.Range(0, numberOfGames),
                delegate(int gameCount)
                {
                    using (IGameLog gameLog = gameCount < 100 ? GetGameLogForIteration(gameCount) : new EmptyGameLog())
                    //using (IGameLog gameLog = new HumanReadableGameLog("..\\..\\Results\\GameLog." + gameCount ) )
                    {
                        // swap order every other game
                        bool swappedOrder = !firstPlayerAdvantage && (gameCount % 2 == 1);
                        PlayerAction startPlayer = !swappedOrder ? player1 : player2;
                        PlayerAction otherPlayer = !swappedOrder ? player2 : player1;

                        Random random = new Random(gameCount);

                        var gameConfig = new GameConfig(
                            useShelters,
                            parameter: 0,
                            useColonyAndPlatinum: false,
                            supplyPiles: GetCardSet(startPlayer, otherPlayer));

                        GameState gameState = new GameState(
                            gameLog,
                            new PlayerAction[] { startPlayer, otherPlayer },
                            gameConfig,
                            random);

                        gameState.PlayGameToEnd();

                        PlayerState[] winners = gameState.WinningPlayers;

                        int startPlayerScore = gameState.players[0].TotalScore();
                        int otherPlayerScore = gameState.players[1].TotalScore();
                        int scoreDifference = startPlayerScore - otherPlayerScore;
                        if (swappedOrder)
                            scoreDifference = -scoreDifference;

                        lock (winnerCount)
                        {
                            countbyBucket.AddOneToBucket(scoreDifference);
                            if (winners.Length == 1)
                            {
                                int winningPlayerIndex = ((PlayerAction)winners[0].Actions).playerIndex - 1;
                                winnerCount[winningPlayerIndex]++;
                            }
                            else
                            {
                                tieCount++;
                            }
                        }
                    }
                }
            );

            if (!showCompactScore)
            {
                for (int index = 0; index < winnerCount.Length; ++index)
                {
                    System.Console.WriteLine("{1}% win for {0}", players[index].name, PlayerWinPercent(index, winnerCount, numberOfGames));
                }
                if (tieCount > 0)
                {
                    System.Console.WriteLine("{0}% there is a tie.", TiePercent(tieCount, numberOfGames));
                }
                System.Console.WriteLine();
            }
            else
            {
                System.Console.WriteLine("{0}, {1}, {2}",
                    PlayerWinPercent(0, winnerCount, numberOfGames),
                    PlayerWinPercent(1, winnerCount, numberOfGames),
                    TiePercent(tieCount, numberOfGames));
            }

            if (showDistribution)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Player 1 Score Delta distribution");
                System.Console.WriteLine("=================================");
                countbyBucket.WriteBuckets(System.Console.Out);
            }
        }

        static double TiePercent(int tieCount, int numberOfGames)
        {
            return tieCount / (double)numberOfGames * 100;
        }

        static double PlayerWinPercent(int player, int[] winnerCount, int numberOfGames)
        {
            return winnerCount[player] / (double)numberOfGames * 100;
        }

        class CountByBucket
        {
            int totalCount = 0;
            Dictionary<int, int> mapBucketToCount = new Dictionary<int, int>();

            public void AddOneToBucket(int bucket)
            {
                this.totalCount++;

                int value = 0;
                this.mapBucketToCount.TryGetValue(bucket, out value);
                value += 1;
                this.mapBucketToCount[bucket] = value;
            }

            public void WriteBuckets(System.IO.TextWriter writer)
            {
                foreach (var pair in this.mapBucketToCount.OrderByDescending(keyValuePair => keyValuePair.Key))
                {
                    writer.WriteLine("{0} points:   {2}% = {1}", pair.Key, pair.Value, (double)pair.Value / this.totalCount * 100);
                }
            }
        }

        static CardPickByPriority SimplePurchaseOrder3()
        {
            return new CardPickByPriority(
                       CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                       CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                       CardAcceptance.For<CardTypes.Gold>(),
                       CardAcceptance.For<CardTypes.Silver>());

        }

        static CardPickByPriority SimplePurchaseOrder2()
        {
            return new CardPickByPriority(
                       CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                       CardAcceptance.For<CardTypes.Gold>(),
                       CardAcceptance.For<CardTypes.Silver>());
        }

        static CardPickByPriority SimplePurchaseOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For<CardTypes.Province>(),
                CardAcceptance.For<CardTypes.Gold>(),
                CardAcceptance.For<CardTypes.Duchy>(),
                CardAcceptance.For<CardTypes.Silver>(),
                CardAcceptance.For<CardTypes.Estate>());
        }

        static Card[] GetCardSet(PlayerAction playerAction1, PlayerAction playerAction2)
        {
            var cards = new HashSet<Card>(new CompareCardByType());

            AddCards(cards, playerAction1.actionOrder);
            AddCards(cards, playerAction1.purchaseOrder);
            AddCards(cards, playerAction1.gainOrder);
            AddCards(cards, playerAction2.actionOrder);
            AddCards(cards, playerAction2.purchaseOrder);
            AddCards(cards, playerAction2.gainOrder);

            cards.Remove(new CardTypes.Platinum());
            cards.Remove(new CardTypes.Gold());
            cards.Remove(new CardTypes.Silver());
            cards.Remove(new CardTypes.Copper());
            cards.Remove(new CardTypes.Colony());
            cards.Remove(new CardTypes.Province());
            cards.Remove(new CardTypes.Duchy());
            cards.Remove(new CardTypes.Estate());
            cards.Remove(new CardTypes.Curse());
            cards.Remove(new CardTypes.Potion());
            cards.Remove(new CardTypes.RuinedLibrary());
            cards.Remove(new CardTypes.RuinedVillage());
            cards.Remove(new CardTypes.RuinedMarket());
            cards.Remove(new CardTypes.AbandonedMine());

            return cards.ToArray();
        }

        static void AddCards(HashSet<Card> cardSet, ICardPicker matchingCards)
        {
            foreach (Card card in matchingCards.GetNeededCards())
            {
                cardSet.Add(card);
            }
        }
    }            
}
