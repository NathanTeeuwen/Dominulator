using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Soothsayer :
        Card
    {
        public Soothsayer()
            : base("Soothsayer", coinCost: 5, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<Gold>(gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (otherPlayer.GainCardFromSupply<Curse>(gameState))
            {
                otherPlayer.DrawAdditionalCardsIntoHand(1);
            }
        }
    }
}