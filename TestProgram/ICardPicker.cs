using Dominion;
using System;
using System.Collections.Generic;

namespace Program
{
    public interface ICardPicker
    {
        Type GetPreferredCard(GameState gameState, CardPredicate cardPredicate);
        IEnumerable<Card> GetNeededCards();
    }
}
