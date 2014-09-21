using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{    
    public class DerivedPlayerAction<TPlayerAction>
        : IPlayerAction
        where TPlayerAction : IPlayerAction
    {
        protected readonly TPlayerAction playerAction;

        public DerivedPlayerAction(TPlayerAction playerAction)
        {
            this.playerAction = playerAction;            
        }

        public virtual int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            return this.playerAction.GetCountToReturnToSupply(card, gameState);
        }

        public virtual Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState)
        {
            return this.playerAction.BanCardToDrawnIntoHandFromRevealedCards(gameState);            
        }

        public virtual Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            return this.playerAction.BanCardForCurrentPlayerPurchase(gameState);            
        }

        public virtual Card GetCardFromSupplyToEmbargo(GameState gameState)
        {
            return this.playerAction.GetCardFromSupplyToEmbargo(gameState);            
        }

        public virtual Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            return this.playerAction.GetCardFromSupplyToPlay(gameState, acceptableCard);            
        }

        public virtual Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);            
        }

        public virtual Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            return this.playerAction.ChooseCardToPlayFirst(gameState, card1, card2);            
        }

        public virtual Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);            
        }

        public virtual Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard)
        {
            return this.playerAction.GetCardFromSupplyToBuy(gameState, acceptableCard);            
        }

        public virtual Card GuessCardTopOfDeck(GameState gameState)
        {
            return this.playerAction.GuessCardTopOfDeck(gameState);            
        }

        public virtual Card NameACard(GameState gameState)
        {
            return this.playerAction.NameACard(gameState);            
        }

        public virtual Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);            
        }

        public virtual Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);            
        }

        public virtual Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);            
        }

        public virtual Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional)
        {
            return this.playerAction.GetCardFromDiscardToTopDeck(gameState, isOptional);            
        }

        public virtual Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            return this.playerAction.GetCardFromRevealedCardsToTopDeck(gameState);            
        }

        public virtual Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            return this.playerAction.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);            
        }

        public virtual Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            return this.playerAction.GetCardFromRevealedCardsToPutOnDeck(gameState);            
        }

        public virtual Card GetCardFromRevealedCardsToDiscard(GameState gameState)
        {
            return this.playerAction.GetCardFromRevealedCardsToDiscard(gameState);            
        }

        public virtual Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
        }

        public virtual Card GetCardFromHandToPassLeft(GameState gameState)
        {
            return this.playerAction.GetCardFromHandToPassLeft(gameState);            
        }

        public virtual Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromHandToDiscard(gameState, acceptableCard, isOptional);            
        }

        public virtual Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return this.playerAction.GetCardFromHandToReveal(gameState, acceptableCard);            
        }

        public virtual Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.GetCardFromHandToTrash(gameState, acceptableCard, isOptional); 
        }

        public virtual Card GetCardFromHandToIsland(GameState gameState)
        {
            return this.playerAction.GetCardFromHandToIsland(gameState);            
        }

        public virtual Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            return this.playerAction.GetCardFromHandToDeferToNextTurn(gameState);            
        }

        public virtual Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            return this.playerAction.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);            
        }

        public virtual Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            return this.playerAction.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);            
        }

        public virtual Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {
            return this.playerAction.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);            
        }

        public virtual int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber)
        {
            return this.playerAction.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxNumber);            
        }

        public virtual bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            return this.playerAction.ShouldPlayerDiscardCardFromDeck(gameState, player, card);            
        }

        public virtual bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            return this.playerAction.ShouldPlayerDiscardCardFromHand(gameState, card);            
        }

        public virtual bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return this.playerAction.ShouldRevealCardFromHandForCard(gameState, card, cardFor);            
        }

        public virtual bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return this.playerAction.ShouldRevealCardFromHand(gameState, card);            
        }

        public virtual bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return this.playerAction.ShouldPutCardInHand(gameState, card);            
        }

        public virtual bool WantToResign(GameState gameState)
        {
            return this.playerAction.WantToResign(gameState);            
        }

        public virtual bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return this.playerAction.ShouldPutDeckInDiscard(gameState);            
        }

        public virtual bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return this.playerAction.ShouldPutCardOnTopOfDeck(card, gameState);            
        }

        public virtual bool ShouldTrashCard(GameState gameState, Card card)
        {
            return this.playerAction.ShouldTrashCard(gameState, card);            
        }

        public virtual bool ShouldGainCard(GameState gameState, Card card)
        {
            return this.playerAction.ShouldGainCard(gameState, card);            
        }

        public virtual PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            return this.playerAction.ChooseBetween(gameState, acceptableChoice);            
        }

        public virtual DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            return this.playerAction.ChooseBetweenTrashAndTopDeck(gameState, card);            
        }

        public virtual DeckPlacement ChooseBetweenTrashTopDeckDiscard(GameState gameState, Card card)
        {
            return this.playerAction.ChooseBetweenTrashTopDeckDiscard(gameState, card);            
        }

        public string PlayerName
        {
            get
            {
                return this.playerAction.PlayerName;
            }
        }

        public virtual int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            return this.playerAction.GetCoinAmountToOverpayForCard(gameState, card);            
        }

        public virtual int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            return this.playerAction.GetCoinAmountToSpendInBuyPhase(gameState);            
        }

        public virtual int GetCoinAmountToUseInButcher(GameState gameState)
        {
            return this.playerAction.GetCoinAmountToUseInButcher(gameState);
        }
    }
}
