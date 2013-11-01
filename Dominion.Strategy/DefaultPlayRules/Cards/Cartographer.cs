using Dominion;
using System;
using System.Linq;

namespace Program.DefaultPlayRules
{
    internal class Cartographer
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Cartographer(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            // good for cartographer, not sure about anyone else.
            foreach (Card card in gameState.Self.CardsBeingRevealed)
            {
                bool shouldDiscard = card.isVictory || card == Cards.Copper;
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
        }
    }
}