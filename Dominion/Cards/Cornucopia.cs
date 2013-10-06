using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Fairgrounds
       : Card
    {
        public Fairgrounds()
            : base("Fairgrounds", coinCost: 6, victoryPoints: playerState => playerState.AllOwnedCards.GroupBy(card => card.GetType()).Count() / 5 * 2)
        {            
        }
    }

    public class FarmingVillage 
        : Card
    {
        public FarmingVillage()
            : base("FarmingVillage", coinCost: 4, isAction: true, plusActions:2)
        {
        }
        
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card foundCard = null;
            gameState.gameLog.PushScope();
            while (true)
            {
                foundCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (foundCard == null)
                    break;

                if (foundCard.isAction || foundCard.isTreasure)
                {
                    break;
                }
            }
            gameState.gameLog.PopScope();

            if (foundCard != null)
            {
                currentPlayer.MoveRevealedCardToHand(foundCard);
            }

            currentPlayer.MoveRevealedCardsToDiscard(cardToMove => true, gameState);
        }
    }

    public class FortuneTeller
        : Card
    {
        public FortuneTeller()
            : base("FortuneTeller", coinCost: 3, isAction: true, plusCoins:2, isAttack:true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }        
    }

    public class Hamlet
        : Card
    {
        public Hamlet()
            : base("Hamlet", coinCost: 2, isAction: true, plusActions: 1, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO:  How does the player know whether he is discarding for action or buy?
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => true, isOptional: true))
            {
                currentPlayer.AddActions(1);
            }
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => true, isOptional: true))
            {
                currentPlayer.AddBuys(1);
            }
        }
    }

    public class Harvest
        : Card
    {
        public Harvest()
            : base("Harvest", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class HornOfPlenty
        : Card
    {
        public HornOfPlenty()
            : base("HornOfPlenty", coinCost: 5, isTreasure: true)
        {
        }
  
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class HorseTraders
        : Card
    {
        public HorseTraders()
            : base("HorseTraders", coinCost: 4, isAction: true, isReaction: true, plusBuy: 1, plusCoins: 3)
        {
        }
        
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            throw new NotImplementedException();
        }
    }


    public class HuntingParty
        : Card
    {
        public HuntingParty()
            : base("HuntingParty", coinCost: 5, isAction: true, plusActions: 1, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }        
    }

    public class Jester
        : Card
    {
        public Jester()
            : base("Jester", coinCost: 5, isAction: true, isAttack: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            Card discardedCard = otherPlayer.DiscardCardFromTopOfDeck();
            if (discardedCard != null)
            {
                if (discardedCard.isVictory)
                {
                    otherPlayer.GainCardFromSupply<CardTypes.Curse>(gameState);
                }
                else if (currentPlayer.actions.ShouldGainCard(gameState, discardedCard))
                {
                    currentPlayer.GainCardFromSupply(gameState, discardedCard);
                }
                else
                {
                    otherPlayer.GainCardFromSupply(gameState, discardedCard);
                }
            }
        }
    }

    public class Menagerie
        : Card
    {
        public Menagerie()
            : base("Menagerie", coinCost: 3, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Remake
        : Card
    {
        public Remake()
            : base("Remake", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            for (int i = 0; i < 2; ++i)
            {
                Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
                if (trashedCard != null)
                {
                    currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                        acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) == trashedCard.CurrentCoinCost(currentPlayer) + 1,
                        "Must gain a card costing exactly 1 more than the trashed card");
                }
            }
        }
    }

    public class Tournament
        : Card
    {
        public Tournament()
            : base("Tournament", coinCost: 4, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

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