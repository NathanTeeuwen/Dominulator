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

        public static int CountAllOwned<T>(GameState gameState)
            where T : Card, new()
        {
            return CountAllOwned(Card.Type<T>(), gameState);
        }

        private static GameStatePredicate CountAllOwned<T>(RelativeAmount relativeAmount, int amount)
            where T: Card, new()
        {
            switch (relativeAmount)
            {
                case RelativeAmount.LessThan:          return delegate(GameState gameState) { return CountAllOwned<T>(gameState) < amount; };
                case RelativeAmount.GreaterThan:       return delegate(GameState gameState) { return CountAllOwned<T>(gameState) > amount; };
                case RelativeAmount.LessThanEqual:     return delegate(GameState gameState) { return CountAllOwned<T>(gameState) <= amount; };
                case RelativeAmount.GreaterThanEqual:  return delegate(GameState gameState) { return CountAllOwned<T>(gameState) >= amount; };
                case RelativeAmount.Equal:             return delegate(GameState gameState) { return CountAllOwned<T>(gameState) == amount; };
                default: throw new System.Exception();
            }            
        }

        private static int CountAllOwned(ICardPicker matchingCards, GameState gameState)
        {
            return gameState.players.CurrentPlayer.AllOwnedCards.CountWhere(
                card => matchingCards.GetPreferredCard(gameState, testCard => testCard.Is(card)) != null);
        }

        private static int CountInDeck<T>(GameState gameState)
            where T : Card, new()
        {
            return gameState.players.CurrentPlayer.CardsInDeck.CountOf(Card.Type<T>());
        }

        private static int CountInDeckAndDiscard<T>(GameState gameState)
            where T : Card, new()
        {
            var player = gameState.players.CurrentPlayer;
            return player.CardsInDeck.CountOf<T>() + player.Discard.CountOf<T>();
        }

        private static int CountMightDraw<T>(GameState gameState, int maxCount)
            where T: Card, new()
        {
            if (gameState.players.CurrentPlayer.CardsInDeck.Count() >= maxCount)
                return CountInDeck<T>(gameState);
            else
                return CountInDeckAndDiscard<T>(gameState);            
        }

        public static bool CardBeingPlayedIs<T>(GameState gameState)
                where T : Card, new()
        {
            var cardBeingPlayed = gameState.players.CurrentPlayer.CurrentCardBeingPlayed;
            return cardBeingPlayed != null && cardBeingPlayed.Is<T>();
        }

        public static int CostOfCard<T>(GameState gameState)
            where T : Card, new()
        {
            return Card.Type<T>().CurrentCoinCost(gameState.players.CurrentPlayer);
        }                

        public static int CountAllOwned(Card cardType, GameState gameState)
        {
            return gameState.players.CurrentPlayer.AllOwnedCards.CountOf(cardType);
        }

        public static int CountInHand(Card cardType, GameState gameState)
        {
            return gameState.players.CurrentPlayer.Hand.CountOf(cardType);
        }

        public static int CountInHand<T>(GameState gameState)
            where T : Card, new()
        {
            return CountInHand(Card.Type<T>(), gameState);
        }

        public static int CountOfPile<T>(GameState gameState)
            where T: Card, new()
        {
            return CountOfPile(Card.Type<T>(), gameState);
        }

        public static int CountOfPile(Card cardType, GameState gameState)
        {
            return gameState.GetPile(cardType).Count;
        }        

        private static int PlayersPointLead(GameState gameState)
        {
            return gameState.players.CurrentPlayer.TotalScore() - gameState.players.OtherPlayers.First().TotalScore();
        }

        private static GameStatePredicate HasCardInHand<T>()
            where T: Card, new()
        {
            return delegate(GameState gameState)
            {
                return HasCardInHand<T>(gameState);                
            };            
        }

        private static bool HasCardInHand<T>(GameState gameState)
            where T: Card, new()
        {
            return gameState.players.CurrentPlayer.Hand.HasCard<T>();
        }

        internal static Card WhichCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return matchingCards.GetPreferredCard(gameState, card => gameState.players.CurrentPlayer.Hand.HasCard(card));
        }        

        private static bool HasCardFromInHand(ICardPicker matchingCards, GameState gameState)
        {
            return WhichCardFromInHand(matchingCards, gameState) != null;
        }
      
        private static int CountInHandFrom(ICardPicker matchingCards, GameState gameState)
        {
            return gameState.players.CurrentPlayer.Hand.CountWhere( card => matchingCards.GetPreferredCard(gameState, current => current.Is(card)) != null);            
        }                         
    }
}
