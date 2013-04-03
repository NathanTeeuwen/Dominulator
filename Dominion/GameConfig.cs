using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GameConfig
    {
        internal readonly bool useColonyAndPlatinum;
        internal readonly Card[] supplyPiles;

        public GameConfig(bool useColonyAndPlatinum, params Card[] supplyPiles)
        {
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.supplyPiles = supplyPiles;
        }

        public GameConfig(params Card[] supplyPiles)
        {
            this.useColonyAndPlatinum = false;
            this.supplyPiles = supplyPiles;
        }
    }
}
