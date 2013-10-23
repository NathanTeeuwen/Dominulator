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
                foreach (var list in this.totalPerTurnByPlayer)
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

        public float[] GetAveragePerTurn(int playerIndex, int throughTurn)
        {
            if (throughTurn > PlayerTurnCount)
            {
                throw new Exception("There aren't that many turns");
            }

            var totalGoldPerTurn = this.totalPerTurnByPlayer[playerIndex];
            var totalCountOfThisTurn = this.totalCountOfTurnPerPlayer[playerIndex];

            float[] result = new float[throughTurn];

            for (int turn = 1; turn <= throughTurn; ++turn)
            {
                result[turn - 1] = ((float)totalGoldPerTurn[turn]) / (totalCountOfThisTurn[turn] == 0 ? 1 : totalCountOfThisTurn[turn]);
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
}