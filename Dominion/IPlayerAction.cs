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
        Card BanCardForCurrentPlayerRevealedCards(GameState gameState);
        Card BanCardForCurrentPlayerPurchase(GameState gameState);
        Card GetCardPileFromSupply(GameState gameState);
        Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard);
        Card GuessCardTopOfDeck(GameState gameState);
        Card NameACard(GameState gameState);
        Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromPlayToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player);
        Card GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard);
        Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player);
        Card GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player);
        Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToPassLeft(GameState gameState);
        Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional);
        Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard); // always optional
        Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToIsland(GameState gameState);
        Card GetCardFromHandToDeferToNextTurn(GameState gameState);
        Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer);
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
