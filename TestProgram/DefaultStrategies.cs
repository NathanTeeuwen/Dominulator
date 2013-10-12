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
        public static class Default
        {
            public static CardPickByPriority EmptyPickOrder()
            {
                return new CardPickByPriority();
            }

            public static ICardPicker DefaultActionPlayOrder(ICardPicker purchaseOrder)
            {
                return new CardPickFromWhatsInHand(new SortCardByDefaultActionOrder(purchaseOrder));
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

                public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);
                    
                    Card cardToPlay = gameState.Self.Hand.Where(card => cardPredicate(card)).OrderBy(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay;
                }

                public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);

                    Card cardToPlay = gameState.Self.Hand.Where(card => cardPredicate(card)).OrderByDescending(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay;
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
                private readonly ICardPicker purchaseOrder;

                public SortCardByDefaultActionOrder(ICardPicker purchaseOrder)
                {
                    this.purchaseOrder = purchaseOrder;
                }

                public IComparer<Card> GetComparer(GameState gameState)
                {
                    return new Comparer(gameState, purchaseOrder);
                }

                private class Comparer
                    : IComparer<Card>
                {
                    private readonly GameState gameState;
                    private readonly ICardPicker purchaseOrder;

                    public Comparer(GameState gameState, ICardPicker purchaseOrder)
                    {
                        this.gameState = gameState;
                        this.purchaseOrder = purchaseOrder;
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
                            int result = CompareRuins(first, second, this.gameState, this.purchaseOrder);
                            if (result != 0)
                                return result;
                        }

                        return 0;
                    }
                }

                // TODO:  implement a better default choice of which Ruins to player.
                private static int CompareRuins(Card first, Card second, GameState gameState, ICardPicker purchaseOrder)
                {
                    PlayerState self = gameState.Self;

                    int coinsToSpend = self.ExpectedCoinValueAtEndOfTurn;

                    if (first == CardTypes.AbandonedMine.card || second == CardTypes.AbandonedMine.card)
                    {
                        CardPredicate shouldGainCard = delegate(Card card)
                        {
                            int currentCardCost = card.CurrentCoinCost(self);

                            return currentCardCost == coinsToSpend + 1;
                        };

                        Card cardType = purchaseOrder.GetPreferredCard(gameState, shouldGainCard);
                        if (cardType != null)
                            return first == CardTypes.AbandonedMine.card ? 0 : 1;

                        //Card Card1 = purchaseOrder.GetPreferredCard(
                        //        gameState,
                        //        card => coinsToSpend >= card.CurrentCoinCost(currentPlayer) &&
                        //        gameState.GetPile(card).Any());
                        //Card Card2 = purchaseOrder.GetPreferredCard(
                        //        gameState,
                        //        card => coinsToSpend + 1 >= card.CurrentCoinCost(currentPlayer) &&
                        //        gameState.GetPile(card).Any());

                        //if (Card1 != Card2)
                        //    return first.name == "Abandoned Mine" ? 0 : 1;
                    }
                    return 0;
                }
            }

            public static CardPickByPriority DefaultTreasurePlayOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Contraband.card),       // play early to provide opponent as little information when banning
                    // base set first
                    CardAcceptance.For(CardTypes.Platinum.card),
                    CardAcceptance.For(CardTypes.Gold.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Spoils.card),
                    CardAcceptance.For(CardTypes.Potion.card),
                    // alphabetical, all other treasures that dont really depend on order
                    CardAcceptance.For(CardTypes.Cache.card),
                    CardAcceptance.For(CardTypes.FoolsGold.card),
                    CardAcceptance.For(CardTypes.Loan.card),
                    CardAcceptance.For(CardTypes.Harem.card),
                    CardAcceptance.For(CardTypes.Hoard.card),
                    CardAcceptance.For(CardTypes.Masterpiece.card),
                    CardAcceptance.For(CardTypes.PhilosophersStone.card),
                    CardAcceptance.For(CardTypes.Quarry.card),
                    CardAcceptance.For(CardTypes.Stash.card),
                    CardAcceptance.For(CardTypes.Talisman.card),
                    // cards whose benefit is sensitive to ordering
                    CardAcceptance.For(CardTypes.Venture.card),          // playing this card might increase the number of treasures played
                    CardAcceptance.For(CardTypes.CounterFeit.card),      // after venture so that you have more variety to counterfeit
                    CardAcceptance.For(CardTypes.IllGottenGains.card),   // by playing after venture, you have more information about whether to gain the copper
                    CardAcceptance.For(CardTypes.HornOfPlenty.card),     // play relatively last so it has the most variety of cards to trigger with
                    CardAcceptance.For(CardTypes.Bank.card));            // try to make bank as valuable as possibile.
            }

            public static CardPickByPriority DefaultDiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.OvergrownEstate.card),
                    CardAcceptance.For(CardTypes.Hovel.card),
                    CardAcceptance.For(CardTypes.Ruins.card),
                    CardAcceptance.For(CardTypes.Curse.card),
                    CardAcceptance.For(CardTypes.Copper.card));
            }

            public static ICardPicker DefaultTrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Curse.card),
                    CardAcceptance.For(CardTypes.RuinedVillage.card),
                    CardAcceptance.For(CardTypes.RuinedMarket.card),
                    CardAcceptance.For(CardTypes.Survivors.card),
                    CardAcceptance.For(CardTypes.RuinedLibrary.card),
                    CardAcceptance.For(CardTypes.AbandonedMine.card), 
                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountAllOwned(CardTypes.Province.card, gameState) == 0),
                    CardAcceptance.For(CardTypes.OvergrownEstate.card),
                    CardAcceptance.For(CardTypes.Hovel.card),
                    CardAcceptance.For(CardTypes.Copper.card));
            }

            public static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(CardTypes.Gold.card, gameState) > 2;
            }

            public static bool ShouldGainIllGottenGains(GameState gameState)
            {
                return CountOfPile(CardTypes.Curse.card, gameState) > 0;
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
                int cardCountToTrash = CountInDeck(CardTypes.Copper.card, gameState);

                if (!shouldBuyProvinces(gameState))
                {
                    cardCountToTrash += CountInDeck(CardTypes.Estate.card, gameState);
                }

                cardCountToTrash += CountInDeck(CardTypes.Hovel.card, gameState);
                cardCountToTrash += CountInDeck(CardTypes.Necropolis.card, gameState);
                cardCountToTrash += CountInDeck(CardTypes.OvergrownEstate.card, gameState);

                cardCountToTrash += CountInDeck(CardTypes.Lookout.card, gameState);

                int totalCardsOwned = gameState.Self.CardsInDeck.Count;

                return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
            }
        }
    }
}