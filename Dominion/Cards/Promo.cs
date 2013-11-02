using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class BlackMarket
       : Card
    {
        public static BlackMarket card = new BlackMarket();

        private BlackMarket()
            : base("Black Market", coinCost: 3, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            gameState.DoPlayTreasures(currentPlayer);
            PileOfCards pile = gameState.GetSpecialPile(typeof(BlackMarket));            
            throw new NotImplementedException();
        }
    }

    public class Envoy
       : Card
    {
        public static Envoy card = new Envoy();

        private Envoy()
            : base("Envoy", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(5);
            Card cardType = gameState.players.PlayerLeft.actions.BanCardToDrawnIntoHandFromRevealedCards(gameState);
            if (!currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                throw new Exception("Must ban a card currently being revealed");
            }
            currentPlayer.MoveRevealedCardToDiscard(cardType, gameState);
            currentPlayer.MoveAllRevealedCardsToHand();
        }
    }

    public class Governor
       : Card
    {
        public static Governor card = new Governor();

        private Governor()
            : base("Governor", coinCost: 5, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice playerChoice = currentPlayer.RequestPlayerChooseBetween(gameState, acceptableChoice =>
                acceptableChoice == PlayerActionChoice.GainCard ||
                acceptableChoice == PlayerActionChoice.PlusCard ||
                acceptableChoice == PlayerActionChoice.Trash);

            switch (playerChoice)
            {
                case PlayerActionChoice.PlusCard:
                    {
                        currentPlayer.DrawAdditionalCardsIntoHand(3);
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.DrawAdditionalCardsIntoHand(1);
                        }
                        break;
                    }
                case PlayerActionChoice.GainCard:
                    {
                        currentPlayer.GainCardFromSupply(gameState, Gold.card);
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.GainCardFromSupply(gameState, Silver.card);
                        }
                        break;
                    }
                case PlayerActionChoice.Trash:
                    {
                        currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                            gameState,
                            acceptableCardsToTrash => true,
                            CostConstraint.Exactly,
                            2,
                            CardRelativeCost.RelativeCost,
                            isOptionalToTrash: true,
                            isOptionalToGain: false);

                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                                gameState,
                                acceptableCardsToTrash => true,
                                CostConstraint.Exactly,
                                1,
                                CardRelativeCost.RelativeCost,
                                isOptionalToTrash: true,
                                isOptionalToGain: false);
                        }
                        break;
                    }
            }
        }
    }

    public class Stash
       : Card
    {
        public static Stash card = new Stash();

        private Stash()
            : base("Stash", coinCost: 2, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // on deck shuffles must give player an opporunity to place these anywhere.
            throw new NotImplementedException();
        }        
    }

    public class WalledVillage
       : Card
    {
        public static WalledVillage card = new WalledVillage();

        private WalledVillage()
            : base("Walled Village", coinCost: 4, plusCards: 1, plusActions: 2, isAction: true)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.cardsPlayed.CountWhere(card => card.isAction == true) <= 2)
            {
                currentPlayer.RequestPlayerTopDeckCardFromCleanup(this, gameState);                
            }
        }
    }
}