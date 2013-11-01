using Dominion;
using System;
using System.Linq;

namespace Program.DefaultPlayRules
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
                return cards.MostCommonCardWhere(card => card != Cards.Estate && !card.isShelter);
            else
                return Cards.Estate;
        }
    }
}