using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class BlackMarket :
        Card
    {
        public BlackMarket()
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

    public class Envoy :
        Card
    {
        public Envoy()
            : base("Envoy", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(5);
            Card cardType = gameState.players.PlayerLeft.actions.BanCardForCurrentPlayerRevealedCards(gameState);
            if (!currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                throw new Exception("Must ban a card currently being revealed");
            }
            currentPlayer.MoveRevealedCardToDiscard(cardType, gameState);
            currentPlayer.MoveAllRevealedCardsToHand();
        }
    }

    public class Governor :
        Card
    {
        public Governor()
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
                        currentPlayer.GainCardFromSupply(gameState, Card.Type<Gold>());
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.GainCardFromSupply(gameState, Card.Type<Silver>());
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

    public class Stash :
        Card
    {
        public Stash()
            : base("Stash", coinCost: 2, isTreasure: true)
        {
        }
    }

    public class WalledVillage :
        Card
    {
        public WalledVillage()
            : base("WalledVillage", coinCost: 4, plusCards: 1, plusActions: 2, isAction: true)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}