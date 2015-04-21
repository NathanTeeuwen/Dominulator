using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy.Description
{
    // Win8client depends on these integer values.  The order of the items in the combo box is assumed to match
    public enum CountSource
    {
        Always = 0,
        CountOfPile = 1,
        CountAllOwned = 2,
        CountInHand = 3,
        CardBeingPlayedIs = 4,
        AvailableCoin = 5
    }

}
