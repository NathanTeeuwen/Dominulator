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
        public PerTurnPlayerCountersSeparatedByGame coinToSpend;
        public PerTurnPlayerCountersSeparatedByGame cardsGained;
        public PerTurnPlayerCountersSeparatedByGame victoryPointTotal;
        public PerTurnPlayerCountersSeparatedByGame victoryPointTotalReversed;
        public PerTurnPlayerCountersSeparatedByGame victoryPointTotalTemporary;
        public PerTurnPlayerCountersSeparatedByGame ruinsGained;
        public MapOfCardsForGameSubset<PerTurnPlayerCountersSeparatedByGame> cardsTotalCount;
        public MapOfCardsForGameSubset<PerTurnPlayerCountersSeparatedByGame> carsGainedOnTurn;
        public MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> endOfGameCardCount;
        public PerTurnPlayerCountersSeparatedByGame cursesGained;
        public PerTurnPlayerCountersSeparatedByGame cursesTrashed;
        public PerTurnPlayerCountersSeparatedByGame deckShuffleCount;
        public PerTurnPlayerCountersSeparatedByGame oddsOfBeingAheadOnRoundEnd;

        public StatsPerTurnGameLog(int playerCount, CardGameSubset gameSubset)
        {
            this.coinToSpend = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.cardsGained = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.ruinsGained = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.cursesGained = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.cursesTrashed = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.victoryPointTotal = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.victoryPointTotalReversed = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.victoryPointTotalTemporary = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.deckShuffleCount = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.oddsOfBeingAheadOnRoundEnd = new PerTurnPlayerCountersSeparatedByGame(playerCount);

            this.cardGameSubset = gameSubset;

            this.cardsTotalCount = ContstructCounterPerTurn(playerCount, gameSubset); 
            this.carsGainedOnTurn = ContstructCounterPerTurn(playerCount, gameSubset);
            this.endOfGameCardCount = ContstructCounter(playerCount, gameSubset); 
        }

        private MapOfCardsForGameSubset<PerTurnPlayerCountersSeparatedByGame> ContstructCounterPerTurn(int playerCount, CardGameSubset gameSubset)
        {
            var result = new MapOfCardsForGameSubset<PerTurnPlayerCountersSeparatedByGame>(gameSubset);
            foreach (Card card in gameSubset)
            {
                result[card] = new PerTurnPlayerCountersSeparatedByGame(playerCount);
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

        private void BeginTurnAllCountersPerCard(MapOfCardsForGameSubset<PerTurnPlayerCountersSeparatedByGame> map, PlayerState playerState)
        {
            foreach (Card card in cardGameSubset)
            {
                map[card].BeginTurn(playerState);
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
            this.coinToSpend.BeginTurn(playerState);
            this.cardsGained.BeginTurn(playerState);
            this.ruinsGained.BeginTurn(playerState);
            this.cursesGained.BeginTurn(playerState);
            this.deckShuffleCount.BeginTurn(playerState);            
            this.cursesTrashed.BeginTurn(playerState);            
            this.victoryPointTotal.BeginTurn(playerState);
            this.victoryPointTotalTemporary.BeginTurn(playerState);
            BeginTurnAllCountersPerCard(this.cardsTotalCount, playerState);
            BeginTurnAllCountersPerCard(this.carsGainedOnTurn, playerState);
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

            this.victoryPointTotalTemporary.Reverse(gameState);
            this.victoryPointTotalReversed.Add(gameState, this.victoryPointTotalTemporary);

            this.victoryPointTotalTemporary.Clear(gameState);
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
            this.victoryPointTotalTemporary.IncrementCounter(playerState, playerState.TotalScore());
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
