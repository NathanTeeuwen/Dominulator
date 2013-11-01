using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class MysticAndWishingWell
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public MysticAndWishingWell(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        override public Card GuessCardTopOfDeck(GameState gameState)
        {
            PlayerState self = gameState.Self;
            if (self.KnownCardsInDeck.Any())
            {
                return self.KnownCardsInDeck.First();
            }

            CollectionCards cards = self.CardsInDeck.Any ? self.CardsInDeck : self.Discard;
            if (cards.Any)
                return cards.MostCommonCardWhere(card => card != Dominion.Cards.Estate && !card.isShelter);
            else
                return Dominion.Cards.Estate;
        }
    }
}