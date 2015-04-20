using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class ApothecaryBishop
            : Strategy
    {
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("ApothecaryBishop",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }

            public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, bool isOptional)
            {
                var result = gameState.Self.CardsBeingRevealed.FindCard(card => card != Cards.Apothecary);
                
                if (result != null) return result;
                
                return base.GetCardFromRevealedCardsToTopDeck(gameState, isOptional);
            }
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Potion, 1),
                        CardAcceptance.For(Cards.Apothecary, 4),
                        CardAcceptance.For(Cards.Bishop, 1),
                        CardAcceptance.For(Cards.Apothecary),
                        //CardAcceptance.For(Cards.Bishop),
                        //CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.Copper, 7));

        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Apothecary, ShouldPlayApothecary),
                        CardAcceptance.For(Cards.Bishop));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Potion, ShouldTrashPotion),
                        CardAcceptance.For(Cards.Province, ShouldTrashProvince),
                        CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Estate, gameState) > 1 || ShouldTrashLastEstate(gameState)),
                        CardAcceptance.For(Cards.Bishop, gameState => CountAllOwned(Cards.Bishop, gameState) > 1));
        }

        private static bool ShouldTrashPotion(GameState gameState)
        {
            bool isSelfPlaying = gameState.CurrentContext.IsSelfPlaying(gameState);
            if (!isSelfPlaying)
                return false;

            if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 7 && CountOfPile(Cards.Province, gameState) <= 3)
                return true;

            return false;
        }

        private static bool ShouldTrashProvince(GameState gameState)
        {
            bool isSelfPlaying = gameState.CurrentContext.IsSelfPlaying(gameState);
            if (!isSelfPlaying)
                return false;

            if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 7)
                return true;

            return false;
        }

        private static bool ShouldTrashLastEstate(GameState gameState)
        {
            if (CountAllOwned(Cards.Province, gameState) > 0)
                return true;

            if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 7)
                return true;

            return false;
        }

        private static bool ShouldPlayApothecary(GameState gameState)
        {
            if (gameState.Self.CardsInDeckAndDiscard.Where(card => card != Cards.Apothecary).Any())
                return true;
            return false;
        }
    }
}