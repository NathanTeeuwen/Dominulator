using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Collections;

namespace Dominion.Data
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

    public class ForwardAndReversePerTurnPlayerCounters
    {
        public readonly PerTurnPlayerCountersSeparatedByGame forwardTotal;
        public readonly PerTurnPlayerCountersSeparatedByGame reverseTotal;
        private readonly PerTurnPlayerCountersSeparatedByGame currentGameData;

        public ForwardAndReversePerTurnPlayerCounters(int playerCount)
        {
            this.forwardTotal = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.reverseTotal = new PerTurnPlayerCountersSeparatedByGame(playerCount);
            this.currentGameData = new PerTurnPlayerCountersSeparatedByGame(playerCount);
        }        

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            currentGameData.IncrementCounter(playerState, amount);
        }

        public int GetSumForCurrentGameAndTurn(PlayerState playerState)
        {
            return this.currentGameData.GetValueAtTurn(playerState);
        }

        public void EndGame(GameState gameState, ForwardAndReversePerTurnPlayerCounters turnCounters)
        {            
            this.forwardTotal.Add(gameState, this.currentGameData);

            if (turnCounters != null)
            {
                this.forwardTotal.GrowToSize(gameState, turnCounters.forwardTotal);
                this.reverseTotal.GrowToSize(gameState, turnCounters.reverseTotal);
                this.currentGameData.GrowToSize(gameState, turnCounters.currentGameData);
            }
            this.currentGameData.Reverse(gameState);
            this.reverseTotal.Add(gameState, this.currentGameData);

            this.currentGameData.Clear(gameState);
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

        public int GetValueAtTurn(PlayerState playerState)
        {
            return this.counters[playerState.Game].GetSumAtTurn(playerState);
        }

        public float[] GetAveragePerTurn(int playerIndex, int throughTurn, PerTurnPlayerCountersSeparatedByGame counts)
        {
            AggregateAllDataIfNecessary();
            counts.AggregateAllDataIfNecessary();            
            return aggregatedResult.GetAveragePerTurn(playerIndex, throughTurn, counts.aggregatedResult);
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

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            GetCounter(playerState).IncrementCounter(playerState, amount);
        }

        public void Clear(GameState gameState)
        {
            this.counters[gameState.Game].Clear();
        }

        public void Add(GameState gameState, PerTurnPlayerCountersSeparatedByGame other)
        {
            this.counters[gameState.Game].Add(other.counters[gameState.Game]);
        }

        public void GrowToSize(GameState gameState, PerTurnPlayerCountersSeparatedByGame counts)
        {
            this.counters[gameState.Game].GrowToSize(counts.counters[gameState.Game]);
        }


        public void Reverse(GameState gameState)
        {
            this.counters[gameState.Game].Reverse();
        }
    }

    public class PerTurnPlayerCounters
    {        
        private List<int>[] sumAtTurnPerPlayer;        

        public PerTurnPlayerCounters(int playerCount)
        {
            this.sumAtTurnPerPlayer = new List<int>[2];
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {
                this.sumAtTurnPerPlayer[playerIndex] = new List<int>(capacity: 30);
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

        public int GetSumAtTurn(PlayerState player)
        {
            GrowListToTurn(this.sumAtTurnPerPlayer[player.PlayerIndex], player.TurnNumber);
            return this.sumAtTurnPerPlayer[player.PlayerIndex][player.TurnNumber];
        }

        // get the results
        public int PlayerTurnLength
        {
            get
            {
                return this.sumAtTurnPerPlayer[0].Count;
            }
        }

        public float[] GetAveragePerTurn(int playerIndex, int throughTurn, PerTurnPlayerCounters counts)
        {
            if (throughTurn > PlayerTurnLength)
            {
                throw new Exception("There aren't that many turns");
            }

            List<int> totalGoldPerTurn = this.sumAtTurnPerPlayer[playerIndex];
            List<int> totalCountOfThisTurn = counts.sumAtTurnPerPlayer[playerIndex];

            float[] result = new float[throughTurn];

            for (int turn = 1; turn <= throughTurn; ++turn)
            {
                result[turn - 1] = ((float)totalGoldPerTurn[turn]) / (totalCountOfThisTurn[turn] == 0 ? 1 : totalCountOfThisTurn[turn]);
            }

            return result;
        }

        public void Clear()
        {
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {
                this.sumAtTurnPerPlayer[playerIndex].Clear();                
            }
        }

        public void Add(PerTurnPlayerCounters other)
        {
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {
                var thisSumAtTurn = this.sumAtTurnPerPlayer[playerIndex];
                var otherSumAtTurn = other.sumAtTurnPerPlayer[playerIndex];

                GrowListToTurn(thisSumAtTurn, otherSumAtTurn.Count);

                for (int turn = 0; turn < otherSumAtTurn.Count; ++turn)
                {
                    thisSumAtTurn[turn] += otherSumAtTurn[turn];                    
                }
            }            
        }

        private static void Reverse<T>(List<T> list)
        {
            for (int index = 1, otherIndex = list.Count-1; index < list.Count / 2; ++index, otherIndex--)
            {
                T temp = list[index];
                list[index] = list[otherIndex];
                list[otherIndex] = temp;
            }
        }

        public void GrowToSize(PerTurnPlayerCounters counts)
        {
            while (this.sumAtTurnPerPlayer[0].Count < counts.sumAtTurnPerPlayer[0].Count)
            {
                this.GrowListsBy1();
            }
        }

        public void Reverse()
        {
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {
                Reverse(this.sumAtTurnPerPlayer[playerIndex]);                
            }
        }
      

        public void IncrementCounter(PlayerState playerState, int amount)
        {
            AddToCounterForPlayer(playerState.PlayerIndex, playerState.TurnNumber, amount, this.sumAtTurnPerPlayer);            
        }

        private void GrowListsBy1()
        {
            for (int playerIndex = 0; playerIndex < this.sumAtTurnPerPlayer.Length; ++playerIndex)
            {                
                this.sumAtTurnPerPlayer[playerIndex].Add(0);
            }
        }

        private void GrowListToTurn(List<int> list, int turn)
        {
            while (list.Count <= turn)
            {
                list.Add(0);
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
                result.Add(counter);                
            }

            return result;
        }
    }
}