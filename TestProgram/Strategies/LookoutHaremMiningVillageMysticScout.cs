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
                        treasurePlayOrder: Default.TreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        discardOrder:DiscardOrder())
                {
                }

                // for scout
                override public Type GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.OrderBy(card => card, new OrderCardByMostValued(player)).First().GetType();
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
                        bool leftHighValue = left.isTreasure || left.Is<CardTypes.Mystic>();
                        bool rightHighValue = right.isTreasure || right.Is<CardTypes.Mystic>();

                        if (leftHighValue ^ rightHighValue)
                        {
                            return leftHighValue ? 1 : -1;
                        }

                        return left.CurrentCoinCost(this.player) - right.CurrentCoinCost(this.player);
                    }
                }

                // for mystic
                override public Type GuessCardTopOfDeck(GameState gameState)
                {
                    PlayerState currentPlayer = gameState.players.CurrentPlayer;
                    if (currentPlayer.KnownCardsInDeck.Any())
                    {
                        return currentPlayer.KnownCardsInDeck.First().GetType();
                    }

                    IEnumerable<Card> cards = currentPlayer.CardsInDeck.Any() ? currentPlayer.CardsInDeck : currentPlayer.CardsInDeckAndDiscard;
                    cards = cards.Where(card => !card.Is<CardTypes.Estate>() && !card.isShelter);

                    if (cards.Any())
                        return MostCommonCard(cards).GetType();                    
                    else
                        return typeof(CardTypes.Estate);
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
                         CardAcceptance.For<CardTypes.Province>(),
                         CardAcceptance.For<CardTypes.Harem>(gameState => CountOfPile<CardTypes.Province>(gameState) > 1),
                         CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 5),
                         CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 3),                         
                         CardAcceptance.For<CardTypes.Mystic>(),
                         CardAcceptance.For<CardTypes.Scout>(gameState => ShouldBuyScout(gameState)),
                         //CardAcceptance.For<CardTypes.MiningVillage>(),                         
                         //CardAcceptance.For<CardTypes.Scout>(gameState => ShouldBuyScout2(gameState)),
                         CardAcceptance.For<CardTypes.Lookout>(gameState => CountAllOwned<CardTypes.Lookout>(gameState) == 0 && CountAllOwned<CardTypes.Mystic>(gameState) == 0),
                         CardAcceptance.For<CardTypes.Silver>());
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                    
                    CardAcceptance.For<CardTypes.Lookout>(CurrentPlayerHasKnownCardToTrashOnTopOfDeck),
                    CardAcceptance.For<CardTypes.Mystic>(CurrentPlayerHasKnownCardOnTopOfDeck),
                    CardAcceptance.For<CardTypes.Scout>(),
                    CardAcceptance.For<CardTypes.Lookout>(Default.ShouldPlayLookout(ShouldBuyProvinces)),                    
                    CardAcceptance.For<CardTypes.Mystic>(),                    
                    CardAcceptance.For<CardTypes.MiningVillage>());                    
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned<CardTypes.Gold>(gameState) > 2;
            }

            static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.OvergrownEstate>(),
                    CardAcceptance.For<CardTypes.Hovel>(),
                    CardAcceptance.For<CardTypes.Necropolis>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountAllOwned<CardTypes.Scout>(gameState) == 0),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Estate>());                    
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.OvergrownEstate>(),
                    CardAcceptance.For<CardTypes.Hovel>(),
                    CardAcceptance.For<CardTypes.Necropolis>(),                    
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Copper>());
            }

            static bool ShouldBuyScout(GameState gameState)
            {
                return CountAllOwned<CardTypes.Scout>(gameState) < 2 &&
                       ShouldBuyScoutOverMystic(gameState); 
            }

            static bool ShouldBuyScout2(GameState gameState)
            {
                return CountAllOwned<CardTypes.Scout>(gameState) < 2 &&
                       CountAllOwned<CardTypes.Silver>(gameState) + CountAllOwned<CardTypes.Harem>(gameState) + CountAllOwned<CardTypes.Mystic>(gameState) > 3;
            }

            static bool CurrentPlayerHasKnownCardOnTopOfDeck(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;
                return currentPlayer.KnownCardsInDeck.Any();                    
            }

            static bool CurrentPlayerHasKnownCardToTrashOnTopOfDeck(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;
                if (!currentPlayer.KnownCardsInDeck.Any())
                    return false;

                Card firstCard = currentPlayer.KnownCardsInDeck.First();

                return TrashOrder().GetPreferredCard(gameState, card => card.Equals(firstCard)) != null;
            }

            static bool ShouldBuyScoutOverMystic(GameState gameState)
            {
                return ((double)CountAllCardsBenefitFromScout(gameState)) / gameState.players.CurrentPlayer.AllOwnedCards.Count() >= 0.5;
            }            

            static int CountAllCardsBenefitFromScout(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;

                int result = 0;

                foreach (Card card in currentPlayer.AllOwnedCards)
                {
                    if (card.isVictory || card.Is<CardTypes.Mystic>())
                        result++;
                }

                return result;
            }
        }
    }
}
