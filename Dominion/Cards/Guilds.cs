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

    public class Butcher
        : Card
    {
        public Butcher()
            : base("Butcher", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(2);
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: true);
            if (trashedCard == null)
                return;

            int coinCount = currentPlayer.actions.GetCoinAmountToUseInButcher(gameState);
            if (coinCount > currentPlayer.AvailableCoins)
                throw new Exception("Tried to use too many coins");
            
            currentPlayer.AddCoinTokens(-coinCount);

            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) == trashedCard.CurrentCoinCost(currentPlayer) + coinCount,
                "Must gain a card costing exactly equal to the cost of the card trashed plus any coin spent");
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
            Type cardType = currentPlayer.RequestPlayerNameACard(gameState);
            currentPlayer.RevealCardsFromDeck(3);

            while (currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                currentPlayer.MoveRevealedCardToTrash(cardType, gameState);
            }
            
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            for (int i = 0; i < overpayAmount; ++i)
            {
                if (!currentPlayer.deck.Any())
                    break;

                currentPlayer.LookAtCardsFromDeck(1);

                PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                    acceptableChoice => acceptableChoice == PlayerActionChoice.Trash ||
                                        acceptableChoice == PlayerActionChoice.Discard ||
                                        acceptableChoice == PlayerActionChoice.TopDeck);

                switch (choice)
                {
                    case PlayerActionChoice.Trash:
                        currentPlayer.RequestPlayerTrashLookedAtCard(gameState);
                        break;
                    case PlayerActionChoice.Discard:
                        currentPlayer.MoveLookedAtCardsToDiscard(gameState);
                        break;
                    case PlayerActionChoice.TopDeck:
                        currentPlayer.MoveLookedAtCardToTopOfDeck();
                        break;
                    default:
                        throw new Exception("Unhandled case");
                }
            }
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
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            currentPlayer.GainCardsFromSupply<CardTypes.Silver>(gameState, overpayAmount);
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
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: false);
            if (card != null)
            {
                for (int i = 0; i < 2; ++i)
                {
                    currentPlayer.RequestPlayerGainCardFromSupply(
                        gameState, 
                        acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) < card.CurrentCoinCost(currentPlayer),
                        "Must gain 2 cards less than the trashed card");
                }
            }
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            for (int i = 0; i < 2; ++i)
            {
                currentPlayer.RequestPlayerGainCardFromSupply(
                    gameState,
                    acceptableCard => acceptableCard.isAction && acceptableCard.CurrentCoinCost(currentPlayer) == overpayAmount,
                    "Must gain 2 action cards costing the amount overpaid");
            }
        }
    }

    public class Taxman
       : Card
    {
        public Taxman()
            : base("Taxman", coinCost: 4, isAction: true, isAttack: true, attackDependsOnPlayerChoice: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => acceptableCard.isTreasure, isOptional: true);

            PlayerState.AttackAction attackAction = this.DoEmptyAttack;

            if (trashedCard != null)
            {
                attackAction = delegate(PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
                {
                    if (otherPlayer.Hand.Count >= 5)
                    {
                        otherPlayer.DiscardCardFromHand(gameState, trashedCard);
                    }
                };

                currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                    acceptableCard => acceptableCard.isTreasure && acceptableCard.CurrentCoinCost(currentPlayer) <= trashedCard.CurrentCoinCost(currentPlayer) + 3,
                    "Gain a card costing up to 3 more than the trashed card",
                    isOptional: false,
                    defaultLocation: DeckPlacement.TopOfDeck);
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);
        }
       
    }   
}