using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Advisor
        : Card
    {
        public Advisor()
            : base("Advisor", coinCost: 4, isAction: true,  plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Baker
        : Card
    {
        public Baker()
            : base("Baker", coinCost: 5, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(1);
        }
    }

    public class CandlestickMaker
        : Card
    {
        public CandlestickMaker()
            : base("CandlestickMaker", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(1);
        }
    }

    public class Doctor
        : Card
    {
        public Doctor()
            : base("Doctor", coinCost: 3, isAction: true, canOverpay: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Herald
       : Card
    {
        public Herald()
            : base("Herald", coinCost: 4, isAction: true, canOverpay: true, plusActions:1, plusCards:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }    


    public class Masterpiece
        : Card
    {
        public Masterpiece()
            : base("Masterpiece", coinCost: 3, isTreasure:true, canOverpay: true, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Plaza
        : Card
    {
        public Plaza()
            : base("Plaza", coinCost: 4, isAction: true, plusActions: 1, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.isTreasure, isOptional: true))
            {
                currentPlayer.AddCoinTokens(1);
            }
        }
    }    

    public class Soothsayer
        : Card
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

    public class StoneMason
        : Card
    {
        public StoneMason()
            : base("StoneMason", coinCost: 2, isAction: true, canOverpay: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Taxman
       : Card
    {
        public Taxman()
            : base("Taxman", coinCost: 4, isAction: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }   
}