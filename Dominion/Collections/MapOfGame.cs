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

        public T this[Game game]
        {
            set
            {                
                this.mapGameIndexToResult[game.GameIndex] = value;
            }

            get
            {
                return this.mapGameIndexToResult[game.GameIndex];
            }
        }

        public void InitAllEntries(Func<T> factory)
        {
            for (int i = 0; i < this.mapGameIndexToResult.Length; ++i)
            {
                this.mapGameIndexToResult[i] = factory();
            }
        }

        public IEnumerable<T> AllEntries
        {
            get
            {
                return this.mapGameIndexToResult;
            }
        }
    }
}
