using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class EmptyGameLog
        : IGameLog
    {
        public void Dispose()
        {
        }

        public void BeginRound()
        {
        }

        public void PushScope()
        {

        }

        public void PopScope()
        {

        }

        public void BeginTurn(PlayerState playerState)
        {
        }

        public void EndTurn(PlayerState playerState)
        {
        }

        public void PlayerBoughtCard(PlayerState playerState, Card card)
        {
        }

        public void GainedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {

        }

        public void PlayerNamedCard(PlayerState playerState, Card card)
        {

        }

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerGainedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerPutCardInHand(PlayerState playerState, Card card)
        {

        }

        public void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
        }

        public void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
        {

        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
        }

        public void EndGame(GameState gameState)
        {
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {
        }

        public void PlayerGainedActions(PlayerState playerState, int coinAmount)
        {

        }

        public void PlayerGainedBuys(PlayerState playerState, int coinAmount)
        {

        }

        public void LogDeck(PlayerState playerState)
        {
        }

        public void PlayerReturnedCardToHand(PlayerState playerState, Card card)
        {

        }

        public void PlayerSetAsideCardFromHandForNextTurn(PlayerState playerState, Card card)
        {

        }
    }
}
