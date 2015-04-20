using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy.Description
{
    // Win8client depends on these integer values
    public enum CountSource
    {
        CountOfPile = 0,
        CountAllOwned = 1,
        InHand = 2,
        Always = 3,
        AvailableCoin = 4
    }

}
