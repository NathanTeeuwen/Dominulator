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

        public virtual void BeginRound(PlayerState playerState)
        {
        }

        public virtual void EndRound(GameState gameState)
        {
        }

        public virtual void PushScope()
        {

        }

        public virtual void PopScope()
        {

        }

        public virtual void BeginTurn(PlayerState playerState)
        {
        }

        public virtual void EndTurn(PlayerState playerState)
        {
        }

        public virtual void PlayerBoughtCard(PlayerState playerState, Card card)
        {
        }

        public void BeginPhase(PlayerState playerState)
        {

        }

        public void EndPhase(PlayerState playerState)
        {

        }

        public virtual void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {

        }

        public virtual void PlayerNamedCard(PlayerState playerState, Card card)
        {

        }

        public virtual void DrewCardIntoHand(PlayerState playerState, Card card)
        {
        }

        public virtual void DiscardedCard(PlayerState playerState, Card card)
        {
        }

        public virtual void PlayerGainedCard(PlayerState playerState, Card card)
        {
        }

        public virtual void PlayerDiscardCard(PlayerState playerState, Card card, DeckPlacement source)
        {
        }

        public virtual void PlayerTrashedCard(PlayerState playerState, Card card)
        {
        }

        public virtual void PlayerPutCardInHand(PlayerState playerState, Card card)
        {

        }

        public virtual void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
        }

        public virtual void PlayedCard(PlayerState playerState, Card card)
        {
        }

        public virtual void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
        {

        }

        public virtual void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
        }

        public virtual void StartGame(GameState gameState)
        {
        }

        public virtual void EndGame(GameState gameState)
        {
        }

        public virtual void PlayerGainedPotion(PlayerState playerState, int coinAmount)
        {
        }

        public virtual void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {
        }

        public virtual void PlayerGainedActions(PlayerState playerState, int coinAmount)
        {

        }

        public virtual void PlayerGainedBuys(PlayerState playerState, int coinAmount)
        {

        }

        public virtual void LogDeck(PlayerState playerState)
        {
        }

        public virtual void PlayerReturnedCardToHand(PlayerState playerState, Card card)
        {

        }

        public virtual void PlayerSetAsideCardFromHandForNextTurn(PlayerState playerState, Card card)
        {

        }

        public virtual void PlayerGainedCoinToken(PlayerState playerState, int coinAmount)
        {

        }

        public virtual void PlayerOverpaidForCard(Card boughtCard, int overPayAmount)
        {

        }

        public virtual void CardWentToLocation(DeckPlacement deckPlacement)
        {

        }

        public virtual void PlayerReturnedCardToPile(PlayerState playerState, Card card)
        {

        }   
     
        public virtual void PlayerGainedVictoryTokens(PlayerState playerState, int amount)
        {

        }
    }
}
