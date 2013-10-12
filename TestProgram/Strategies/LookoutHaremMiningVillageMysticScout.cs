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
        public static class LookoutHaremMiningVillageMysticScout
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base(
                        "LookoutHaremMiningVillageMysticScout",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        discardOrder:DiscardOrder())
                {
                }

                // for scout
                override public Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.OrderBy(card => card, new OrderCardByMostValued(player)).First();
                }

                class OrderCardByMostValued
                    : IComparer<Card>
                {
                    private readonly PlayerState player;

                    public OrderCardByMostValued(PlayerState player)
                    {
                        this.player = player;
                    }

                    public int Compare(Card left, Card right)
                    {
                        bool leftHighValue = left.isTreasure || left == CardTypes.Mystic.card;
                        bool rightHighValue = right.isTreasure || right == CardTypes.Mystic.card;

                        if (leftHighValue ^ rightHighValue)
                        {
                            return leftHighValue ? 1 : -1;
                        }

                        return left.CurrentCoinCost(this.player) - right.CurrentCoinCost(this.player);
                    }
                }

                // for mystic
                override public Card GuessCardTopOfDeck(GameState gameState)
                {
                    PlayerState self = gameState.Self;
                    if (self.KnownCardsInDeck.Any())
                    {
                        return self.KnownCardsInDeck.First();
                    }

                    IEnumerable<Card> cards = self.CardsInDeck.Any() ? self.CardsInDeck : self.CardsInDeckAndDiscard;
                    cards = cards.Where(card => card != CardTypes.Estate.card && !card.isShelter);

                    if (cards.Any())
                        return MostCommonCard(cards);
                    else
                        return CardTypes.Estate.card;
                }

                // for playing mining village
                public override bool ShouldTrashCard(GameState gameState, Card card)
                {
                    return true;
                }

                private Card MostCommonCard(IEnumerable<Card> cards)
                {
                    Card result = cards.GroupBy(
                                    card => card,                                    
                                    (key, group) => new { Card = key, Count = group.Count() })
                                .OrderByDescending(group => group.Count)
                                .First()
                                .Card;

                    return result;
                }
            }

            static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                         CardAcceptance.For(CardTypes.Province.card),
                         CardAcceptance.For(CardTypes.Harem.card, gameState => CountOfPile(CardTypes.Province.card, gameState) > 1),
                         CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 5),
                         CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 3),                         
                         CardAcceptance.For(CardTypes.Mystic.card),
                         CardAcceptance.For(CardTypes.Scout.card, gameState => ShouldBuyScout(gameState)),
                         //CardAcceptance.For(CardTypes.MiningVillage.card),                         
                         //CardAcceptance.For(CardTypes.Scout.card, gameState => ShouldBuyScout2(gameState)),
                         CardAcceptance.For(CardTypes.Lookout.card, gameState => CountAllOwned(CardTypes.Lookout.card, gameState) == 0 && CountAllOwned(CardTypes.Mystic.card, gameState) == 0),
                         CardAcceptance.For(CardTypes.Silver.card));
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                    
                    CardAcceptance.For(CardTypes.Lookout.card, CurrentPlayerHasKnownCardToTrashOnTopOfDeck),
                    CardAcceptance.For(CardTypes.Mystic.card, CurrentPlayerHasKnownCardOnTopOfDeck),
                    CardAcceptance.For(CardTypes.Scout.card),
                    CardAcceptance.For(CardTypes.Lookout.card, Default.ShouldPlayLookout(ShouldBuyProvinces)),                    
                    CardAcceptance.For(CardTypes.Mystic.card),                    
                    CardAcceptance.For(CardTypes.MiningVillage.card));                    
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(CardTypes.Gold.card, gameState) > 2;
            }

            static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.OvergrownEstate.card),
                    CardAcceptance.For(CardTypes.Hovel.card),
                    CardAcceptance.For(CardTypes.Necropolis.card),
                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountAllOwned(CardTypes.Scout.card, gameState) == 0),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Estate.card));                    
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.OvergrownEstate.card),
                    CardAcceptance.For(CardTypes.Hovel.card),
                    CardAcceptance.For(CardTypes.Necropolis.card),                    
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Copper.card));
            }

            static bool ShouldBuyScout(GameState gameState)
            {
                return CountAllOwned(CardTypes.Scout.card, gameState) < 2 &&
                       ShouldBuyScoutOverMystic(gameState); 
            }

            static bool ShouldBuyScout2(GameState gameState)
            {
                return CountAllOwned(CardTypes.Scout.card, gameState) < 2 &&
                       CountAllOwned(CardTypes.Silver.card, gameState) + CountAllOwned(CardTypes.Harem.card, gameState) + CountAllOwned(CardTypes.Mystic.card, gameState) > 3;
            }

            static bool CurrentPlayerHasKnownCardOnTopOfDeck(GameState gameState)
            {
                PlayerState self = gameState.Self;
                return self.KnownCardsInDeck.Any();                    
            }

            static bool CurrentPlayerHasKnownCardToTrashOnTopOfDeck(GameState gameState)
            {
                PlayerState self = gameState.Self;
                if (!self.KnownCardsInDeck.Any())
                    return false;

                Card firstCard = self.KnownCardsInDeck.First();

                return TrashOrder().GetPreferredCard(gameState, card => card.Equals(firstCard)) != null;
            }

            static bool ShouldBuyScoutOverMystic(GameState gameState)
            {
                return ((double)CountAllCardsBenefitFromScout(gameState)) / gameState.Self.AllOwnedCards.Count >= 0.5;
            }            

            static int CountAllCardsBenefitFromScout(GameState gameState)
            {
                PlayerState currentPlayer = gameState.Self;

                return currentPlayer.AllOwnedCards.Count(card => card.isVictory || card == CardTypes.Mystic.card);
                                
            }
        }
    }
}
