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
        public ForwardAndReversePerTurnPlayerCounters turnCounters;
        public ForwardAndReversePerTurnPlayerCounters coinToSpend;
        public ForwardAndReversePerTurnPlayerCounters cardsGained;
        public ForwardAndReversePerTurnPlayerCounters victoryPointTotal;        
        public ForwardAndReversePerTurnPlayerCounters ruinsGained;
        public MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> cardsTotalCount;
        public MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> carsGainedOnTurn;
        public MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> endOfGameCardCount;
        public ForwardAndReversePerTurnPlayerCounters[] oddsOfHittingAtLeastACoinAmount;
        public ForwardAndReversePerTurnPlayerCounters cursesGained;
        public ForwardAndReversePerTurnPlayerCounters cursesTrashed;
        public ForwardAndReversePerTurnPlayerCounters deckShuffleCount;
        public ForwardAndReversePerTurnPlayerCounters oddsOfBeingAheadOnRoundEnd;

        public StatsPerTurnGameLog(int playerCount, CardGameSubset gameSubset)
        {
            this.turnCounters = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.coinToSpend = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.cardsGained = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.ruinsGained = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.cursesGained = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.cursesTrashed = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.victoryPointTotal = new ForwardAndReversePerTurnPlayerCounters(playerCount);            
            this.deckShuffleCount = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            this.oddsOfBeingAheadOnRoundEnd = new ForwardAndReversePerTurnPlayerCounters(playerCount);

            this.oddsOfHittingAtLeastACoinAmount = new ForwardAndReversePerTurnPlayerCounters[9];
            for (int i = 0; i < this.oddsOfHittingAtLeastACoinAmount.Length; ++i)
            {
                this.oddsOfHittingAtLeastACoinAmount[i] = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            }

            this.cardGameSubset = gameSubset;

            this.cardsTotalCount = ContstructCounterPerTurn(playerCount, gameSubset); 
            this.carsGainedOnTurn = ContstructCounterPerTurn(playerCount, gameSubset);
            this.endOfGameCardCount = ContstructCounter(playerCount, gameSubset); 
        }

        private MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> ContstructCounterPerTurn(int playerCount, CardGameSubset gameSubset)
        {
            var result = new MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters>(gameSubset);
            foreach (Card card in gameSubset)
            {
                result[card] = new ForwardAndReversePerTurnPlayerCounters(playerCount);
            }

            return result;
        }

        private MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> ContstructCounter(int playerCount, CardGameSubset gameSubset)
        {
            var result = new MapOfCardsForGameSubset<PlayerCounterSeparatedByGame>(gameSubset);
            foreach (Card card in gameSubset)
            {
                result[card] = new PlayerCounterSeparatedByGame(playerCount);
            }

            return result;
        }        

        private void EndGameAllCounters(IEnumerable<ForwardAndReversePerTurnPlayerCounters> counters, GameState gameState)
        {
            foreach (var counter in counters)
            {
                counter.EndGame(gameState, this.turnCounters);
            }
        }        

        private void EndGamePerCard(MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> map, GameState gameState)
        {
            foreach (Card card in cardGameSubset)
            {
                map[card].EndGame(gameState, this.turnCounters);
            }
        }

        private void IncrementAllDivisors(MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> map, PlayerState playerState)
        {
            foreach (Card card in cardGameSubset)
            {
                map[card].IncrementDivisor(playerState);
            }
        }

        public override void BeginTurn(PlayerState playerState)
        {
            this.turnCounters.IncrementCounter(playerState, 1);            
        }

        public override void EndGame(GameState gameState)
        {
            foreach (PlayerState playerState in gameState.players.OriginalPlayerOrder)
            {
                IncrementAllDivisors(this.endOfGameCardCount, playerState);
                foreach (Card card in this.cardGameSubset)
                {
                    this.endOfGameCardCount[card].IncrementCounter(playerState, playerState.AllOwnedCards.CountOf(card));
                }
            }

            this.turnCounters.EndGame(gameState, null);
            this.coinToSpend.EndGame(gameState, this.turnCounters);
            this.cardsGained.EndGame(gameState, this.turnCounters);
            this.victoryPointTotal.EndGame(gameState, this.turnCounters);
            this.ruinsGained.EndGame(gameState, this.turnCounters);
            EndGamePerCard(this.cardsTotalCount, gameState);
            EndGamePerCard(this.carsGainedOnTurn, gameState);
            EndGameAllCounters(this.oddsOfHittingAtLeastACoinAmount, gameState);
            this.cursesGained.EndGame(gameState, this.turnCounters);
            this.cursesTrashed.EndGame(gameState, this.turnCounters);
            this.deckShuffleCount.EndGame(gameState, this.turnCounters);
            this.oddsOfBeingAheadOnRoundEnd.EndGame(gameState, this.turnCounters);                                  
        }

        public override void EndRound(GameState gameState)
        {
            PlayerState winningPlayer = null;
            int winningScore = -1;

            foreach (PlayerState player in gameState.players.OriginalPlayerOrder)
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

            foreach (PlayerState player in gameState.players.OriginalPlayerOrder)
            {
                if (player == winningPlayer)
                {
                    oddsOfBeingAheadOnRoundEnd.IncrementCounter(player, 1);
                }                
            }
        }

        public override void EndTurn(PlayerState playerState)
        {
            this.victoryPointTotal.IncrementCounter(playerState, playerState.TotalScore());            
            foreach (Card card in this.cardGameSubset)
            {
                this.cardsTotalCount[card].IncrementCounter(playerState, playerState.AllOwnedCards.CountOf(card));            
            }

            int totalCoinSpent = this.coinToSpend.GetSumForCurrentGameAndTurn(playerState);
            for (int i = 1; i < this.oddsOfHittingAtLeastACoinAmount.Length; ++i)
            {
                if (totalCoinSpent >= i)
                {
                    this.oddsOfHittingAtLeastACoinAmount[i].IncrementCounter(playerState, 1);
                }
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
            this.cardsGained.IncrementCounter(playerState, 1);
            
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
                this.carsGainedOnTurn[card].IncrementCounter(playerState, 1);                
            }
        }

        public override void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            this.deckShuffleCount.IncrementCounter(playerState, 1);            
        }        
    }     
}
