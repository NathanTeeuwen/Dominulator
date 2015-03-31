using Dominion;
using System;
using System.Collections.Generic;

namespace Dominion.Strategy
{
    public interface ICardPicker
    {
        int AmountWillingtoOverPayFor(Card card, GameState gameState);        
        Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate);
        Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate, CardPredicate defaultPredicate);
        Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate);
        IEnumerable<Card> GetNeededCards();
    }

    public static class ICardPickerExtensions
    {
        public static bool DoesCardPickerMatch(this ICardPicker pickOrder, GameState gameState, Card card)
        {
            return pickOrder.GetPreferredCard(gameState, c => c == card) != null;
        }

        public static int CountInSetMatching(this ICardPicker pickOrder, GameState gameState, CollectionCards collectionCards)
        {
            int result = 0;
            foreach(var cardType in collectionCards.AllTypes)
            {
                if (pickOrder.DoesCardPickerMatch(gameState, cardType))
                {
                    result += collectionCards.CountOf(cardType);
                }
            }

            return result;
        }
    }
}
