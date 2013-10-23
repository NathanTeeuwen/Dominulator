using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;

namespace Program
{
    public class PerTurnPlayerCounters
    {        
        private List<int>[] totalPerTurnByPlayer;
        private List<int>[] totalCountOfTurnPerPlayer;

        public PerTurnPlayerCounters(int playerCount)
        {
            this.totalPerTurnByPlayer = new List<int>[2];
            for (int playerIndex = 0; playerIndex < this.totalPerTurnByPlayer.Length; ++playerIndex)
            {
                this.totalPerTurnByPlayer[playerIndex] = new List<int>(capacity: 30);
            }

            this.totalCountOfTurnPerPlayer = new List<int>[2];
            for (int playerIndex = 0; playerIndex < this.totalCountOfTurnPerPlayer.Length; ++playerIndex)
            {
                this.totalCountOfTurnPerPlayer[playerIndex] = new List<int>(capacity: 30);
            }            
        }

        public bool HasNonZeroData
        {
            get
            {
                foreach(var list in this.totalPerTurnByPlayer)
                {                    
                    for (int turn = 0; turn < list.Count; ++turn)
                    {
                        if (list[turn] > 0)
                            return true;
                    }                    
                }
                return false;
            }
        }

        // get the results
        public int PlayerTurnCount
        {
            get
            {
                return this.totalCountOfTurnPerPlayer[0].Count - 1;
            }
        }

        public float[] GetAveragePerTurn(int playerIndex)
        {
            var totalGoldPerTurn = this.totalPerTurnByPlayer[playerIndex];
            var totalCountOfThisTurn = this.totalCountOfTurnPerPlayer[playerIndex];

            float[] result = new float[totalGoldPerTurn.Count-1];

            for (int turn = 1; turn < result.Length; ++turn)
            {
                result[turn-1] = ((float)totalGoldPerTurn[turn]) / (totalCountOfThisTurn[turn] == 0 ? 1 : totalCountOfThisTurn[turn]);
            }

            return result;
        }

        // methods to be used by IGameLog
        public void BeginTurn(PlayerState playerState)
        {
            lock (this)
            {
                AddToCounterForPlayer(playerState, playerState.TurnNumber, 1, this.totalCountOfTurnPerPlayer);
            }
        }

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            lock (this)
            {
                AddToCounterForPlayer(playerState, playerState.TurnNumber, amount, this.totalPerTurnByPlayer);
            }
        }

        private void GrowListsBy1()
        {
            for (int playerIndex = 0; playerIndex < this.totalCountOfTurnPerPlayer.Length; ++playerIndex)
            {
                this.totalCountOfTurnPerPlayer[playerIndex].Add(0);
                this.totalPerTurnByPlayer[playerIndex].Add(0);
            }
        }

        private void AddToCounterForPlayer(PlayerState playerState, int turn, int value, List<int>[] listPerPlayer)
        {            
            List<int> list = listPerPlayer[playerState.PlayerIndex];
            while (list.Count <= turn)
            {
                this.GrowListsBy1();
            }

            list[turn] += value;
        }                    
    }

    public class StatsPerTurnGameLog
        : EmptyGameLog
    {
        public PerTurnPlayerCounters coinToSpend;
        public PerTurnPlayerCounters ruinsGained;
        public PerTurnPlayerCounters cursesGained;
        public PerTurnPlayerCounters cursesTotal;
        public PerTurnPlayerCounters provincesGained;
        public PerTurnPlayerCounters victoryPointTotal;

        public StatsPerTurnGameLog(int playerCount)
        {
            this.coinToSpend = new PerTurnPlayerCounters(playerCount);
            this.ruinsGained = new PerTurnPlayerCounters(playerCount);
            this.cursesGained = new PerTurnPlayerCounters(playerCount);
            this.cursesTotal = new PerTurnPlayerCounters(playerCount);
            this.provincesGained = new PerTurnPlayerCounters(playerCount);
            this.victoryPointTotal = new PerTurnPlayerCounters(playerCount);
        }        

        public override void BeginTurn(PlayerState playerState)
        {
            this.coinToSpend.BeginTurn(playerState);
            this.ruinsGained.BeginTurn(playerState);
            this.cursesGained.BeginTurn(playerState);
            this.cursesTotal.BeginTurn(playerState);
            this.provincesGained.BeginTurn(playerState);
            this.victoryPointTotal.BeginTurn(playerState);
        }

        public override void EndTurn(PlayerState playerState)
        {
            this.victoryPointTotal.IncrementCounter(playerState, playerState.TotalScore());
            this.cursesTotal.IncrementCounter(playerState, playerState.AllOwnedCards.CountOf(Cards.Curse));            
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
            else if (card == Cards.Province)
            {
                this.provincesGained.IncrementCounter(playerState, 1);
            }
        }
    }     
}
