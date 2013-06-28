using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes.TestCards
{
    // Test Cards

    public class FollowersTest :
        Card
    {
        public FollowersTest(int cost)
            : base("Followers", coinCost: cost, isAction: true, plusCards: 2, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<CardTypes.Estate>(gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply<CardTypes.Curse>(gameState);
            otherPlayer.DiscardHandDownToCount(gameState, 3);
        }
    }

}
