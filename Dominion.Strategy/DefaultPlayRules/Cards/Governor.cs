using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Governor
       : DerivedPlayerAction
    {
        public Governor(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if (gameState.Self.CardsInDeckAndDiscard.Count() > 3)
                return PlayerActionChoice.PlusCard;
            if (ShouldGainCard(gameState, Dominion.Cards.Province) && Strategy.CountInHand(Dominion.Cards.Gold, gameState) > 0)
                return PlayerActionChoice.Trash;            
            if (ShouldGainCard(gameState, Dominion.Cards.Gold))
                return PlayerActionChoice.GainCard;

            return PlayerActionChoice.PlusCard;
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, CollectionCards cardsTrashedSoFar)
        {
            if (gameState.Self == gameState.players.CurrentPlayer)
            {
                if (Strategy.CountInHand(Dominion.Cards.Gold, gameState) > 0)
                    return Dominion.Cards.Gold;
            }
            return base.GetCardFromHandToTrash(gameState, acceptableCard, isOptional, cardsTrashedSoFar);
        }     
    }
}