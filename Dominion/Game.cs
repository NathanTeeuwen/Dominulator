using System;
using System.Collections.Generic;

namespace Dominion
{
    public class Game
        : IDisposable
    {
        public static readonly int MaxSimultaneousGames = 100;
        public static readonly int ExpectedNumberOfGameTurns = 30;
        public static readonly int ApproxNumberOfDifferentCards = 250;

        private int gameIndex = -1;
        internal readonly Random random;
        private readonly GameConfig gameConfig;
        private readonly IGameLog gameLog;

        public Game(Random random, GameConfig gameConfig, IGameLog gameLog)
        {
            this.random = random;
            this.gameConfig = gameConfig;
            this.gameLog = gameLog;
            this.gameIndex = Game.RecycledGameIndices.GetInteger();
        }

        public void Dispose()
        {
            if (this.gameIndex == -1)
                return;
            Game.RecycledGameIndices.ReturnInteger(this.gameIndex);
            this.gameIndex = -1;
        }

        public int GameIndex { get { return this.gameIndex; } }
        public GameConfig GameConfig { get { return this.gameConfig; } }
        public CardGameSubset CardGameSubset { get { return this.gameConfig.cardGameSubset; } }
        public IGameLog GameLog { get { return this.gameLog; } }

        private class IntCache
        {
            private readonly Stack<int> availableIntegers = new Stack<int>();
            private int nextInteger = 0;

            public int GetInteger()
            {
                lock (this.availableIntegers)
                {
                    if (this.availableIntegers.Count == 0)
                    {
                        if (this.nextInteger >= Game.MaxSimultaneousGames)
                        {
                            throw new Exception("Too many simultaneous Games");
                        }
                        return nextInteger++;
                    }

                    return this.availableIntegers.Pop();
                }
            }

            public void ReturnInteger(int integer)
            {
                System.Diagnostics.Debug.Assert(integer <= nextInteger);
                lock (this.availableIntegers)
                {
                    this.availableIntegers.Push(integer);
                }
            }
        }

        static private IntCache RecycledGameIndices = new IntCache();
    }
}
