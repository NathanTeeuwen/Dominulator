using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public struct CardPlacementPair
    {
        public readonly Card card;
        public readonly DeckPlacement placement;

        public CardPlacementPair(Card card, DeckPlacement placement)
        {
            this.card = card;
            this.placement = placement;
        }
    }
}
