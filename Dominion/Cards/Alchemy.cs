using System;

namespace Dominion.CardTypes
{
    using Dominion;
    
    public class Potion 
        : Card 
    { 
        public static Potion card = new Potion();

        private Potion() 
            : base("Potion", Expansion.Alchemy, coinCost: 4, isTreasure:true) 
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
        public static Alchemist card = new Alchemist();

        private Alchemist()
            : base("Alchemist", Expansion.Alchemy, coinCost: 3, potionCost:1, isAction: true, plusCards:2, plusActions:1)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.cardsPlayed.HasCard(Cards.Potion))
            {
                currentPlayer.RequestPlayerTopDeckCardFromCleanup(this, gameState);                
            }
        }        
    }

    public class Apothecary
       : Card
    {
        public static Apothecary card = new Apothecary();

        private Apothecary()
            : base("Apothecary", Expansion.Alchemy, pluralName: "Apothecaries", coinCost: 2, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4, gameState);
            currentPlayer.MoveRevealedCardsToHand(card => card == Copper.card || card == Potion.card);
            currentPlayer.RequestPlayerTopDeckRevealedCardsInAnyOrder(gameState); 
        }
    }

    public class Apprentice
       : Card
    {
        public static Apprentice card = new Apprentice();

        private Apprentice()
            : base("Apprentice", Expansion.Alchemy, coinCost: 5, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);

            if (card != null)
            {
                int cardsToDraw = card.CurrentCoinCost(currentPlayer) + 2 * card.potionCost;

                currentPlayer.DrawAdditionalCardsIntoHand(cardsToDraw, gameState);
            }
        }
    }

    public class Familiar
       : Card
    {
        public static Familiar card = new Familiar();

        private Familiar()
            : base("Familiar", Expansion.Alchemy, coinCost: 3, potionCost: 1, isAction: true, plusCards: 1, plusActions: 1, isAttack:true)
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
        public static Golem card = new Golem();

        private Golem()
            : base("Golem", Expansion.Alchemy, coinCost: 4, potionCost: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            gameState.gameLog.PushScope();

            Card actionOne = DrawAndRevealTillFindAnActionIsntGolem(currentPlayer, gameState);            
            Card actionTwo = DrawAndRevealTillFindAnActionIsntGolem(currentPlayer, gameState);
            
            currentPlayer.MoveRevealedCardsToDiscard(cardToMove => !cardToMove.Equals(actionOne) && !cardToMove.Equals(actionTwo), gameState);
            // set the cards asside in case golem plays other cards that also must be revealed.
            currentPlayer.MoveRevealedCardsToSetAside();

            if (actionOne != null && actionTwo != null)
            {
                Card cardToPlayFirst = currentPlayer.actions.ChooseCardToPlayFirst(gameState, actionOne, actionTwo);
                if (cardToPlayFirst != actionOne && cardToPlayFirst != actionTwo)
                {
                    throw new Exception("Must pick one of the actions to player first");
                }

                // swap order;
                if (cardToPlayFirst == actionTwo)
                {
                    actionTwo = actionOne;
                    actionOne = cardToPlayFirst;
                }
            }

            if (actionOne != null)
            {                
                currentPlayer.cardsSetAside.RemoveCard(actionOne);
                currentPlayer.DoPlayAction(actionOne, gameState);                
            }

            if (actionTwo != null)
            {
                currentPlayer.cardsSetAside.RemoveCard(actionTwo);
                currentPlayer.DoPlayAction(actionTwo, gameState);
            }

            gameState.gameLog.PopScope();
        }

        private static Card DrawAndRevealTillFindAnActionIsntGolem(PlayerState currentPlayer, GameState gameState)
        {
            while (true)
            {
                Card result = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (result == null)
                    return null;

                if (result.isAction && result != Cards.Golem)
                {
                    return result;
                }
            }            
        }
    }

    public class Herbalist
      : Card
    {
        public static Herbalist card = new Herbalist();

        private Herbalist()
            : base("Herbalist", Expansion.Alchemy, coinCost: 2, isAction: true, plusCoins:1, plusBuy:1)
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
        public static PhilosophersStone card = new PhilosophersStone();

        private PhilosophersStone()
            : base("Philosopher's Stone", Expansion.Alchemy, coinCost: 3, potionCost:1, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int stoneValue = (currentPlayer.CardsInDeck.Count + currentPlayer.discard.Count) / 5;
            currentPlayer.AddCoins(stoneValue); 
        }
    }

    public class Possession
       : Card
    {
        public static Possession card = new Possession();

        private Possession()
            : base("Possession", Expansion.Alchemy, coinCost: 6, potionCost:1, isAction: true)
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
        public static ScryingPool card = new ScryingPool();

        private ScryingPool()
            : base("Scrying Pool", Expansion.Alchemy, coinCost: 2, potionCost:1, isAction: true, isAttack:true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerInspectTopOfDeckForDiscard(currentPlayer, gameState);
            while (true)
            {
                Card card = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (card == null || !card.isAction)
                    break;
            }
            currentPlayer.MoveAllRevealedCardsToHand();
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerInspectTopOfDeckForDiscard(currentPlayer, gameState);            
        }        
    }

    public class Transmute
       : Card
    {
        public static Transmute card = new Transmute();

        private Transmute()
            : base("Transmute", Expansion.Alchemy, coinCost: 0, potionCost:1, isAction: true)
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
        public static University card = new University();

        private University()
            : base("University", Expansion.Alchemy, coinCost: 2, potionCost:1, isAction: true, plusActions:2)
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
        public static Vineyard card = new Vineyard();

        private Vineyard()
            : base("Vineyard", Expansion.Alchemy, coinCost: 0, potionCost:1, victoryPoints: playerState => playerState.AllOwnedCards.CountWhere(card => card.isAction)/3)
        {
        }        
    }
}
