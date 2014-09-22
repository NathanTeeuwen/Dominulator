using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Chancellor
       : DerivedPlayerAction
    {
        public Chancellor(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }
        
        
        private bool ShouldPutDeckInDiscardBasedOnAverageValue(GameState gameState)
        {
            double averageValueOfDeck = 0;
            double averageValueOfDiscard = 0;

            if (gameState.Self.CardsInDeck.AnyWhere(card => !card.isVictory))
                averageValueOfDeck = ((double)gameState.Self.CardsInDeck.Where(card => !card.isVictory).Select(card => card.DefaultCoinCost).Sum()) /
                                     gameState.Self.CardsInDeck.Count();

            
            // hand + discard + the chancellor in play
            averageValueOfDiscard = ((double)gameState.Self.Discard.Where(card => !card.isVictory).Select(card => card.DefaultCoinCost).Sum() +
                                                gameState.Self.Hand.Where(card => !card.isVictory).Select(card => card.DefaultCoinCost).Sum() + Dominion.Cards.Chancellor.DefaultCoinCost) /
                                        (gameState.Self.Discard.Count() + gameState.Self.Hand.Count() + 1);

            // add 0.5 to average value of discard to prefer discarding.  This takes into account the advantages of preferring a quicker deck to get cards bought in play earlier.
            return averageValueOfDiscard + 0.5 > averageValueOfDeck;
        }

        /*
        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return true;
        }*/
        
        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {            
            return ShouldPutDeckInDiscardBasedOnAverageValue(gameState);                     
        }
    }
}