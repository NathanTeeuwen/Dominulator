using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Cartographer
        : DerivedPlayerAction
    {
        public Cartographer(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, bool isOptional)
        {
            // good for cartographer, not sure about anyone else.
            foreach (Card card in gameState.Self.CardsBeingRevealed)
            {
                bool shouldDiscard = card.isVictory || card == Dominion.Cards.Copper;
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
        }
    }
}