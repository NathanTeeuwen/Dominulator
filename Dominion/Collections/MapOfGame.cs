using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Collections
{
    public class MapOfGame<T>
    {
        private readonly T[] mapGameIndexToResult;

        public MapOfGame()
        {
            this.mapGameIndexToResult = new T[Game.MaxSimultaneousGames];
        }

        public T this[GameState gameState]
        {
            set
            {                
                this.mapGameIndexToResult[gameState.InProgressGameIndex] = value;
            }

            get
            {                
                return this.mapGameIndexToResult[gameState.InProgressGameIndex];
            }
        }
    }
}
