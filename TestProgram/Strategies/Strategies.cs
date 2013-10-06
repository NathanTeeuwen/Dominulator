using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        private enum RelativeAmount
        {            
            LessThan,
            LessThanEqual,
            GreaterThan,
            GreaterThanEqual,
            Equal
        }        

        private static GameStatePredicate CountAllOwned<T>(RelativeAmount relativeAmount, int amount)
        {
            switch (relativeAmount)
            {
                case RelativeAmount.LessThan:          return delegate(GameState gameState) { return CountAllOwned<T>(gameState) < amount; };
                case RelativeAmount.GreaterThan:       return delegate(GameState gameState) { return CountAllOwned<T>(gameState) > amount; };
                case RelativeAmount.LessThanEqual:     return delegate(GameState gameState) { return CountAllOwned<T>(gameState) <= amount; };
                case RelativeAmount.GreaterThanEqual:  return delegate(GameState gameState) { return CountAllOwned<T>(gameState) >= amount; };
                case RelativeAmount.Equal:             return delegate(GameState gameState) { return CountAllOwned<T>(gameState) == amount; };
                default: throw new System.Exception();
            }            
        }

        private static int CountInDeck<T>(GameState gameState)
        {
            return gameState.players.CurrentPlayer.CardsInDeck.Where(card => card is T).Count();
        }

        private static int CountInDeckAndDiscard<T>(GameState gameState)
        {
            return gameState.players.CurrentPlayer.CardsInDeckAndDiscard.Where(card => card is T).Count();
        }

        private static int CountMightDraw<T>(GameState gameState, int maxCount)
        {
            if (gameState.players.CurrentPlayer.CardsInDeck.Count() >= maxCount)
                return CountInDeck<T>(gameState);
            else
                return CountInDeckAndDiscard<T>(gameState);            
        }

        public static bool CardBeingPlayedIs<T>(GameState gameState)
                where T : Card
        {
            var cardBeingPlayed = gameState.players.CurrentPlayer.CurrentCardBeingPlayed;
            return cardBeingPlayed != null && cardBeingPlayed.Is<T>();
        }

        public static int CostOfCard<T>(GameState gameState)
            where T : Card, new()
        {
            return Example<T>.Card.CurrentCoinCost(gameState.players.CurrentPlayer);
        }

        public static class Example<T>
            where T: Card, new()
        {
            static public readonly T Card = new T();
        }

        public static int CountAllOwned<T>(GameState gameState)
        {
            return CountAllOwned(typeof(T), gameState);
        }

        public static int CountAllOwned(Type cardType, GameState gameState)
        {
            return gameState.players.CurrentPlayer.AllOwnedCards.Where( card => card.Is(cardType)).Count();
        }

        public static int CountInHand(Type cardType, GameState gameState)
        {
            return gameState.players.CurrentPlayer.Hand.Where(card => card.Is(cardType)).Count();
        }

        public static int CountInHand<T>(GameState gameState)
        {
            return CountInHand(typeof(T), gameState);
        }

        public static int CountOfPile<T>(GameState gameState)
        {
            return CountOfPile(typeof(T), gameState);
        }

        public static int CountOfPile(Type cardType, GameState gameState)
        {
            return gameState.GetPile(cardType).Count();
        }

        private static int CountAllOwnedMatching(ICardPicker matchingCards, GameState gameState)
        {
            int result = 0;

            foreach (Card card in gameState.players.CurrentPlayer.AllOwnedCards)
            {
                if (matchingCards.GetPreferredCard(gameState, testCard => testCard.Is(card.GetType())) != null)
                {
                    result += 1;
                }
            }

            return result;
        }

        private static int PlayersPointLead(GameState gameState)
        {
            return gameState.players.CurrentPlayer.TotalScore() - gameState.players.OtherPlayers.First().TotalScore();
        }

        private static GameStatePredicate HasCardInHand<T>()
            where T: Card, new()
        {
            return delegate(GameState gameState)
            {
                return HasCardInHand<T>(gameState);                
            };            
        }

        private static bool HasCardInHand<T>(GameState gameState)
        {
            return gameState.players.CurrentPlayer.Hand.HasCard<T>();
        }

        internal static Type WhichCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return matchingCards.GetPreferredCard(gameState, card => gameState.players.CurrentPlayer.Hand.HasCard(card));
        }        

        private static bool HasCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return WhichCardFromInHand(matchingCards, gameState) != null;
        }

        private static bool HandHasOnlyCardsFrom(ICardPicker matchingCards, GameState gameState)
        {
            foreach (Card card in gameState.players.CurrentPlayer.Hand)
            {
                if (matchingCards.GetPreferredCard(gameState, current => current.Is(card.GetType())) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static int CountInHandFrom(ICardPicker matchingCards, GameState gameState)
        {
            int result = 0;
            foreach (Card card in gameState.players.CurrentPlayer.Hand)
            {
                if (matchingCards.GetPreferredCard(gameState, current => current.Is(card.GetType())) != null)
                {
                    ++result;
                }
            }

            return result;
        }

        public static class Default
        {
            public static CardPickByPriority EmptyPickOrder()
            {
                return new CardPickByPriority();
            }

            public static ICardPicker ActionPlayOrder()
            {
                return new CardPickFromWhatsInHand( new SortCardByDefaultActionOrder());                    
            }

            private class CardPickFromWhatsInHand
                : ICardPicker
            {
                private readonly IComparerFactory comparerFactory;

                public CardPickFromWhatsInHand(IComparerFactory comparerFactory)
                {
                    this.comparerFactory = comparerFactory;
                }

                public int AmountWillingtoOverPayFor(Card card, GameState gameState)
                {
                    throw new NotImplementedException();
                }

                public Type GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);

                    PlayerState currentPlayer = gameState.players.CurrentPlayer;

                    Card cardToPlay = currentPlayer.Hand.Where(card => cardPredicate(card)).OrderBy(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay.GetType();
                }

                public Type GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);

                    PlayerState currentPlayer = gameState.players.CurrentPlayer;

                    Card cardToPlay = currentPlayer.Hand.Where(card => cardPredicate(card)).OrderByDescending(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay.GetType();
                }
                
                public IEnumerable<Card> GetNeededCards()
                {                    
                    yield break;
                }
            }

            private interface IComparerFactory
            {
                IComparer<Card> GetComparer(GameState gameState);
            }

            private class SortCardByDefaultActionOrder
                : IComparerFactory
            {
                public IComparer<Card> GetComparer(GameState gameState)
                {
                    return new Comparer(gameState);
                }

                private class Comparer
                    : IComparer<Card>
                {
                    private readonly GameState gameState;

                    public Comparer(GameState gameState)
                    {
                        this.gameState = gameState;
                    }

                    public int Compare(Card first, Card second)
                    {
                        if (first.plusAction != 0 ^ second.plusAction != 0)
                        {
                            return first.plusAction != 0 ? -1 : 1;
                        }

                        if (first.DefaultCoinCost != second.DefaultCoinCost)
                        {
                            return first.DefaultCoinCost - second.DefaultCoinCost;
                        }

                        if (first.isRuins && second.isRuins)
                        {
                            int result = CompareRuins(first, second, this.gameState);
                            if (result != 0)
                                return result;
                        }

                        return 0;
                    }                    
                }

                // TODO:  implement a better default choice of which Ruins to player.
                private static int CompareRuins(Card first, Card second, GameState gameState)
                {
                    return 0;
                }
            }

            public static CardPickByPriority TreasurePlayOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Contraband>(),       // play early to provide opponent as little information when banning
                    // base set first
                    CardAcceptance.For<CardTypes.Platinum>(),
                    CardAcceptance.For<CardTypes.Gold>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Spoils>(),
                    // alphabetical, all other treasures that dont really depend on order
                    CardAcceptance.For<CardTypes.Cache>(),
                    CardAcceptance.For<CardTypes.FoolsGold>(),
                    CardAcceptance.For<CardTypes.Loan>(),
                    CardAcceptance.For<CardTypes.Harem>(),
                    CardAcceptance.For<CardTypes.Hoard>(),
                    CardAcceptance.For<CardTypes.Masterpiece>(),                    
                    CardAcceptance.For<CardTypes.PhilosophersStone>(),
                    CardAcceptance.For<CardTypes.Quarry>(),
                    CardAcceptance.For<CardTypes.Stash>(),
                    CardAcceptance.For<CardTypes.Talisman>(),
                    // cards whose benefit is sensitive to ordering
                    CardAcceptance.For<CardTypes.Venture>(),          // playing this card might increase the number of treasures played
                    CardAcceptance.For<CardTypes.CounterFeit>(),      // after venture so that you have more variety to counterfeit
                    CardAcceptance.For<CardTypes.IllGottenGains>(),   // by playing after venture, you have more information about whether to gain the copper
                    CardAcceptance.For<CardTypes.HornOfPlenty>(),     // play relatively last so it has the most variety of cards to trigger with
                    CardAcceptance.For<CardTypes.Bank>());            // try to make bank as valuable as possibile.
            }

            public static CardPickByPriority DefaultDiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.OvergrownEstate>(),
                    CardAcceptance.For<CardTypes.Hovel>(),                    
                    CardAcceptance.For<CardTypes.Ruins>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Curse>());
            }

            public static ICardPicker DefaultTrashOrder()
            {                
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Curse>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) == 8),
                    CardAcceptance.For<CardTypes.OvergrownEstate>(),
                    CardAcceptance.For<CardTypes.Hovel>(),                    
                    CardAcceptance.For<CardTypes.Copper>());                
            }
            
            public static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned<CardTypes.Gold>(gameState) > 2;                
            }

            public static bool ShouldGainIllGottenGains(GameState gameState)
            {
                return CountOfPile<CardTypes.Curse>(gameState) > 0;
            }

            public static bool ShouldPlaySalvager(ICardPicker trashOrder, GameState gameState)
            {
                return HasCardFromInHand(trashOrder, gameState);
            }

            public static GameStatePredicate ShouldPlaySalvager(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    return HasCardFromInHand(trashOrder, gameState);
                };
            }

            public static GameStatePredicate ShouldPlayLookout(GameStatePredicate shouldBuyProvinces = null)
            {
                if (shouldBuyProvinces == null)
                {
                    shouldBuyProvinces = ShouldBuyProvinces;
                }
                return delegate(GameState gameState)
                {
                    return ShouldPlayLookout(gameState, shouldBuyProvinces);
                };
            }

            public static bool ShouldPlayLookout(GameState gameState, GameStatePredicate shouldBuyProvinces)
            {
                int cardCountToTrash = CountInDeck<CardTypes.Copper>(gameState);

                if (!shouldBuyProvinces(gameState))
                {
                    cardCountToTrash += CountInDeck<CardTypes.Estate>(gameState);
                }

                cardCountToTrash += CountInDeck<CardTypes.Hovel>(gameState);
                cardCountToTrash += CountInDeck<CardTypes.Necropolis>(gameState);
                cardCountToTrash += CountInDeck<CardTypes.OvergrownEstate>(gameState);

                cardCountToTrash += CountInDeck<CardTypes.Lookout>(gameState);

                int totalCardsOwned = gameState.players.CurrentPlayer.CardsInDeck.Count();

                return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
            }
        }

        public static class DoubleWarehouse
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "DoubleWarehouse",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),                            
                            actionOrder: ActionOrder(),                            
                            discardOrder: DiscardOrder());
            }

            static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Warehouse>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Warehouse).Count() < 1),
                           CardAcceptance.For<CardTypes.Warehouse>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Silver).Count() > 2 &&
                                                                                gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Warehouse).Count() < 2),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Warehouse>());
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Warehouse>(),
                    CardAcceptance.For<CardTypes.Gold>());
            }
        }              

        public static class BigMoneyDelayed
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyDelayed",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 3),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Silver>());
            }
        } 
    }
}
