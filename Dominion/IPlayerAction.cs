using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IPlayerAction
    {        
        int GetCountToReturnToSupply(Card card, GameState gameState);
        Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState);
        Card BanCardForCurrentPlayerPurchase(GameState gameState);        
        Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2);        
        Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromSupplyToEmbargo(GameState gameState);
        Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GuessCardTopOfDeck(GameState gameState);
        Card NameACard(GameState gameState);
        Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional);
        Card GetCardFromRevealedCardsToTopDeck(GameState gameState);
        Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState);
        Card GetCardFromRevealedCardsToDiscard(GameState gameState);
        Card GetCardFromHandToDeferToNextTurn(GameState gameState);
        Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToIsland(GameState gameState);
        Card GetCardFromHandToPassLeft(GameState gameState);        
        Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard); // always optional
        Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, CollectionCards cardsTrashedSoFar);        
        Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement);
        Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer);
        Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard);
        int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card);
        bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor);
        bool ShouldRevealCardFromHand(GameState gameState, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);        
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState);
        bool ShouldTrashCard(GameState gameState, Card card);
        bool ShouldGainCard(GameState gameState, Card card);
        PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice);
        DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card);
        DeckPlacement ChooseBetweenTrashTopDeckDiscard(GameState gameState, Card card);
        string PlayerName { get; }        
        int GetCoinAmountToOverpayForCard(GameState gameState, Card card);
        int GetCoinAmountToSpendInBuyPhase(GameState gameState);
        int GetCoinAmountToUseInButcher(GameState gameState);
    }   
}
