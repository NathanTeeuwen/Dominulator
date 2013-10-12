using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;
    
    public class Potion 
        : Card 
    { 
        public static Potion card; private Potion() 
            : base("Potion", coinCost: 4, isTreasure:true) 
        { 
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddPotions(1);
        }
    }

    public class Alchemist 
        : Card
    {
        public static Alchemist card; private Alchemist()
            : base("Alchemist", coinCost: 3, potionCost:1, isAction: true, plusCards:2, plusActions:1)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardsFromPlay(gameState,
                acceptableCard => acceptableCard == Alchemist.card,
                isOptional: true);
        }        
    }

    public class Apothecary
       : Card
    {
        public static Apothecary card; private Apothecary()
            : base("Apothecary", coinCost: 2, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);
            currentPlayer.MoveRevealedCardsToHand(card => card == Copper.card || card == Potion.card);
        }
    }

    public class Apprentice
       : Card
    {
        public static Apprentice card; private Apprentice()
            : base("Apprentice", coinCost: 5, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);

            if (card != null)
            {
                int cardsToDraw = card.CurrentCoinCost(currentPlayer) + 2 * card.potionCost;

                currentPlayer.DrawAdditionalCardsIntoHand(cardsToDraw);
            }
        }
    }

    public class Familiar
       : Card
    {
        public static Familiar card; private Familiar()
            : base("Familiar", coinCost: 3, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
 	         otherPlayer.GainCardFromSupply(Curse.card, gameState);
        }
    }

    public class Golem
       : Card
    {
        public static Golem card; private Golem()
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
        public static Herbalist card; private Herbalist()
            : base("Herbalist", coinCost: 2, isAction: true, plusCoins:1, plusBuy:1)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardFromPlay(gameState,
                acceptableCard => acceptableCard.isTreasure,
                isOptional: true);
        }
    }

    public class PhilosophersStone
       : Card
    {
        public static PhilosophersStone card; private PhilosophersStone()
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
        public static Possession card; private Possession()
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
        public static ScryingPool card; private ScryingPool()
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
        public static Transmute card; private Transmute()
            : base("Transmute", coinCost: 0, potionCost:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);

            if (trashedCard.isAction)
                currentPlayer.GainCardFromSupply(Duchy.card, gameState);

            if (trashedCard.isTreasure)
                currentPlayer.GainCardFromSupply(Transmute.card, gameState);

            if (trashedCard.isVictory)
                currentPlayer.GainCardFromSupply(Gold.card, gameState);
        }
    }

    public class University
       : Card
    {
        public static University card; private University()
            : base("University", coinCost: 2, potionCost:1, isAction: true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
 	         currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.isAction && card.CurrentCoinCost(currentPlayer) <= 5 && card.potionCost == 0,
                 "May gain an action card costing up to 5",
                 isOptional:true);
        }
    }

    public class Vineyard
       : Card
    {
        public static Vineyard card; private Vineyard()
            : base("Vineyard", coinCost: 0, potionCost:1, isAction: true, victoryPoints: playerState => playerState.AllOwnedCards.Where(card => card.isAction).Count()/3)
        {
        }        
    }
}
