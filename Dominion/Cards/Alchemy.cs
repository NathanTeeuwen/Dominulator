using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;
    
    public class Potion : Card { public Potion() : base("Potion", coinCost: 4) { } }

    public class Alchemist 
        : Card
    {
        public Alchemist()
            : base("Alchemist", coinCost: 3, potionCost:1, isAction: true, plusCards:2, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Apothecary
       : Card
    {
        public Apothecary()
            : base("Apothecary", coinCost: 2, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Apprentice
       : Card
    {
        public Apprentice()
            : base("Apprentice", coinCost: 5, , isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Familiar
       : Card
    {
        public Familiar()
            : base("Familiar", coinCost: 3, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
 	         otherPlayer.GainCardFromSupply<Curse>(gameState);
        }
    }

    public class Golem
       : Card
    {
        public Golem()
            : base("Golem", coinCost: 4, potionCost: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class Herbalist :
       Card
    {
        public Herbalist()
            : base("Herbalist", coinCost: 2, isAction: true, plusCoins:1, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class PhilosophersStone
       : Card
    {
        public PhilosophersStone()
            : base("PhilosophersStone", coinCost: 3, potionCost:1, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class Possession
       : Card
    {
        public Possession()
            : base("Possession", coinCost: 6, potionCost:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class ScryingPool
       : Card
    {
        public ScryingPool()
            : base("ScryingPool", coinCost: 2, potionCost:1, isAction: true, isAttack:true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class Transmute
       : Card
    {
        public Transmute()
            : base("Transmute", coinCost: 0, potionCost:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         throw new NotImplementedException();
        }
    }

    public class University
       : Card
    {
        public University()
            : base("University", coinCost: 2, potionCost:1, isAction: true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) <= 5,
                 "May gain a card costing up to 5",
                 isOptional:true);
        }
    }

    public class Vineyard
       : Card
    {
        public Vineyard()
            : base("Vineyard", coinCost: 0, potionCost:1, isAction: true, victoryPoints: playerState => playerState.Hand.Where(card => card.isAction).Count()/3)
        {
        }        
    }
}
