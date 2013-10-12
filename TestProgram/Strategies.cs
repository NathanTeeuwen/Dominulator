using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {      
        private enum RelativeAmount
        {            
            LessThan,
            LessThanEqual,
            GreaterThan,
            GreaterThanEqual,
            Equal
        }

        private static GameStatePredicate CountAllOwned(Card card, RelativeAmount relativeAmount, int amount)
        {
            switch (relativeAmount)
            {
                case RelativeAmount.LessThan:          return delegate(GameState gameState) { return CountAllOwned(card, gameState) < amount; };
                case RelativeAmount.GreaterThan:       return delegate(GameState gameState) { return CountAllOwned(card, gameState) > amount; };
                case RelativeAmount.LessThanEqual:     return delegate(GameState gameState) { return CountAllOwned(card, gameState) <= amount; };
                case RelativeAmount.GreaterThanEqual:  return delegate(GameState gameState) { return CountAllOwned(card, gameState) >= amount; };
                case RelativeAmount.Equal:             return delegate(GameState gameState) { return CountAllOwned(card, gameState) == amount; };
                default: throw new System.Exception();
            }            
        }

        private static int CountAllOwned(ICardPicker matchingCards, GameState gameState)
        {
            return gameState.Self.AllOwnedCards.CountWhere(
                card => matchingCards.GetPreferredCard(gameState, testCard => testCard.Is(card)) != null);
        }

        private static int CountInDeck(Card card, GameState gameState)
        {
            return gameState.Self.CardsInDeck.CountOf(card);
        }

        private static int CountInDeckAndDiscard(Card card, GameState gameState)
        {
            var player = gameState.Self;
            return player.CardsInDeck.CountOf(card) + player.Discard.CountOf(card);
        }

        private static int CountMightDraw(Card card, GameState gameState, int maxCount)
        {
            if (gameState.Self.CardsInDeck.Count() >= maxCount)
                return CountInDeck(card, gameState);
            else
                return CountInDeckAndDiscard(card, gameState);            
        }

        public static bool CardBeingPlayedIs(Card card, GameState gameState)
        {
            var cardBeingPlayed = gameState.Self.CurrentCardBeingPlayed;
            return cardBeingPlayed != null && cardBeingPlayed == card;
        }

        public static int CostOfCard(Card card, GameState gameState)
        {
            return card.CurrentCoinCost(gameState.Self);
        }                

        public static int CountAllOwned(Card cardType, GameState gameState)
        {
            return gameState.Self.AllOwnedCards.CountOf(cardType);
        }

        public static int CountInHand(Card cardType, GameState gameState)
        {
            return gameState.Self.Hand.CountOf(cardType);
        }

        public static int CountOfPile(Card cardType, GameState gameState)
        {
            return gameState.GetPile(cardType).Count;
        }        

        private static int PlayersPointLead(GameState gameState)
        {
            int selfScore = gameState.Self.TotalScore();
            int maxOtherScore = 0;

            foreach (var player in gameState.players.AllPlayers)
            {
                if (player != gameState.Self)
                {
                    maxOtherScore = Math.Max(maxOtherScore, player.TotalScore());
                }
            }

            return selfScore - maxOtherScore;
        }

        private static GameStatePredicate HasCardInHand(Card card)
        {
            return delegate(GameState gameState)
            {
                return HasCardInHand(card, gameState);                
            };            
        }

        private static bool HasCardInHand(Card card, GameState gameState)
        {
            return gameState.Self.Hand.HasCard(card);
        }

        internal static Card WhichCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return matchingCards.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card));
        }        

        private static bool HasCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return WhichCardFromInHand(matchingCards, gameState) != null;
        }
      
        private static int CountInHandFrom(ICardPicker matchingCards, GameState gameState)
        {
            return gameState.Self.Hand.CountWhere(card => matchingCards.GetPreferredCard(gameState, current => current.Is(card)) != null);            
        }        
    }
}
