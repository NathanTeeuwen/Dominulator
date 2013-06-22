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
        Type BanCardForCurrentPlayerRevealedCards(GameState gameState);
        Type BanCardForCurrentPlayerPurchase(GameState gameState);
        Type GetCardPileFromSupply(GameState gameState);
        Type GetActionFromHandToPlay(GameState gameState, bool isOptional);
        Type GetTreasureFromHandToPlay(GameState gameState);
        Type GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard);
        Type GuessCardTopOfDeck(GameState gameState);
        Type NameACard(GameState gameState);
        Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player);        
        Type GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard);
        Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player);
        Type GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player);
        Type GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard);
        Type GetCardFromHandToPassLeft(GameState gameState);
        Type GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional);
        Type GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard); // always optional
        Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        int GetNumberOfCardsFromDiscardToPutInHand(GameState gameState, int maxNumber);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);        
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState);
        bool ShouldTrashCard(GameState gameState, Card card);
        bool ShouldGainCard(GameState gameState, Card card);
        PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice);
        string PlayerName { get; }
    }
}
