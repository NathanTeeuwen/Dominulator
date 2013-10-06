using Dominion;
using System;
using System.Collections.Generic;

namespace Program
{
    public interface ICardPicker
    {
        int AmountWillingtoOverPayFor(Card card, GameState gameState);
        Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate);
        Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate);
        IEnumerable<Card> GetNeededCards();
    }
}
