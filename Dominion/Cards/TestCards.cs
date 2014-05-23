using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes.TestCards
{
    // Test Cards

    public class FollowersTest
       : Card
    {
        public FollowersTest(int cost)
            : base("Followers", Expansion.Unknown, coinCost: cost, isAction: true, plusCards: 2, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Estate, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(Cards.Curse, gameState);
            otherPlayer.DiscardHandDownToCount(gameState, 3);
        }
    }

    public class FishingVillageAvailableForDeckCycle
       : Card
    {
        public static FishingVillageAvailableForDeckCycle card = new FishingVillageAvailableForDeckCycle();

        private FishingVillageAvailableForDeckCycle()
            : base("FishingVillageAvailableForDeckCycle", Expansion.Unknown, coinCost:3, isAction: true, plusCoins:1, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.actionsToExecuteAtBeginningOfNextTurn.Add( DelayedAction);
        }

        private static void DelayedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(1);
        }
    }

    public class FishingVillageEmptyDuration
       : Card
    {
        public static FishingVillageEmptyDuration card = new FishingVillageEmptyDuration();

        private FishingVillageEmptyDuration()
            : base("FishingVillageAvailableForDeckCycle", Expansion.Unknown, coinCost: 3, isAction: true, plusCoins: 2, plusActions: 2, isDuration:true )
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
        }
    }
}
