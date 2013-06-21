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
            FeodumVsDuke();           
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
                ComparePlayers(Strategies.FollowersTest.Player(1, i), Strategies.BigMoney.Player(2), showCompactScore:true);
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
            return new HumanReadableGameLog("..\\..\\Results\\GameLog"  + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt");
        }


        static void ComparePlayers(PlayerAction player1, PlayerAction player2, bool useShelters = false, bool firstPlayerAdvantage = false, bool showCompactScore = false, bool showDistribution=false)
        {
            int numberOfGames = 10000;

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
                            new PlayerAction[] { startPlayer, otherPlayer},
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
                    System.Console.WriteLine("Player {0} won: {1} percent of the time.", index + 1, PlayerWinPercent(index, winnerCount, numberOfGames));
                }
                if (tieCount > 0)
                {
                    System.Console.WriteLine("Ties: {0} percent of the time.", TiePercent(tieCount, numberOfGames));
                }
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
                foreach(var pair in this.mapBucketToCount.OrderByDescending(keyValuePair => keyValuePair.Key))
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

        static void AddCards(HashSet<Card> cardSet, IGetMatchingCard matchingCards)
        {
            foreach (Card card in matchingCards.GetNeededCards())
            {
                cardSet.Add(card);
            }
        }        
    }

    public struct CardAcceptance
    {
        internal Card card;
        internal GameStatePredicate match;

        public CardAcceptance(Card card)
        {
            this.card = card;
            this.match = gameState => true;
        }

        public CardAcceptance(Card card, GameStatePredicate match)
        {
            this.card = card;
            this.match = match;
        }

        public static CardAcceptance For(Card card)
        {
            return new CardAcceptance(card);
        }

        public static CardAcceptance For(Card card, GameStatePredicate match)
        {
            return new CardAcceptance(card, match);
        }

        public static CardAcceptance For<T>()
            where T : Card, new()
        {
            return new CardAcceptance(new T());
        }

        public static CardAcceptance For<T>(GameStatePredicate match)
            where T : Card, new()
        {
            return new CardAcceptance(new T(), match);
        }
    }

    public interface IGetMatchingCard        
    {
        Type GetMatchingCard(GameState gameState, CardPredicate cardPredicate);
        IEnumerable<Card> GetNeededCards();        
    }

    public class CardPickByPriority
        : IGetMatchingCard
    {
        private readonly CardAcceptance[] cardAcceptances;

        public CardPickByPriority(params CardAcceptance[] cardAcceptances)
        {
            this.cardAcceptances = cardAcceptances;
        }        

        public Type GetMatchingCard(GameState gameState, CardPredicate cardPredicate)
        {
            foreach (CardAcceptance acceptance in this.cardAcceptances)
            {
                if (cardPredicate(acceptance.card) &&
                    acceptance.match(gameState))
                {
                    return acceptance.card.GetType();
                }
            }

            return null;
        }

        public Type GetMatchingCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            for (int i = this.cardAcceptances.Length-1; i>= 0; i--)
            {
                CardAcceptance acceptance = this.cardAcceptances[i];            
                if (cardPredicate(acceptance.card) &&
                    acceptance.match(gameState))
                {
                    return acceptance.card.GetType();
                }
            }

            return null;
        }

        public IEnumerable<Card> GetNeededCards()
        {
            return this.cardAcceptances.Select( cardAcceptance => cardAcceptance.card);
        }        
    }

    public class CardPickByBuildOrder
        : IGetMatchingCard
    {        
        private readonly Card[] buildOrder;

        public CardPickByBuildOrder(params Card[] buildOrer)
        {
            this.buildOrder = buildOrer;
        }

        public Type GetMatchingCard(GameState gameState, CardPredicate cardPredicate)
        {
            var existingCards = new BagOfCards();

            foreach (Card card in gameState.players.CurrentPlayer.AllOwnedCards)
            {
                existingCards.AddCard(card);
            }

            int numberOfTries = 2;
            
            for (int index = 0; index < this.buildOrder.Length; ++index)
            {
                Card currentCard = this.buildOrder[index];

                if (existingCards.HasCard(currentCard))
                {
                    existingCards.RemoveCard(currentCard);
                    continue;
                }
                numberOfTries--;
                
                if (cardPredicate(currentCard))
                {                    
                    return currentCard.GetType();
                }

                if (numberOfTries == 0)
                {
                    break;
                }
            }

            return null;
        }

        public IEnumerable<Card> GetNeededCards()
        {
            return this.buildOrder;
        }        
    }

    public class CardPickConcatenator
        : IGetMatchingCard
    {
        private readonly IGetMatchingCard[] matchers;

        public CardPickConcatenator(params IGetMatchingCard[] matchers)
        {
            this.matchers = matchers;
        }

        public Type GetMatchingCard(GameState gameState, CardPredicate cardPredicate)
        {
            foreach (IGetMatchingCard matcher in this.matchers)
            {
                Type result = matcher.GetMatchingCard(gameState, cardPredicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IEnumerable<Card> GetNeededCards()
        {
            foreach (IGetMatchingCard matcher in this.matchers)
            {
                foreach (Card card in matcher.GetNeededCards())
                {
                    yield return card;
                }
            }
        }        
    }

    public class PlayerAction
        : DefaultPlayerAction
    {
        internal readonly int playerIndex;
        internal readonly IGetMatchingCard purchaseOrder;
        internal readonly IGetMatchingCard actionOrder;
        internal readonly IGetMatchingCard trashOrder;
        internal readonly IGetMatchingCard treasurePlayOrder;
        internal readonly IGetMatchingCard discardOrder;
        internal readonly IGetMatchingCard gainOrder;

        public PlayerAction(int playerIndex,
            IGetMatchingCard purchaseOrder,
            IGetMatchingCard actionOrder,
            IGetMatchingCard treasurePlayOrder = null,            
            IGetMatchingCard discardOrder = null,
            IGetMatchingCard trashOrder = null,
            IGetMatchingCard gainOrder = null)
        {
            this.playerIndex = playerIndex;
            this.purchaseOrder = purchaseOrder;
            this.actionOrder = actionOrder;
            this.trashOrder = trashOrder == null ? Strategies.Default.EmptyPickOrder() : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? Strategies.Default.TreasurePlayOrder() : treasurePlayOrder;
            this.discardOrder = discardOrder == null ? Strategies.Default.EmptyPickOrder() : discardOrder;
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
        }        

        public override Type GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.purchaseOrder.GetMatchingCard(
                gameState,
                card => currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer) &&
                gameState.GetPile(card).Any());
        }

        public override Type GetTreasureFromHandToPlay(GameState gameState)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.treasurePlayOrder.GetMatchingCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()));
        }

        public override Type GetActionFromHandToPlay(GameState gameState, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.actionOrder.GetMatchingCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()));
        }

        public override Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.trashOrder.GetMatchingCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                Card card = currentPlayer.Hand.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard)
        {
            var currentPlayer = player;
            Type result = this.trashOrder.GetMatchingCard(
                gameState,
                card => currentPlayer.CardsBeingRevealed.HasCard(card.GetType()) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = currentPlayer.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.discardOrder.GetMatchingCard(
                gameState, 
                card => currentPlayer.CardsBeingRevealed.HasCard(card.GetType()));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = currentPlayer.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
        {
            BagOfCards revealedCards = player.CardsBeingRevealed;
            // good for cartographer, not sure about anyone else.
            foreach (Card card in revealedCards)
            {
                bool shouldDiscard = card.isVictory || card.Is<CardTypes.Copper>();
                if (!shouldDiscard)
                {
                    return card.GetType();
                }
            }

            return null;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return this.discardOrder.GetMatchingCard(gameState, testCard => testCard.Is(card.GetType())) != null;
        }

        struct CompareCardByFirstToTrash
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? -1 : 1;
                }

                if (x.isAction ^ y.isAction)
                {
                    return x.isAction ? -1 : 1;
                }

                if (x.isTreasure ^ y.isTreasure)
                {
                    return x.isTreasure ? -1 : 1;
                }

                return x.DefaultCoinCost < y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost > y.DefaultCoinCost ? 1 :
                       0;
            }
        }

        public override Type GetCardFromHandToDiscard(GameState gameState, PlayerState player, bool isOptional)
        {            
            Type result = this.discardOrder.GetMatchingCard(
                gameState,
                card => player.Hand.HasCard(card.GetType()));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                Card card = player.Hand.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        struct CompareCardByFirstToDiscard
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? -1 : 1;
                }

                if (x.isAction ^ y.isAction)
                {
                    return x.isAction ? 1 : -1;
                }

                if (x.isVictory ^ y.isVictory)
                {
                    return x.isVictory ? -1 : 1;
                }

                return x.DefaultCoinCost < y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost > y.DefaultCoinCost ? 1 :
                       0;
            }
        }

        public override Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
        {            
            return this.discardOrder.GetMatchingCard(
                gameState,
                card => player.CardsBeingRevealed.HasCard(card.GetType()));
        }

        public override Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.gainOrder.GetMatchingCard(
                gameState,
                card => acceptableCard(card) && gameState.GetPile(card).Any());

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                Card card = gameState.supplyPiles.Where(supplyPile => !supplyPile.IsEmpty).Select(pile => pile.ProtoTypeCard).Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToGain()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        struct CompareCardByFirstToGain
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? 1 : -1;
                }

                if (x.isRuin ^ y.isRuin)
                {
                    return x.isRuin ? 1 : -1;
                }

                if (x.isTreasure ^ y.isTreasure)
                {
                    return x.isTreasure ? -1 : 1;
                }               

                return x.DefaultCoinCost > y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost < y.DefaultCoinCost ? 1 :
                       0;
            }
        }

        public override string PlayerName
        {
            get
            {
                return "Player" + playerIndex;
            }
        }
    }    
}
