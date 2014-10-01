using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IGameLog
        : IDisposable
    {
        void PushScope();
        void PopScope();
        void BeginRound(PlayerState playerState);
        void EndRound(GameState gameState);
        void BeginPhase(PlayerState playerState);
        void EndPhase(PlayerState playerState);
        void BeginTurn(PlayerState playerState);
        void EndTurn(PlayerState playerState);
        void PlayerNamedCard(PlayerState playerState, Card card);
        void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source);
        void PlayerBoughtCard(PlayerState playerState, Card card);        
        void PlayedCard(PlayerState playerState, Card card);
        void CardWentToLocation(DeckPlacement deckPlacement);
        void ReceivedDurationEffectFrom(PlayerState playerState, Card card);
        void PlayerGainedCard(PlayerState playerState, Card card);
        void PlayerDiscardCard(PlayerState playerState, Card card);
        void PlayerTrashedCard(PlayerState playerState, Card card);
        void PlayerPutCardInHand(PlayerState playerState, Card card);
        void PlayerTopDeckedCard(PlayerState playerState, Card card);
        void PlayerSetAsideCardFromHandForNextTurn(PlayerState playerState, Card card);
        void PlayerReturnedCardToHand(PlayerState playerState, Card card);
        void PlayerReturnedCardToPile(PlayerState playerState, Card card);
        void DrewCardIntoHand(PlayerState playerState, Card card);
        void DiscardedCard(PlayerState playerState, Card card);
        void ReshuffledDiscardIntoDeck(PlayerState playerState);
        void StartGame(GameState gameState);
        void EndGame(GameState gameState);
        void PlayerGainedPotion(PlayerState playerState, int potionCount);
        void PlayerGainedCoin(PlayerState playerState, int coinAmount);
        void PlayerGainedCoinToken(PlayerState playerState, int coinAmount);
        void PlayerGainedActions(PlayerState playerState, int actionAmount);
        void PlayerGainedBuys(PlayerState playerState, int actionAmount);
        void PlayerOverpaidForCard(Card boughtCard, int overPayAmount);
        void PlayerGainedVictoryTokens(PlayerState playerState, int amount);
    }
}
