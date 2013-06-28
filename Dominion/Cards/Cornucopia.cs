using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class YoungWitch
       : Card
    {
        public YoungWitch()
            : base("Young Witch", coinCost: 4, plusCards: 2, isAction: true, isAttack: true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // TODO: BANE CARD
            otherPlayer.GainCardFromSupply<Curse>(gameState);
        }
    }

}