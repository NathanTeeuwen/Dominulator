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

        private static int CountMightDraw<T>(GameState gameState)
        {
            return gameState.players.CurrentPlayer.CardsInDeckAndDiscard.Where(card => card is T).Count();
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

        private static GameStatePredicate HasCardInHand<T>()
            where T: Card, new()
        {
            return delegate(GameState gameState)
            {
                return gameState.players.CurrentPlayer.Hand.HasCard<T>();
            };            
        }        

        private static bool HasCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return matchingCards.GetPreferredCard(gameState, card => gameState.players.CurrentPlayer.Hand.HasCard(card)) != null;
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

            public static CardPickByPriority ActionPlayOrder(ICardPicker purchaseOrder)
            {
                return new CardPickByPriority(
                    purchaseOrder.GetNeededCards().Where(card => card.isAction).OrderBy(card => card, new SortCardByActionOrder()).Select(card => new CardAcceptance(card)).ToArray());
            }

            private struct SortCardByActionOrder
                : IComparer<Card>
            {
                public int Compare(Card first, Card second)
                {
                    if (first.plusAction != 0 ^ second.plusAction != 0)
                    {
                        return first.plusAction != 0 ? -1 : 1;
                    }

                    return 0;
                }
            }

            public static CardPickByPriority TreasurePlayOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Platinum>(),
                    CardAcceptance.For<CardTypes.Gold>(),
                    CardAcceptance.For<CardTypes.Harem>(),
                    CardAcceptance.For<CardTypes.Talisman>(),                    
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Copper>());
            }

            public static CardPickByPriority DefaultDiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Ruin>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Gold>());
            }

            public static GameStatePredicate ShouldPlayLookout(GameStatePredicate shouldBuyProvinces)
            {
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
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 2),
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
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: Default.EmptyPickOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 3),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Silver>());
            }
        }


        public static class BigMoneyDoubleSmithy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber, int secondSmithy = 15)
            {
                return new PlayerAction(
                            "BigMoneyDoubleSmithy",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(secondSmithy),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static CardPickByPriority PurchaseOrder(int secondSmithy)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Smithy>(gameState => CountAllOwned<CardTypes.Smithy>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Smithy>(gameState => CountAllOwned<CardTypes.Smithy>(gameState) < 2 &&
                                                                             gameState.players.CurrentPlayer.AllOwnedCards.Count() >= secondSmithy),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Smithy>());
            }
        }

        public static class BigMoneySingleSmithy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Smithy>.Player(playerNumber);
            }
        }    
    }
}
