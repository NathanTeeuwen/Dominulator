using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy
{
    public class PlayerActionFromCardResponses
        : DerivedPlayerAction
    {
        private MapOfCards<IPlayerAction> cardResponses;

        public PlayerActionFromCardResponses(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
            this.cardResponses = DefaultPlayRules.DefaultResponses.GetCardResponses(playerAction);            
        }

        private IPlayerAction GetActionForCurrentCardContext(GameState gameState)
        {
            if (gameState.CurrentContext.CurrentCard == null)
                return null;

            return this.cardResponses[gameState.CurrentContext.CurrentCard];
        }           

        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCountToReturnToSupply(card, gameState);
            else
                return base.GetCountToReturnToSupply(card, gameState);
        }

        public override Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.BanCardToDrawnIntoHandFromRevealedCards(gameState);
            else
                return base.BanCardToDrawnIntoHandFromRevealedCards(gameState);
        }

        public override Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.BanCardForCurrentPlayerPurchase(gameState);
            else
                return base.BanCardForCurrentPlayerPurchase(gameState);
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ChooseCardToPlayFirst(gameState, card1, card2);
            else
                return base.ChooseCardToPlayFirst(gameState, card1, card2);
        }

        public override Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);
            else
                return base.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromSupplyToEmbargo(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromSupplyToEmbargo(gameState);
            else
                return base.GetCardFromSupplyToEmbargo(gameState);
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromSupplyToPlay(gameState, acceptableCard);
            else
                return base.GetCardFromSupplyToPlay(gameState, acceptableCard);
        }

        public override Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromSupplyToBuy(gameState, acceptableCard);
            else
                return base.GetCardFromSupplyToBuy(gameState, acceptableCard);
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }

        public override Card GuessCardTopOfDeck(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
            return playerAction.GuessCardTopOfDeck(gameState);
            return playerAction.GuessCardTopOfDeck(gameState);
        }

        public override Card NameACard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.NameACard(gameState);
            else
                return base.NameACard(gameState);
        }

        public override Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromDiscardToTopDeck(gameState, isOptional);
            else
                return base.GetCardFromDiscardToTopDeck(gameState, isOptional);
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromRevealedCardsToTopDeck(gameState, isOptional);
            else
                return base.GetCardFromRevealedCardsToTopDeck(gameState, isOptional);
        }

        public override Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
            else
                return base.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
        }

        public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromRevealedCardsToPutOnDeck(gameState);
            else
                return base.GetCardFromRevealedCardsToPutOnDeck(gameState);
        }

        public override Card GetCardFromRevealedCardsToDiscard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromRevealedCardsToDiscard(gameState);
            else
                return base.GetCardFromRevealedCardsToDiscard(gameState);
        }

        public override Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToDeferToNextTurn(gameState);
            else
                return base.GetCardFromHandToDeferToNextTurn(gameState);
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromHandToIsland(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToIsland(gameState);
            else
                return base.GetCardFromHandToIsland(gameState);
        }

        public override Card GetCardFromHandToPassLeft(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToPassLeft(gameState);
            else
                return base.GetCardFromHandToPassLeft(gameState);
        }

        public override Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToReveal(gameState, acceptableCard);
            else
                return base.GetCardFromHandToReveal(gameState, acceptableCard);
        }

        public override Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
            else
                return base.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, CollectionCards cardsTrashedSoFar)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandToTrash(gameState, acceptableCard, isOptional, cardsTrashedSoFar);
            else
                return base.GetCardFromHandToTrash(gameState, acceptableCard, isOptional, cardsTrashedSoFar);
        }

        public override Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);
            else
                return base.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);
        }

        public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            else
                return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
        }

        public override Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);
            else
                return base.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);
        }

        public override int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxNumber);
            else
                return base.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxNumber);
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldPlayerDiscardCardFromDeck(gameState, player, card);
            else
                return base.ShouldPlayerDiscardCardFromDeck(gameState, player, card);
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldPlayerDiscardCardFromHand(gameState, card);
            else
                return base.ShouldPlayerDiscardCardFromHand(gameState, card);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldRevealCardFromHandForCard(gameState, card, cardFor);
            else
                return base.ShouldRevealCardFromHandForCard(gameState, card, cardFor);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldRevealCardFromHand(gameState, card);
            else
                return base.ShouldRevealCardFromHand(gameState, card);
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldPutCardInHand(gameState, card);
            else
                return base.ShouldPutCardInHand(gameState, card);
        }

        public override bool WantToResign(GameState gameState)
        {
            return base.WantToResign(gameState);
        }

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldPutDeckInDiscard(gameState);
            else
                return base.ShouldPutDeckInDiscard(gameState);
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldPutCardOnTopOfDeck(card, gameState);
            else
                return base.ShouldPutCardOnTopOfDeck(card, gameState);
        }

        public override bool ShouldTrashCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldTrashCard(gameState, card);
            else
                return base.ShouldTrashCard(gameState, card);
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ShouldGainCard(gameState, card);
            else
                return base.ShouldGainCard(gameState, card);
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ChooseBetween(gameState, acceptableChoice);
            else
                return base.ChooseBetween(gameState, acceptableChoice);
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ChooseBetweenTrashAndTopDeck(gameState, card);
            else
                return base.ChooseBetweenTrashAndTopDeck(gameState, card);
        }

        public override DeckPlacement ChooseBetweenTrashTopDeckDiscard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.ChooseBetweenTrashTopDeckDiscard(gameState, card);
            else
                return base.ChooseBetweenTrashTopDeckDiscard(gameState, card);
        }
      
        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCoinAmountToOverpayForCard(gameState, card);
            else
                return base.GetCoinAmountToOverpayForCard(gameState, card);
        }

        public override int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCoinAmountToSpendInBuyPhase(gameState);
            else
                return base.GetCoinAmountToSpendInBuyPhase(gameState);
        }

        public override int GetCoinAmountToUseInButcher(GameState gameState)
        {
            IPlayerAction playerAction = this.GetActionForCurrentCardContext(gameState);
            if (playerAction != null)
                return playerAction.GetCoinAmountToUseInButcher(gameState);
            else
                return base.GetCoinAmountToUseInButcher(gameState);
        }
    }
}
