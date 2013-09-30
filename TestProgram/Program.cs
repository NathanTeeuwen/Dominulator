using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    class Program    
    {        
        static void Main()
        {                        
            CompareStrategyVsAllKnownStrategies(Strategies.IllgottengainsMoneylender.Player(1));
        }

        static void CompareStrategyVsAllKnownStrategies(PlayerAction playerAction)
        {
            var resultList = new List<System.Tuple<string, double>>();

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var type = assembly.GetType("Program.Strategies");
            foreach (Type innerType in type.GetNestedTypes())
            {
                if (!innerType.IsClass)                
                    continue;                                    

                System.Reflection.MethodInfo playerMethodInfo = innerType.GetMethod("Player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (playerMethodInfo == null)                
                    continue;

                if (playerMethodInfo.ContainsGenericParameters)
                    continue;

                PlayerAction otherPlayerAction = playerMethodInfo.Invoke(null, new object[]{2}) as PlayerAction;
                if (otherPlayerAction == null)
                    continue;

                double percentDiff = ComparePlayers(playerAction, otherPlayerAction);

                resultList.Add( new System.Tuple<string,double>(otherPlayerAction.PlayerName, percentDiff));
            }            

            foreach(var result in resultList.OrderBy(t => t.Item2))
            {
                if (result.Item1 == playerAction.name)
                    System.Console.Write("=====>");
                System.Console.WriteLine("{0:F1}% difference for {1}", -result.Item2, result.Item1);
            }
        }

        static void FindAndCompareBestStrategy()
        {
            EvaulateBestStrategyForFirstGame();
            FindBestStrategyForFirstGame();

            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),                                   
        }

        static void CompareGame1()
        {
            // possible strategies choen for Shanty Town, Swindler, lookout, spicemerchant, nomad camp, horse tradres, navigator, laboratory, warehouse, 
            ComparePlayers(Strategies.NomadCampLaboratorySpiceMerchantWarehouse.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.LaboratorySpiceMerchantWarehouse.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.NomadCampLaboratorySpiceMerchantWarehouse.Player(1), Strategies.LaboratorySpiceMerchantWarehouse.Player(2));
            ComparePlayers(Strategies.NomadCampLaboratorySpiceMerchantWarehouse.Player(1), Strategies.BigMoneyWithCard<CardTypes.Laboratory>.Player(2, cardCount: 3));
        }

        static void CompareGame2()
        {
            // possible strategies for armory, conspirator, forager, great hall, pawn, governor, mining village, pawn, junk dealdre
            ComparePlayers(Strategies.BigMoneySingleSmithy.Player(1), Strategies.ArmoryConspiratorForagerGreatHall.Player(2), useShelters: true);
            ComparePlayers(Strategies.BigMoneyDoubleJack.Player(1), Strategies.ArmoryConspiratorForagerGreatHall.Player(2), useShelters: true);
            ComparePlayers(Strategies.BigMoneySingleSmithy.Player(1), Strategies.GovernorJunkdealer.Player(2), useShelters: true);
            ComparePlayers(Strategies.ArmoryConspiratorForagerGreatHall.Player(1), Strategies.GovernorJunkdealer.Player(2), useShelters: true);
        }

        static void PlayRemake()
        {
            var player1 = new PlayerAction("Player 1", 1,
                Strategies.BigMoneyWithCard<CardTypes.Remake>.Player(1).purchaseOrder,
                actionOrder: new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Remake>(gameState => Strategies.CountInHand<CardTypes.Copper>(gameState) + Strategies.CountInHand<CardTypes.Estate>(gameState) >= 2)),
                trashOrder: new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Copper>()));

            ComparePlayers(player1, Strategies.BigMoney.Player(2));            
        }

        static void EvaulateBestStrategyForFirstGame()
        {
            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),
            var player1 = new PlayerAction("Player 1", 1, 
                new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Gold>(),
                    CardAcceptance.For<CardTypes.Market>(gameState => Strategies.CountAllOwned<CardTypes.Market>(gameState) < 1),
                    CardAcceptance.For<CardTypes.Duchy>(gameState => Strategies.CountAllOwned<CardTypes.Duchy>(gameState) < 2),
                    CardAcceptance.For<CardTypes.Militia>(gameState => Strategies.CountAllOwned<CardTypes.Militia>(gameState) < 2),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => Strategies.CountAllOwned<CardTypes.Militia>(gameState) < 1)
                    ));
            ComparePlayers(player1, Strategies.BigMoneySimple.Player(2), showVerboseScore: true);
            ComparePlayers(player1, Strategies.BigMoney.Player(2), showVerboseScore:true);
            ComparePlayers(player1, Strategies.BigMoneySingleSmithy.Player(2), showVerboseScore: true);            
        }

        static void FindBestStrategyForFirstGame()        
        {
            StrategyOptimizer.FindBestStrategyForGame(GameSets.FirstGame);
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
            //ComparePlayers(Strategies.FishingVillageChapelPoorHouseTalisman.Player(1), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.FishingVillageChapelPoorHouse.Player(1), Strategies.BigMoney.Player(2));            
            ComparePlayers(Strategies.GardensBeggarIronworks.Player(1), Strategies.BigMoney.Player(2), numberOfGames: 10000);
        }

        static void RebuildResults()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8391.0
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoney.Player(2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Wharf>.Player(2, cardCount:2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Mountebank>.Player(2, cardCount: 2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(2, cardCount: 2));
            ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard<CardTypes.YoungWitch>.Player(2, cardCount: 2));
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
                ComparePlayers(Strategies.FollowersTest.TestPlayer(1, i), Strategies.BigMoney.Player(2), showCompactScore: true);
            }
        }
        
        static IGameLog GetGameLogForIteration(int gameCount)
        {
            return new HumanReadableGameLog("..\\..\\Results\\GameLog" + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt");
        }

        public delegate IGameLog CreateGameLog();

        public static double ComparePlayers(
            PlayerAction player1, 
            PlayerAction player2, 
            bool useShelters = false, 
            bool firstPlayerAdvantage = false, 
            bool showVerboseScore = true,
            bool showCompactScore = false, 
            bool showDistribution = false,
            bool shouldParallel = true,
            bool showPlayer2Wins = false,
            int numberOfGames = 1000, 
            int logGameCount = 100,
            CreateGameLog createGameLog = null,
            IEnumerable<CardCountPair> startingDeck = null)
        {            
            PlayerAction[] players = new PlayerAction[] { player1, player2 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var countbyBucket = new CountByBucket();
             
            Action<int> loopBody = delegate(int gameCount)                    
            {
                using (IGameLog gameLog = createGameLog != null ? createGameLog() :
                                          gameCount < logGameCount ? GetGameLogForIteration(gameCount) : 
                                          new EmptyGameLog())                
                {
                    // swap order every other game
                    bool swappedOrder = !firstPlayerAdvantage && (gameCount % 2 == 1);
                    PlayerAction startPlayer = !swappedOrder ? player1 : player2;
                    PlayerAction otherPlayer = !swappedOrder ? player2 : player1;

                    Random random = new Random(gameCount);

                    var gameConfig = new GameConfig(
                        useShelters,                        
                        useColonyAndPlatinum: false,
                        supplyPiles: PlayerAction.GetKingdomCards(startPlayer, otherPlayer),
                        startingDeck : startingDeck);

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
                            
                            if (winningPlayerIndex == 1 && showPlayer2Wins)
                            {
                                System.Console.WriteLine("Player 2 won game {0}. ", gameCount);
                            }
                        }
                        else
                        {
                            tieCount++;
                        }
                    }
                }
            };

            if (shouldParallel)
            {
                Parallel.ForEach(Enumerable.Range(0, numberOfGames), loopBody);
            }
            else
            {
                for (int gameCount = 0; gameCount < numberOfGames; ++gameCount)
                    loopBody(gameCount);
            }

            if (showVerboseScore) 
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

            if (showCompactScore)
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

            double diff = PlayerWinPercent(0, winnerCount, numberOfGames) - PlayerWinPercent(1, winnerCount, numberOfGames);
            return diff;
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
    }            
}
