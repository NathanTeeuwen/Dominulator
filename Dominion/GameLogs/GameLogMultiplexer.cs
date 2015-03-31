using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{    
    public class GameLogMultiplexer
        : IGameLog
    {
        private readonly IGameLog[] gameLogs;

        public GameLogMultiplexer(params IGameLog[] gameLogs)
        {
            this.gameLogs = gameLogs;
        }

        public void Dispose()
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].Dispose();
            }
        }

        public void BeginRound(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].BeginRound(playerState);
            }
        }

        public void EndRound(GameState gameState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].EndRound(gameState);
            }
        }

        public void BeginPhase(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].BeginPhase(playerState);
            }
        }

        public void EndPhase(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].EndPhase(playerState);
            }
        }

        public void PushScope()
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PushScope();
            }
        }

        public void PopScope()
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PopScope();
            }
        }

        public void BeginTurn(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].BeginTurn(playerState);
            }
        }

        public void EndTurn(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].EndTurn(playerState);
            }
        }

        public void PlayerBoughtCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerBoughtCard(playerState, card);
            }
        }

        public void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerRevealedCard(playerState, card, source);
            }
        }

        public void PlayerNamedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerNamedCard(playerState, card);
            }
        }

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].DrewCardIntoHand(playerState, card);
            }
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].DiscardedCard(playerState, card);
            }
        }

        public void PlayerGainedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedCard(playerState, card);
            }
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card, DeckPlacement source)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerDiscardCard(playerState, card, source);
            }
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerTrashedCard(playerState, card);
            }
        }

        public void PlayerPutCardInHand(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerPutCardInHand(playerState, card);
            }
        }

        public void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerTopDeckedCard(playerState, card);
            }
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayedCard(playerState, card);
            }
        }

        public void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].ReceivedDurationEffectFrom(playerState, card);
            }
        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].ReshuffledDiscardIntoDeck(playerState);
            }
        }

        public void StartGame(GameState gameState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].StartGame(gameState);
            }
        }

        public void EndGame(GameState gameState)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].EndGame(gameState);
            }
        }

        public void PlayerGainedPotion(PlayerState playerState, int amount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedPotion(playerState, amount);
            }
        }

        public void PlayerGainedCoin(PlayerState playerState, int amount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedCoin(playerState, amount);
            }
        }

        public void PlayerGainedVictoryTokens(PlayerState playerState, int amount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedVictoryTokens(playerState, amount);
            }
        }

        public void PlayerGainedActions(PlayerState playerState, int amount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedActions(playerState, amount);
            }
        }

        public void PlayerGainedBuys(PlayerState playerState, int amount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedBuys(playerState, amount);
            }
        }      

        public void PlayerReturnedCardToHand(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerReturnedCardToHand(playerState, card);
            }
        }

        public void PlayerSetAsideCardFromHandForNextTurn(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerSetAsideCardFromHandForNextTurn(playerState, card);
            }
        }

        public void PlayerGainedCoinToken(PlayerState playerState, int coinAmount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerGainedCoinToken(playerState, coinAmount);
            }
        }

        public void PlayerOverpaidForCard(Card boughtCard, int overPayAmount)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerOverpaidForCard(boughtCard, overPayAmount);
            }
        }

        public void CardWentToLocation(DeckPlacement deckPlacement)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].CardWentToLocation(deckPlacement);
            }
        }

        public void PlayerReturnedCardToPile(PlayerState playerState, Card card)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerReturnedCardToPile(playerState, card);
            }
        }        

        public void PlayerChoseLocationForStash(PlayerState playerState, int[] positions)
        {
            for (int i = 0; i < this.gameLogs.Length; ++i)
            {
                this.gameLogs[i].PlayerChoseLocationForStash(playerState, positions);
            }
        }
    }
}
