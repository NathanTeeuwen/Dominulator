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
                        bool leftHighValue = left.isTreasure || left == Cards.Mystic;
                        bool rightHighValue = right.isTreasure || right == Cards.Mystic;

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
                    cards = cards.Where(card => card != Cards.Estate && !card.isShelter);

                    if (cards.Any())
                        return MostCommonCard(cards);
                    else
                        return Cards.Estate;
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
                         CardAcceptance.For(Cards.Province),
                         CardAcceptance.For(Cards.Harem, gameState => CountOfPile(Cards.Province, gameState) > 1),
                         CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 5),
                         CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 3),                         
                         CardAcceptance.For(Cards.Mystic),
                         CardAcceptance.For(Cards.Scout, gameState => ShouldBuyScout(gameState)),
                         //CardAcceptance.For(Cards.MiningVillage),                         
                         //CardAcceptance.For(Cards.Scout, gameState => ShouldBuyScout2(gameState)),
                         CardAcceptance.For(Cards.Lookout, gameState => CountAllOwned(Cards.Lookout, gameState) == 0 && CountAllOwned(Cards.Mystic, gameState) == 0),
                         CardAcceptance.For(Cards.Silver));
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                    
                    CardAcceptance.For(Cards.Lookout, CurrentPlayerHasKnownCardToTrashOnTopOfDeck),
                    CardAcceptance.For(Cards.Mystic, CurrentPlayerHasKnownCardOnTopOfDeck),
                    CardAcceptance.For(Cards.Scout),
                    CardAcceptance.For(Cards.Lookout, Default.ShouldPlayLookout(ShouldBuyProvinces)),                    
                    CardAcceptance.For(Cards.Mystic),                    
                    CardAcceptance.For(Cards.MiningVillage));                    
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(Cards.Gold, gameState) > 2;
            }

            static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.OvergrownEstate),
                    CardAcceptance.For(Cards.Hovel),
                    CardAcceptance.For(Cards.Necropolis),
                    CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Scout, gameState) == 0),
                    CardAcceptance.For(Cards.Copper),
                    CardAcceptance.For(Cards.Estate));                    
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.OvergrownEstate),
                    CardAcceptance.For(Cards.Hovel),
                    CardAcceptance.For(Cards.Necropolis),                    
                    CardAcceptance.For(Cards.Estate),
                    CardAcceptance.For(Cards.Duchy),
                    CardAcceptance.For(Cards.Province),
                    CardAcceptance.For(Cards.Copper));
            }

            static bool ShouldBuyScout(GameState gameState)
            {
                return CountAllOwned(Cards.Scout, gameState) < 2 &&
                       ShouldBuyScoutOverMystic(gameState); 
            }

            static bool ShouldBuyScout2(GameState gameState)
            {
                return CountAllOwned(Cards.Scout, gameState) < 2 &&
                       CountAllOwned(Cards.Silver, gameState) + CountAllOwned(Cards.Harem, gameState) + CountAllOwned(Cards.Mystic, gameState) > 3;
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

                return currentPlayer.AllOwnedCards.Count(card => card.isVictory || card == Cards.Mystic);
                                
            }
        }
    }
}
