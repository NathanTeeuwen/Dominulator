using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class YoungWitch
        : DerivedPlayerAction
    {
        public YoungWitch(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            if (gameState.Self.Hand.HasCard(gameState.BaneCard))
                return gameState.BaneCard;

            return null;
        }                                        
    }
}