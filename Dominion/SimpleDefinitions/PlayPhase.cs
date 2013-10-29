using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum PlayPhase
    {
        Action,
        PlayTreasure,
        SpendCoinTokens,
        Buy,
        Cleanup,
        DrawCards,
        NotMyTurn
    }
}
