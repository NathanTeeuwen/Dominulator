using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Collections;

namespace Program
{
    public class PlayerCounterSeparatedByGame
    {
        private int playerCount;
        private MapOfGame<PlayerCounter> counters;
        private PlayerCounter aggregatedResult = null;

        public PlayerCounterSeparatedByGame(int playerCount)
        {
            this.playerCount = playerCount;
            this.counters = new MapOfGame<PlayerCounter>();
            this.counters.InitAllEntries(delegate()
            {
                return new PlayerCounter(playerCount);
            });
        }

        private void AggregateAllDataIfNecessary()
        {
            if (aggregatedResult == null)
            {
                aggregatedResult = PlayerCounter.Sum(this.counters.AllEntries, playerCount);
            }
        }

        public float GetAverage(int playerIndex)
        {
            this.AggregateAllDataIfNecessary();
            return this.aggregatedResult.GetAverage(playerIndex);
        }

        public void IncrementDivisor(PlayerState playerState)
        {
            this.counters[playerState.Game].IncrementDivisor(playerState);
        }

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            this.counters[playerState.Game].IncrementCounter(playerState, amount);
        }
    }
    
    public class PlayerCounter
    {
        private readonly object theLock = new object();
        private readonly int[] totalDivisor;
        private readonly int[] totalCount;

        public PlayerCounter(int playerCount)
        {
            this.totalDivisor = new int[playerCount];
            this.totalCount = new int[playerCount];            
        }

        public float GetAverage(int playerIndex)
        {
            return (float)this.totalCount[playerIndex] / this.totalDivisor[playerIndex];
        }

        public void IncrementDivisor(PlayerState playerState)
        {
            this.totalDivisor[playerState.PlayerIndex]++;        
        }

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            this.totalCount[playerState.PlayerIndex] += amount;
        }
        
        static public PlayerCounter Sum(IEnumerable<PlayerCounter> counters, int playerCount)
        {
            var result = new PlayerCounter(playerCount);

            foreach (var counter in counters)
            {
                for (int playerIndex = 0; playerIndex < playerCount; ++playerIndex)
                {
                    result.totalDivisor[playerIndex] += counter.totalDivisor[playerIndex];
                    result.totalCount[playerIndex] += counter.totalCount[playerIndex];                    
                }               
            }

            return result;
        }
    }

    public class PerTurnPlayerCountersSeparatedByGame
    {
        private int playerCount;
        MapOfGame<PerTurnPlayerCounters> counters;
        PerTurnPlayerCounters aggregatedResult;

        public PerTurnPlayerCountersSeparatedByGame(int playerCount)
        {
            this.counters = new MapOfGame<PerTurnPlayerCounters>();
            this.playerCount = playerCount;
            this.counters.InitAllEntries(delegate()
            {
                return new PerTurnPlayerCounters(playerCount);
            });
            this.aggregatedResult = null;
        }

        public bool HasNonZeroData
        {
            get
            {
                AggregateAllDataIfNecessary();
                return aggregatedResult.HasNonZeroData;
            }
        }

        public float[] GetAveragePerTurn(int playerIndex, int throughTurn)
        {
            AggregateAllDataIfNecessary();
            return aggregatedResult.GetAveragePerTurn(playerIndex, throughTurn);
        }

        private void AggregateAllDataIfNecessary()
        {
            if (aggregatedResult == null)
            {
                aggregatedResult = PerTurnPlayerCounters.Sum(this.counters.AllEntries, playerCount);
            }
        }

        private PerTurnPlayerCounters GetCounter(PlayerState playerState)
        {
            return this.counters[playerState.Game];            
        }

        public void BeginTurn(PlayerState playerState)
        {
            GetCounter(playerState).BeginTurn(playerState);
        }

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            GetCounter(playerState).IncrementCounter(playerState, amount);
        }

    }

    public class PerTurnPlayerCounters
    {
        private readonly object theLock = new object();
        private List<int>[] sumAtTurnPerPlayer;
        private List<int>[] countAtTurnPerPlayer;

        public PerTurnPlayerCounters(int playerCount)
        {
            this.sumAtTurnPerPlayer = new List<int>[2];
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {
                this.sumAtTurnPerPlayer[playerIndex] = new List<int>(capacity: 30);
            }

            this.countAtTurnPerPlayer = new List<int>[2];
            for (int playerIndex = 0; playerIndex < this.countAtTurnPerPlayer.Length; ++playerIndex)
            {
                this.countAtTurnPerPlayer[playerIndex] = new List<int>(capacity: 30);
            }
        }

        public bool HasNonZeroData
        {
            get
            {
                foreach (var list in this.sumAtTurnPerPlayer)
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
                return this.countAtTurnPerPlayer[0].Count - 1;
            }
        }

        public float[] GetAveragePerTurn(int playerIndex, int throughTurn)
        {
            if (throughTurn > PlayerTurnCount)
            {
                throw new Exception("There aren't that many turns");
            }

            var totalGoldPerTurn = this.sumAtTurnPerPlayer[playerIndex];
            var totalCountOfThisTurn = this.countAtTurnPerPlayer[playerIndex];

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
            AddToCounterForPlayer(playerState.PlayerIndex, playerState.TurnNumber, 1, this.countAtTurnPerPlayer);            
        }

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            AddToCounterForPlayer(playerState.PlayerIndex, playerState.TurnNumber, amount, this.sumAtTurnPerPlayer);            
        }

        private void GrowListsBy1()
        {
            for (int playerIndex = 0; playerIndex < this.countAtTurnPerPlayer.Length; ++playerIndex)
            {
                this.countAtTurnPerPlayer[playerIndex].Add(0);
                this.sumAtTurnPerPlayer[playerIndex].Add(0);
            }
        }

        private void AddToCounterForPlayer(int playerIndex, int turn, int value, List<int>[] listPerPlayer)
        {
            List<int> list = listPerPlayer[playerIndex];
            while (list.Count <= turn)
            {
                this.GrowListsBy1();
            }

            list[turn] += value;
        }

        static public PerTurnPlayerCounters Sum(IEnumerable<PerTurnPlayerCounters> counters, int playerCount)
        {
            var result = new PerTurnPlayerCounters(playerCount);

            foreach (var counter in counters)
            {
                for (int turn = 0; turn < counter.PlayerTurnCount; ++turn)
                {
                    for (int playerIndex = 0; playerIndex < playerCount; ++playerIndex)
                    {
                        result.AddToCounterForPlayer(playerIndex, turn, counter.countAtTurnPerPlayer[playerIndex][turn], result.countAtTurnPerPlayer);
                        result.AddToCounterForPlayer(playerIndex, turn, counter.sumAtTurnPerPlayer[playerIndex][turn], result.sumAtTurnPerPlayer);            
                    }                    
                }
            }

            return result;
        }
    }
}