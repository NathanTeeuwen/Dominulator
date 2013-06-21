using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GameConfig
    {
        internal readonly bool useShelters;
        internal readonly bool useColonyAndPlatinum;
        internal readonly Card[] supplyPiles;

        public GameConfig(bool useShelters, bool useColonyAndPlatinum, params Card[] supplyPiles)
        {
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.supplyPiles = supplyPiles;
        }

        public GameConfig(params Card[] supplyPiles)
        {
            this.useShelters = false;
            this.useColonyAndPlatinum = false;
            this.supplyPiles = supplyPiles;
        }
    }
}
