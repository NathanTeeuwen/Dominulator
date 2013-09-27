using Dominion;
using System;
using System.Collections.Generic;

namespace Program
{
    public interface ICardPicker
    {
        int AmountWillingtoOverPayFor(Card card, GameState gameState);
        Type GetPreferredCard(GameState gameState, CardPredicate cardPredicate);
        IEnumerable<Card> GetNeededCards();
    }
}
