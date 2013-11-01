using Dominion;
using System.Collections.Generic;

namespace Dominion.Strategy.DefaultPlayRules
{
    class CompareCardForBanningForDrawIntoHand
            : IComparer<Card>
    {
        GameState gameState;

        public CompareCardForBanningForDrawIntoHand(GameState gameState)
        {
            this.gameState = gameState;
        }

        public int Compare(Card first, Card second)
        {
            // no available actions.  Prefer to ban actions over non actions.
            if (gameState.players.CurrentPlayer.AvailableActions == 0)
            {
                if (first.isAction ^ second.isAction)
                {
                    return first.isAction ? -1 : 1;
                }
            }

            bool firstIsUseful = first.isTreasure || first.isAction;
            bool secondIsUseful = second.isTreasure || second.isAction;
            // only one of the cards is useful
            if (firstIsUseful ^ secondIsUseful)
            {
                return firstIsUseful ? -1 : 1;
            }

            int firstCost = first.CurrentCoinCost(gameState.players.CurrentPlayer);
            int secondCost = second.CurrentCoinCost(gameState.players.CurrentPlayer);
            if (firstCost != secondCost)
            {
                // ban most expensive card first.
                return firstCost > secondCost ? -1 : 1;
            }

            // dont care on order
            return 0;
        }
    }
}
