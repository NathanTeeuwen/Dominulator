using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class GreatHall : Card { public GreatHall() : base("Great Hall", coinCost: 3, victoryPoints: playerState => 1, plusCards: 1, plusActions: 1, isAction: true) { } }
    public class Harem : Card { public Harem() : base("Harem", coinCost: 6, victoryPoints: playerState => 2, plusCoins: 2, isTreasure: true) { } }

    public class Baron :
        Card
    {
        public Baron()
            : base("Baron", coinCost: 4, plusBuy: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => acceptableCard.Is<Estate>(), isOptional: true))
            {
                currentPlayer.AddCoins(4);
            }
            else
            {
                currentPlayer.GainCardFromSupply(gameState, typeof(Estate));
            }
        }
    }

    public class Bridge :
        Card
    {
        public Bridge()
            : base("Bridge", coinCost: 4, plusBuy: 1, plusCoins: 1, isAction: true)
        {
        }

        public override int ProvideDiscountForWhileInPlay(Card card)
        {
            return 1;
        }
    }

    public class Conspirator :
        Card
    {
        public Conspirator()
            : base("Conspirator", coinCost: 4, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.CountCardsPlayedThisTurn >= 3)
            {
                currentPlayer.DrawOneCardIntoHand();
                currentPlayer.AddActions(1);
            }
        }
    }

    public class Coppersmith :
        Card
    {
        public Coppersmith()
            : base("Coppersmith", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.turnCounters.copperAdditionalValue += 1;
        }
    }

    public class Courtyard :
        Card
    {
        public Courtyard()
            : base("Courtyard", coinCost: 2, plusCards: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false);
        }
    }

    public class Duke :
        Card
    {
        public Duke()
            : base("Duke", coinCost: 5, isAction: true)
        {
            this.victoryPointCounter = player => player.AllOwnedCards.Where(card => card.Is<Duchy>()).Count();
        }
    }

    public class IronWorks :
        Card
    {
        public IronWorks()
            : base("IronWorks", coinCost: 4, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, acceptableCard => true, "Any card");

            if (gainedCard.isAction)
            {
                currentPlayer.AddActions(1);
            }

            if (gainedCard.isTreasure)
            {
                currentPlayer.AddCoins(1);
            }

            if (gainedCard.isVictory)
            {
                currentPlayer.DrawOneCardIntoHand();
            }
        }
    }

    public class Masquerade :
        Card
    {
        public Masquerade()
            : base("Masquerade", coinCost: 3, plusCards: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                player.cardToPass = player.RequestPlayerGiveCardToPassLeft(gameState);
            }

            for (int playerIndex = 0; playerIndex < gameState.players.PlayerCount; ++playerIndex)
            {
                Card card = gameState.players[playerIndex].cardToPass;
                if (card != null)
                {
                    gameState.players[playerIndex + 1].hand.AddCard(card);
                }
            }

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                player.cardToPass = null;
            }

            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: true);
        }
    }

    public class MiningVillage :
        Card
    {
        public MiningVillage()
            : base("MiningVillage", coinCost: 4, plusCards: 1, plusActions: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldTrashCard(gameState, this))
            {
                if (currentPlayer.MoveCardFromPlayToTrash(gameState))
                {
                    gameState.gameLog.PushScope();
                    currentPlayer.AddCoins(2);
                    gameState.gameLog.PopScope();
                }
            }
        }
    }

    public class Minion :
        Card
    {
        public Minion()
            : base("Minion", coinCost: 5, plusActions: 1, isAttack: true, attackDependsOnPlayerChoice: false, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // Choose one: ...
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard || acceptableChoice == PlayerActionChoice.PlusCoin);

            if (actionChoice == PlayerActionChoice.PlusCoin)
            {
                // +2 coin;
                currentPlayer.AddCoins(2);
                foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                {
                    otherPlayer.IsAffectedByAttacks(gameState); // trigger reactions;
                }
            }
            else
            {
                // discard your hand, 
                currentPlayer.DiscardHand();
                // +4 cards
                currentPlayer.DrawAdditionalCardsIntoHand(4);

                // and each other player 
                foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                {
                    if (!otherPlayer.IsAffectedByAttacks(gameState))
                    {
                        continue;
                    }

                    // with at least 5 cards in hand
                    if (otherPlayer.hand.Count >= 5)
                    {
                        // discards his hand
                        otherPlayer.DiscardHand();
                        // and draws 4 cards
                        otherPlayer.DrawAdditionalCardsIntoHand(4);
                    }
                }
            }
        }
    }

    public class Nobles :
        Card
    {
        public Nobles()
            : base("Nobles", coinCost: 6, victoryPoints: playerState => 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.PlusCard || acceptableChoice == PlayerActionChoice.PlusAction);

            if (actionChoice == PlayerActionChoice.PlusCard)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(3);
            }
            else
            {
                currentPlayer.AddActions(2);
            }
        }
    }

    public class Pawn :
        Card
    {
        public Pawn()
            : base("Pawn", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            IsValidChoice acceptableFirstChoice = delegate(PlayerActionChoice acceptableChoice)
            {
                return acceptableChoice == PlayerActionChoice.PlusCard ||
                       acceptableChoice == PlayerActionChoice.PlusAction ||
                       acceptableChoice == PlayerActionChoice.PlusBuy ||
                       acceptableChoice == PlayerActionChoice.PlusCoin;
            };

            PlayerActionChoice firstChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableFirstChoice);

            PlayerActionChoice secondChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableSecondChoice => acceptableSecondChoice != firstChoice && acceptableFirstChoice(acceptableSecondChoice));

            DoActionChoice(currentPlayer, firstChoice);
            DoActionChoice(currentPlayer, secondChoice);
        }

        private void DoActionChoice(PlayerState currentPlayer, PlayerActionChoice actionChoice)
        {
            switch (actionChoice)
            {
                case PlayerActionChoice.PlusCard: currentPlayer.DrawOneCardIntoHand(); break;
                case PlayerActionChoice.PlusAction: currentPlayer.AddActions(1); break;
                case PlayerActionChoice.PlusBuy: currentPlayer.AddBuys(1); break;
                case PlayerActionChoice.PlusCoin: currentPlayer.AddCoins(1); break;
                default: throw new Exception("Invalid pawn action choice");
            }
        }
    }

    public class Sabateur :
        Card
    {
        public Sabateur()
            : base("Sabateur", coinCost: 5, isAttack: true, isAction: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // Each other player reveals cards from the top of his deck 
            Card revealedCard = otherPlayer.DrawAndRevealOneCardFromDeck();
            while (revealedCard != null)
            {
                // until revealing one costing 3 or more
                if (revealedCard.CurrentCoinCost(otherPlayer) >= 3)
                {
                    break;
                }
                revealedCard = otherPlayer.DrawAndRevealOneCardFromDeck();
            }

            if (revealedCard != null)
            {
                // He trashess that card and 
                otherPlayer.MoveRevealedCardToTrash(revealedCard, gameState);
                // may gain a card costing at most 2 less than it.
                otherPlayer.RequestPlayerGainCardFromSupply(
                    gameState,
                    acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= revealedCard.CurrentCoinCost(currentPlayer) - 2,
                    "Must gain a card costing at most 2 less than the trashed card",
                    isOptional: true);
            }

            // he discards the other revealed cards
            otherPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Scout :
        Card
    {
        public Scout()
            : base("Scout", coinCost: 4, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);
            currentPlayer.MoveRevealedCardsToHand(card => card.isVictory);
            while (currentPlayer.cardsBeingRevealed.Any)
            {
                currentPlayer.RequestPlayerTopDeckCardFromRevealed(gameState, isOptional: false);
            }
        }
    }

    public class SecretChamber :
       Card
    {
        public SecretChamber()
            : base("Secret Chamber", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (!currentPlayer.hand.IsEmpty)
            {
                if (!currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: true))
                {
                    break;
                }
                currentPlayer.AddCoins(1);
            }
        }

        public override bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.Is<SecretChamber>(), gameState);
            if (revealedCard != null)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2);
                for (int i = 0; i < 2; ++i)
                {
                    currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, false);
                }
            }

            currentPlayer.MoveRevealedCardToHand(revealedCard);

            return false;
        }
    }

    public class ShantyTown :
       Card
    {
        public ShantyTown()
            : base("Shanty Town", coinCost: 3, plusActions: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();
            if (!currentPlayer.hand.Where(card => card.isAction).Any())
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2);
            }
        }
    }

    public class Steward :
       Card
    {
        public Steward()
            : base("Steward", coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.PlusCard ||
                                    acceptableChoice == PlayerActionChoice.PlusCoin ||
                                    acceptableChoice == PlayerActionChoice.Trash);

            switch (actionChoice)
            {
                case PlayerActionChoice.PlusCard: currentPlayer.DrawAdditionalCardsIntoHand(2); break;
                case PlayerActionChoice.PlusCoin: currentPlayer.AddCoins(2); break;
                case PlayerActionChoice.Trash: currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 2, false); break;
                default: throw new Exception("Invalid case");
            }
        }
    }


    public class Swindler :
       Card
    {
        public Swindler()
            : base("Swindler", coinCost: 3, plusCoins: 2, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // Each other player trashes the top card of his deck ...
            Card trashedCard = otherPlayer.TrashCardFromTopOfDeck(gameState);
            if (trashedCard != null)
            {
                // ... and gains a card with the same cost that you choose                
                currentPlayer.RequestPlayerGainCardFromSupply(
                    gameState,
                    otherPlayer,
                    acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) == trashedCard.CurrentCoinCost(currentPlayer),
                    "Card costing equal trashed card");
            }
        }
    }

    public class Torturer :
      Card
    {
        public Torturer()
            : base("Torturer", coinCost: 5, plusCards: 3, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            PlayerActionChoice playerChoice = otherPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard ||
                                    acceptableChoice == PlayerActionChoice.GainCard);

            switch (playerChoice)
            {
                case PlayerActionChoice.Discard: otherPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false); break;
                case PlayerActionChoice.GainCard: otherPlayer.GainCardFromSupply(gameState, typeof(Curse), DeckPlacement.Hand); break;
                default: throw new Exception("Invalid Choice");
            }
        }
    }

    public class TradingPost :
        Card
    {
        public TradingPost()
            : base("Trading Post", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // Trash 2 cards from your hand
            if (currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 2, isOptional: false).Length == 2)
            {
                // If you do, gain a silver card; put it into your hand
                currentPlayer.GainCardFromSupply(gameState, typeof(Silver), DeckPlacement.Hand);
            }
        }
    }

    public class Tribute :
       Card
    {
        public Tribute()
            : base("Tribute", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // The player to your left reveals (...then discards) the top 2 cards of his deck.
            gameState.players.PlayerLeft.RevealCardsFromDeck(2);

            // for each differently named card
            Card firstCard = null;
            foreach (Card card in currentPlayer.cardsBeingRevealed)
            {
                if (firstCard == null)
                {
                    firstCard = card;
                    GainBenefitFromCard(firstCard, currentPlayer);
                }
                else
                {
                    Card secondCard = card;
                    if (!card.IsSameType(firstCard))
                    {
                        GainBenefitFromCard(secondCard, currentPlayer);
                    }
                }
            }

            // ... then discards
            gameState.players.PlayerLeft.MoveRevealedCardsToDiscard();
        }

        private void GainBenefitFromCard(Card card, PlayerState currentPlayer)
        {
            if (card.isAction)
            {
                currentPlayer.AddActions(2);
            }

            if (card.isTreasure)
            {
                currentPlayer.AddCoins(2);
            }

            if (card.isVictory)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2);
            }
        }
    }

    public class Upgrade :
        Card
    {
        public Upgrade()
            : base("Upgrade", coinCost: 5, plusCards: 1, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                gameState,
                acceptableCardsToTrash => true,
                CostConstraint.Exactly,
                1,
                CardRelativeCost.RelativeCost,
                isOptionalToTrash: false,
                isOptionalToGain: false);
        }
    }

    public class WishingWell :
        Card
    {
        public WishingWell()
            : base("Wishing Well", coinCost: 3, plusCards: 1, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type cardType = currentPlayer.GuessCardTopOfDeck(gameState);

            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
            if (revealedCard.Is(cardType))
            {
                currentPlayer.MoveRevealedCardsToHand(acceptableCard => true);
            }
            else
            {
                currentPlayer.MoveRevealedCardsToDiscard();
            }
        }
    }

}