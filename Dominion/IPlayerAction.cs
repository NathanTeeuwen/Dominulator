using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IPlayerAction
    {
        void BeginTurn();
        void EndTurn();
        int GetCountToReturnToSupply(Card card, GameState gameState);
        Type BanCardForCurrentPlayerRevealedCards(GameState gameState);
        Type BanCardForCurrentPlayerPurchase(GameState gameState);
        Type GetCardPileFromSupply(GameState gameState);
        Type GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard);
        Type GuessCardTopOfDeck(GameState gameState);
        Type NameACard(GameState gameState);
        Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromPlayToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player);        
        Type GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard);
        Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player);
        Type GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player);
        Type GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromHandToPassLeft(GameState gameState);
        Type GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional);
        Type GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard); // always optional
        Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        Type GetCardFromHandToIsland(GameState gameState);
        Type GetCardFromHandToDeferToNextTurn(GameState gameState);
        Type GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer);
        int GetNumberOfCardsFromDiscardToPutInHand(GameState gameState, int maxNumber);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);        
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState);
        bool ShouldTrashCard(GameState gameState, Card card);
        bool ShouldGainCard(GameState gameState, Card card);
        PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice);
        DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card);
        string PlayerName { get; }        
        int GetCoinAmountToOverpayForCard(GameState gameState, Card card);
        int GetCoinAmountToSpendInBuyPhase(GameState gameState);
        int GetCoinAmountToUseInButcher(GameState gameState);
    }
}
