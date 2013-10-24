using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;

namespace Program
{
    public class StatsPerTurnGameLog
        : EmptyGameLog
    {
        private CardGameSubset cardGameSubset;
        public PerTurnPlayerCounters coinToSpend;
        public PerTurnPlayerCounters victoryPointTotal;
        public PerTurnPlayerCounters ruinsGained;
        public MapOfCardsForGameSubset<PerTurnPlayerCounters> cardsTotalCount;
        public MapOfCardsForGameSubset<PerTurnPlayerCounters> cardsGain;
        public MapOfCardsForGameSubset<PlayerCounter> endOfGameCardCount;
        public PerTurnPlayerCounters cursesGained;        
        public PerTurnPlayerCounters cursesTrashed;
        public PerTurnPlayerCounters deckShuffleCount;
        public PerTurnPlayerCounters oddsOfBeingAheadOnRoundEnd;

        public StatsPerTurnGameLog(int playerCount, CardGameSubset gameSubset)
        {
            this.coinToSpend = new PerTurnPlayerCounters(playerCount);
            this.ruinsGained = new PerTurnPlayerCounters(playerCount);
            this.cursesGained = new PerTurnPlayerCounters(playerCount);            
            this.cursesTrashed = new PerTurnPlayerCounters(playerCount);            
            this.victoryPointTotal = new PerTurnPlayerCounters(playerCount);
            this.deckShuffleCount = new PerTurnPlayerCounters(playerCount);
            this.oddsOfBeingAheadOnRoundEnd = new PerTurnPlayerCounters(playerCount);

            this.cardGameSubset = gameSubset;

            this.cardsTotalCount = ContstructCounterPerTurn(playerCount, gameSubset); 
            this.cardsGain = ContstructCounterPerTurn(playerCount, gameSubset);
            this.endOfGameCardCount = ContstructCounter(playerCount, gameSubset); 
        }

        private MapOfCardsForGameSubset<PerTurnPlayerCounters> ContstructCounterPerTurn(int playerCount, CardGameSubset gameSubset)
        {
            var result = new MapOfCardsForGameSubset<PerTurnPlayerCounters>(gameSubset);
            foreach (Card card in gameSubset)
            {
                result[card] = new PerTurnPlayerCounters(playerCount);
            }

            return result;
        }

        private MapOfCardsForGameSubset<PlayerCounter> ContstructCounter(int playerCount, CardGameSubset gameSubset)
        {
            var result = new MapOfCardsForGameSubset<PlayerCounter>(gameSubset);
            foreach (Card card in gameSubset)
            {
                result[card] = new PlayerCounter(playerCount);
            }

            return result;
        }

        private void BeginTurnAllCountersPerCard(MapOfCardsForGameSubset<PerTurnPlayerCounters> map, PlayerState playerState)
        {
            foreach (Card card in cardGameSubset)
            {
                map[card].BeginTurn(playerState);
            }  
        }

        private void IncrementAllDivisors(MapOfCardsForGameSubset<PlayerCounter> map, PlayerState playerState)
        {
            foreach (Card card in cardGameSubset)
            {
                map[card].IncrementDivisor(playerState);
            }
        }

        public override void BeginTurn(PlayerState playerState)
        {
            this.coinToSpend.BeginTurn(playerState);
            this.ruinsGained.BeginTurn(playerState);
            this.cursesGained.BeginTurn(playerState);
            this.deckShuffleCount.BeginTurn(playerState);            
            this.cursesTrashed.BeginTurn(playerState);            
            this.victoryPointTotal.BeginTurn(playerState);
            BeginTurnAllCountersPerCard(this.cardsTotalCount, playerState);
            BeginTurnAllCountersPerCard(this.cardsGain, playerState);
        }

        public override void EndGame(GameState gameState)
        {
            foreach (PlayerState playerState in gameState.players.AllPlayers)
            {
                IncrementAllDivisors(this.endOfGameCardCount, playerState);
                foreach (Card card in this.cardGameSubset)
                {
                    this.endOfGameCardCount[card].IncrementCounter(playerState, playerState.AllOwnedCards.CountOf(card));
                }
            }
        }

        public override void EndRound(GameState gameState)
        {
            PlayerState winningPlayer = null;
            int winningScore = -1;

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                int score = player.TotalScore();
                if (score > winningScore)
                {                    
                    winningScore = score;
                    winningPlayer = player;
                }
                else if (score == winningScore)
                {
                    winningPlayer = null;
                }
            }

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                if (player == winningPlayer)
                {
                    oddsOfBeingAheadOnRoundEnd.IncrementCounter(player, 1);
                }
                oddsOfBeingAheadOnRoundEnd.BeginTurn(player);
            }
        }

        public override void EndTurn(PlayerState playerState)
        {
            this.victoryPointTotal.IncrementCounter(playerState, playerState.TotalScore());
            foreach (Card card in this.cardGameSubset)
            {
                this.cardsTotalCount[card].IncrementCounter(playerState, playerState.AllOwnedCards.CountOf(card));            
            }                    
        }        

        public override void PlayerTrashedCard(PlayerState playerState, Card card)
        {
            if (card.isCurse)
            {
                this.cursesTrashed.IncrementCounter(playerState, 1);
            }
        }

        public override void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {
            this.coinToSpend.IncrementCounter(playerState, coinAmount);
        }

        public override void PlayerBoughtCard(PlayerState playerState, Card card)
        {
            PlayerGainedOrBoughtCard(playerState, card);
        }

        public override void PlayerGainedCard(PlayerState playerState, Card card)
        {
            PlayerGainedOrBoughtCard(playerState, card);
        }

        private void PlayerGainedOrBoughtCard(PlayerState playerState, Card card)
        {
            if (card.isRuins)
            {
                this.ruinsGained.IncrementCounter(playerState, 1);
            }
            else if (card.isCurse)
            {
                this.cursesGained.IncrementCounter(playerState, 1);
            }
            else 
            {
                this.cardsGain[card].IncrementCounter(playerState, 1);                
            }
        }

        public override void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            this.deckShuffleCount.IncrementCounter(playerState, 1);            
        }        
    }     
}
