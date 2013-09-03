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
            CompareGame2();
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
            var initialDescription = new PickByPriorityDescription( new CardAcceptanceDescription[]
            {
                new CardAcceptanceDescription( new CardTypes.Province(), new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)}),
                new CardAcceptanceDescription( new CardTypes.Gold(), new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)}),
                new CardAcceptanceDescription( new CardTypes.Silver(), new MatchDescription[] { new MatchDescription( null, CountSource.None, Comparison.None, 0)})
            });

            Random random = new Random();

            Card[] supplyCards = GameSets.FirstGame.GetSupplyPiles(2, random).Select(pile => pile.ProtoTypeCard).ToArray();

            var initialPopulation = Enumerable.Range(0, 10).Select( index => initialDescription).ToArray();
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

        delegate bool ApplyMutation(List<CardAcceptanceDescription> descripton);

        class MutatePickByPriorityDescription
            : ISpecidesMutator<PickByPriorityDescription>
        {
            ApplyMutation[] mutators;
            Random random;
            Card[] availableCards;

            public MutatePickByPriorityDescription(Random random, Card[] availableCards)
            {
                this.random = random;
                this.availableCards = availableCards;
                this.mutators = CreateMutators();
            }

            public PickByPriorityDescription Mutate(PickByPriorityDescription member)
            {
                List<CardAcceptanceDescription> descriptions = new List<CardAcceptanceDescription>(member.descriptions);

                bool applied = false;
                while (!applied)
                {
                    var mutator = this.mutators[this.random.Next(this.mutators.Length)];
                    applied = mutator(descriptions);
                }

                return new PickByPriorityDescription(descriptions.ToArray());
            }

            ApplyMutation[] CreateMutators()
            {
                return new ApplyMutation[]
                {
                    this.ApplyAddNewCardAcceptance,
                    this.ApplyRemoveCardAcceptance,
                    this.ApplyModifyCardAcceptanceCount,
                    this.ApplySwapOrderCardAcceptance,
                    //this.ApplyAddNewUniqueCardAcceptance
                };
            }            

            private Card PickRandomCardFromSupply(Card[] excluded)
            {                
                for (int i = 0; i < 3; ++i)
                {
                    Card result = this.availableCards[this.random.Next(this.availableCards.Length)];
                    if (!DoesSetInclude(excluded, result))
                        return result;
                }

                return null;
            }

            private bool DoesSetInclude(Card[] cards, Card test)
            {
                foreach (Card card in cards)
                {
                    if (card.Equals(test))
                        return true;
                }

                return false;
            }

            private bool ApplyAddNewUniqueCardAcceptance(List<CardAcceptanceDescription> descriptions)
            {
                Card card = this.PickRandomCardFromSupply(descriptions.Select(descr => descr.card).ToArray());
                if (card == null)
                    return false;
                int insertLocation = FindLocationByCost(descriptions, card);
                descriptions.Insert(insertLocation, new CardAcceptanceDescription(card, new MatchDescription[] { new MatchDescription(null, CountSource.AllOwned, Comparison.LessThan, 10) }));

                return true;
            }

            private bool ApplyAddNewCardAcceptance(List<CardAcceptanceDescription> descriptions)
            {
                Card card = this.PickRandomCardFromSupply(new Card[0]{});            
                if (card == null)
                    return false;
                
                int insertLocation = FindLocationByCost(descriptions, card);
                
                if (this.random.Next(2) == 0)
                {
                    insertLocation += this.random.Next(2)+2;
                }
                else
                {
                    insertLocation -= this.random.Next(2)+2;
                }
                //int insertLocation = this.random.Next(descriptions.Count());

                insertLocation = Math.Max(0, insertLocation);
                insertLocation = Math.Min(descriptions.Count, insertLocation);

                if (insertLocation > 0 && descriptions[insertLocation - 1].card.Equals(card))
                    return false;

                if (insertLocation < descriptions.Count-1 && descriptions[insertLocation +1].card.Equals(card))
                    return false;
                                
                descriptions.Insert(insertLocation, new CardAcceptanceDescription(card, new MatchDescription[] { new MatchDescription(null, CountSource.AllOwned, Comparison.LessThan, 1) }));

                return true;
            }

            private int FindLocationByCost(List<CardAcceptanceDescription> descriptions, Card card)
            {
                int insertLocation = 0;

                while (insertLocation < descriptions.Count)
                {                    
                    if (descriptions[insertLocation].matchDescriptions[0].countSource != CountSource.None ||
                        descriptions[insertLocation].card.DefaultCoinCost > card.DefaultCoinCost)
                    {
                        insertLocation++;
                    }
                    else
                        break;
                }

                return insertLocation;
            }

            private bool ApplyRemoveCardAcceptance(List<CardAcceptanceDescription> descriptions)
            {
                if (descriptions.Count <= 3)
                    return false;

                int removeLocation = this.random.Next(descriptions.Count);

                if (descriptions[removeLocation].matchDescriptions[0].countSource == CountSource.None)
                    return false;

                descriptions.RemoveAt(removeLocation);
                return true;
            }

            private bool ApplyModifyCardAcceptanceCount(List<CardAcceptanceDescription> descriptions)
            {
                int descriptionIndex = this.random.Next(descriptions.Count);
                
                var description = descriptions[descriptionIndex];

                int threshhold = description.matchDescriptions[0].countThreshHold;

                if (threshhold == 0)
                {
                    return false;
                }


                bool shouldIncrement = this.random.Next(2) == 0 || threshhold == 1;

                if (shouldIncrement)
                {
                    threshhold++;
                }
                else
                {
                    threshhold--;
                }

                var newDescription = description.Clone();
                newDescription.matchDescriptions[0].countThreshHold = threshhold;
                descriptions[descriptionIndex] = newDescription;

                return true;
            }

            private bool ApplySwapOrderCardAcceptance(List<CardAcceptanceDescription> descriptions)
            {
                if (descriptions.Count <= 1)
                    return false;

                int swapFirstIndex = this.random.Next(descriptions.Count - 1);
                int nextSwapIndex = swapFirstIndex + 1;

                var temp = descriptions[swapFirstIndex];
                descriptions[swapFirstIndex] = descriptions[nextSwapIndex];
                descriptions[nextSwapIndex] = temp;

                return true;
            }
        }

        class CompareStrategies
            : IScoreSpecies<PickByPriorityDescription>
        {
            public double Compare(PickByPriorityDescription left, PickByPriorityDescription right)
            {
                //System.Console.WriteLine("Comparing: ");
                //left.Write(System.Console.Out);
                //System.Console.WriteLine("");
                //right.Write(System.Console.Out);
                //System.Console.WriteLine("");
                PlayerAction leftPlayer = new PlayerAction("Player1", 1, left.ToCardPicker());
                PlayerAction rightPlayer = new PlayerAction("Player2", 2, right.ToCardPicker());
                return Program.ComparePlayers(leftPlayer, rightPlayer, numberOfGames:33);
            }            
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
        
        static IGameLog GetGameLogForIteration(int gameCount)
        {
            return new HumanReadableGameLog("..\\..\\Results\\GameLog" + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt");
        }

        static double ComparePlayers(
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
            int logGameCount = 100)
        {            
            PlayerAction[] players = new PlayerAction[] { player1, player2 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var countbyBucket = new CountByBucket();
             
            Action<int> loopBody = delegate(int gameCount)                    
            {
                using (IGameLog gameLog = gameCount < logGameCount || gameCount == 203 ? GetGameLogForIteration(gameCount) : new EmptyGameLog())                
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
                        supplyPiles: GetKingdomCards(startPlayer, otherPlayer));

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

        static Card[] GetKingdomCards(PlayerAction playerAction1, PlayerAction playerAction2)
        {
            var cards = new HashSet<Card>(new CompareCardByType());

            AddCards(cards, playerAction1.actionOrder);
            AddCards(cards, playerAction1.purchaseOrder);
            AddCards(cards, playerAction1.gainOrder);
            AddCards(cards, playerAction2.actionOrder);
            AddCards(cards, playerAction2.purchaseOrder);
            AddCards(cards, playerAction2.gainOrder);

            var cardsToRemove = new Card[] { 
                new CardTypes.Platinum(),
                new CardTypes.Gold(),
                new CardTypes.Silver(),
                new CardTypes.Copper(),
                new CardTypes.Colony(),
                new CardTypes.Province(),
                new CardTypes.Duchy(),
                new CardTypes.Estate(),
                new CardTypes.Curse(),
                new CardTypes.Potion(),
                new CardTypes.RuinedLibrary(),
                new CardTypes.RuinedVillage(),
                new CardTypes.RuinedMarket(),
                new CardTypes.Survivors(),
                new CardTypes.Curse(),
                new CardTypes.Spoils(),
            };

            foreach (Card card in cardsToRemove)
            {
                cards.Remove(card);
            }            

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
