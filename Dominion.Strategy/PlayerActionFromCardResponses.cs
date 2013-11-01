using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy
{
    class PlayerActionFromCardResponses
        : IPlayerAction
    {
        private MapOfCards<IPlayerAction> cardResponses;

        public PlayerActionFromCardResponses(MapOfCards<IPlayerAction> cardResponses)
        {
            this.cardResponses = cardResponses;
        }

        private IPlayerAction GetActionForCard(Card card)
        {
            return this.cardResponses[card];
        }

        private IPlayerAction GetActionForCardInPlay(GameState gameState)
        {
            if (gameState.CurrentCardBeingPlayed == null)
                return null;

            return this.cardResponses[gameState.CurrentCardBeingPlayed];
        }   

        public bool ShouldDeferForCardInPlay(GameState gameState)
        {
            return this.GetActionForCardInPlay(gameState) != null;
        }

        private IPlayerAction GetActionForCardBeingCleanedUp(GameState gameState)
        {
            if (gameState.Self.CurrentCardBeingCleanedUp == null)
                return null;

            return this.cardResponses[gameState.Self.CurrentCardBeingCleanedUp];
        }

        public bool ShouldDeferForCardBeingCleanedUp(GameState gameState)
        {
            return this.GetActionForCardBeingCleanedUp(gameState) != null;
        }

        public bool ShouldDeferForCard(Card card)
        {
            return this.GetActionForCard(card) != null;
        }

        public int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCountToReturnToSupply(card, gameState);
        }

        public Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.BanCardToDrawnIntoHandFromRevealedCards(gameState);
        }

        public Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.BanCardForCurrentPlayerPurchase(gameState);
        }

        public Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ChooseCardToPlayFirst(gameState, card1, card2);
        }

        public Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);
        }
        
        public Card GetCardFromSupplyToEmbargo(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromSupplyToEmbargo(gameState);
        }

        public Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromSupplyToPlay(gameState, acceptableCard);
        }
        
        public Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromSupplyToBuy(gameState, acceptableCard);
        }

        public Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }

        public Card GuessCardTopOfDeck(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GuessCardTopOfDeck(gameState);
        }

        public Card NameACard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.NameACard(gameState);
        }

        public Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
        }
        
        public Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardBeingCleanedUp(gameState);
            return playerAction.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
        }

        public Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromDiscardToTopDeck(gameState, isOptional);
        }

        public Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromRevealedCardsToTopDeck(gameState);
        }

        public Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
        }

        public Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromRevealedCardsToPutOnDeck(gameState);
        }

        public Card GetCardFromRevealedCardsToDiscard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromRevealedCardsToDiscard(gameState);
        }

        public Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToDeferToNextTurn(gameState);
        }

        public Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        public Card GetCardFromHandToIsland(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToIsland(gameState);
        }

        public Card GetCardFromHandToPassLeft(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToPassLeft(gameState);
        }

        public Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);
        }

        public Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToReveal(gameState, acceptableCard);
        }

        public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
        }

        public Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandToTrash(gameState, acceptableCard, isOptional);
        }

        public Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);
        }

        public Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
        }

        public Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);
        }

        public int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxNumber);
        }

        public bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ShouldPlayerDiscardCardFromDeck(gameState, player, card);
        }

        public bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCard(card);
            return playerAction.ShouldPlayerDiscardCardFromHand(gameState, card);
        }

        public bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            IPlayerAction playerAction = this.GetActionForCard(card);
            return playerAction.ShouldRevealCardFromHandForCard(gameState, card, cardFor);
        }

        public bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCard(card);
            return playerAction.ShouldRevealCardFromHand(gameState, card);
        }

        public bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ShouldPutCardInHand(gameState, card);
        }

        public bool WantToResign(GameState gameState)
        {
            throw new NotImplementedException();
        }

        public bool ShouldPutDeckInDiscard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ShouldPutDeckInDiscard(gameState);
        }

        public bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCard(card);
            return playerAction.ShouldPutCardOnTopOfDeck(card, gameState);
        }

        public bool ShouldTrashCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ShouldTrashCard(gameState, card);
        }

        public bool ShouldGainCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ShouldGainCard(gameState, card);
        }

        public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ChooseBetween(gameState, acceptableChoice);
        }

        public DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.ChooseBetweenTrashAndTopDeck(gameState, card);
        }

        public string PlayerName 
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCard(card);
            return playerAction.GetCoinAmountToOverpayForCard(gameState, card);
        }

        public int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCoinAmountToSpendInBuyPhase(gameState);
        }

        public int GetCoinAmountToUseInButcher(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCardInPlay(gameState);
            return playerAction.GetCoinAmountToUseInButcher(gameState);
        }
    }
}
