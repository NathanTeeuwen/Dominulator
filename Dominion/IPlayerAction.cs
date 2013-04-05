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
        Type GetActionFromHandToPlay(GameState gameState, bool isOptional);
        Type GetTreasureFromHandToPlay(GameState gameState);
        Type GetCardFromSupplyToBuy(GameState gameState);
        Type GuessCardTopOfDeck(GameState gameState);
        Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromRevealedCarsToTopDeck(BagOfCards revealedCards);
        Type GetCardFromRevealedCardsToTrash(PlayerState player, BagOfCards revealedCards, CardPredicate acceptableCard);
        Type GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard);
        Type GetCardFromHandToPassLeft(GameState gameState);
        Type GetCardFromHandToDiscard(GameState gameState, bool isOptional);
        Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);
        bool ShouldRevealCard(GameState gameState, Card card);
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldTrashCard(GameState gameState);
        bool ShouldGainCard(GameState gameState, Card card);
        PlayerActionChoice ChooseAction(GameState gameState, IsValidChoice acceptableChoice);
        string PlayerName { get; }
    }
}
