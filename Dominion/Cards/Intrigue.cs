using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class GreatHall 
        : Card 
    { 
        public static GreatHall card = new GreatHall();

        private GreatHall() : base("Great Hall", coinCost: 3, victoryPoints: playerState => 1, plusCards: 1, plusActions: 1, isAction: true) 
        { 
        } 
    }

    public class Harem 
        : Card 
    { 
        public static Harem card = new Harem();

        private Harem() : base("Harem", coinCost: 6, victoryPoints: playerState => 2, plusCoins: 2, isTreasure: true) 
        { 
        } 
    }

    public class Baron
       : Card
    {
        public static Baron card = new Baron();

        private Baron()
            : base("Baron", coinCost: 4, plusBuy: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => acceptableCard == Estate.card, isOptional: true))
            {
                currentPlayer.AddCoins(4);
            }
            else
            {
                currentPlayer.GainCardFromSupply(Estate.card, gameState);
            }
        }
    }

    public class Bridge
       : Card
    {
        public static Bridge card = new Bridge();

        private Bridge()
            : base("Bridge", coinCost: 4, plusBuy: 1, plusCoins: 1, isAction: true)
        {
            this.provideDiscountForWhileInPlay = ProvideDiscountForWhileInPlay;
        }

        private new int ProvideDiscountForWhileInPlay(Card card)
        {
            return 1;
        }
    }

    public class Conspirator
       : Card
    {        
        public static Conspirator card = new Conspirator();

        private Conspirator()
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

    public class Coppersmith
       : Card
    {
        public static Coppersmith card = new Coppersmith();

        private Coppersmith()
            : base("Coppersmith", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.turnCounters.copperAdditionalValue += 1;
        }
    }

    public class Courtyard
       : Card
    {
        public static Courtyard card = new Courtyard();

        private Courtyard()
            : base("Courtyard", coinCost: 2, plusCards: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false);
        }
    }

    public class Duke
       : Card
    {
        public static Duke card = new Duke();

        private Duke()
            : base("Duke", coinCost: 5, isAction: true)
        {
            this.victoryPointCounter = player => player.AllOwnedCards.Where(card => card == Duchy.card).Count();
        }
    }

    public class IronWorks
       : Card
    {
        public static IronWorks card = new IronWorks();

        private IronWorks()
            : base("IronWorks", coinCost: 4, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, 
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4 && acceptableCard.potionCost == 0, 
                "Any card costing up to 4");

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

    public class Masquerade
       : Card
    {
        public static Masquerade card = new Masquerade();

        private Masquerade()
            : base("Masquerade", coinCost: 3, plusCards: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                player.cardToPass.Set(player.RequestPlayerGiveCardToPassLeft(gameState));
            }

            for (int playerIndex = 0; playerIndex < gameState.players.PlayerCount; ++playerIndex)
            {
                Card card = gameState.players[playerIndex].cardToPass.Card;
                if (card != null)
                {
                    gameState.players[playerIndex + 1].hand.AddCard(card);
                }
            }

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                player.cardToPass.Clear();
            }

            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: true);
        }
    }

    public class MiningVillage
       : Card
    {
        public static MiningVillage card = new MiningVillage();

        private MiningVillage()
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

    public class Minion
       : Card
    {
        public static Minion card = new Minion();

        private Minion()
            : base("Minion", coinCost: 5, plusActions: 1, isAttack: true, attackDependsOnPlayerChoice: true, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // Choose one: ...
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard || acceptableChoice == PlayerActionChoice.PlusCoin);

            PlayerState.AttackAction attackAction = this.DoEmptyAttack;

            if (actionChoice == PlayerActionChoice.PlusCoin)
            {
                // +2 coin;
                currentPlayer.AddCoins(2);                
            }
            else
            {
                // discard your hand, 
                currentPlayer.DiscardHand(gameState);
                // +4 cards
                currentPlayer.DrawAdditionalCardsIntoHand(4);

                attackAction = this.DoSpecializedAttackInternal;
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);
        }

        private void DoSpecializedAttackInternal(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // with at least 5 cards in hand
            if (otherPlayer.hand.Count >= 5)
            {
                // discards his hand
                otherPlayer.DiscardHand(gameState);
                // and draws 4 cards
                otherPlayer.DrawAdditionalCardsIntoHand(4);
            }
        }
    }

    public class Nobles
       : Card
    {
        public static Nobles card = new Nobles();

        private Nobles()
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

    public class Pawn
       : Card
    {
        public static Pawn card = new Pawn();

        private Pawn()
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

    public class Sabateur
       : Card
    {
        public static Sabateur card = new Sabateur();

        private Sabateur()
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
                    acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= revealedCard.CurrentCoinCost(currentPlayer) - 2 && 
                                      acceptableCard.potionCost <= revealedCard.potionCost,
                    "Must gain a card costing at most 2 less than the trashed card",
                    isOptional: true);
            }

            // he discards the other revealed cards
            otherPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Scout
       : Card
    {
        public static Scout card = new Scout();

        private Scout()
            : base("Scout", coinCost: 4, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);
            currentPlayer.MoveRevealedCardsToHand(card => card.isVictory);
            currentPlayer.RequestPlayerTopDeckRevealedCardsInAnyOrder(gameState);            
        }
    }

    public class SecretChamber
      : Card
    {
        public static SecretChamber card = new SecretChamber();

        private SecretChamber()
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

        public override bool DoReactionToAttackWhileInHand(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;

            if (currentPlayer.actions.ShouldRevealCardFromHand(gameState, this))
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2);
                for (int i = 0; i < 2; ++i)
                {
                    currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, false);
                }

                return true;
            }
            else
            {
                return false;
            }           
        }
    }

    public class ShantyTown
      : Card
    {
        public static ShantyTown card = new ShantyTown();

        private ShantyTown()
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

    public class Steward
      : Card
    {
        public static Steward card = new Steward();

        private Steward()
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


    public class Swindler
      : Card
    {
        public static Swindler card = new Swindler();

        private Swindler()
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
                    acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) == trashedCard.CurrentCoinCost(currentPlayer) && acceptableCard.potionCost == trashedCard.potionCost,
                    "Card costing equal trashed card");
            }
        }
    }

    public class Torturer
     : Card
    {
        public static Torturer card = new Torturer();

        private Torturer()
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
                case PlayerActionChoice.GainCard: otherPlayer.GainCardFromSupply(Curse.card, gameState, DeckPlacement.Hand); break;
                default: throw new Exception("Invalid Choice");
            }
        }
    }

    public class TradingPost
       : Card
    {
        public static TradingPost card = new TradingPost();

        private TradingPost()
            : base("Trading Post", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // Trash 2 cards from your hand
            if (currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 2, isOptional: false).Length == 2)
            {
                // If you do, gain a silver card; put it into your hand
                currentPlayer.GainCardFromSupply(Silver.card, gameState, DeckPlacement.Hand);
            }
        }
    }

    public class Tribute
      : Card
    {
        public static Tribute card = new Tribute();

        private Tribute()
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
                    if (card != firstCard)
                    {
                        GainBenefitFromCard(secondCard, currentPlayer);
                    }
                }
            }

            // ... then discards
            gameState.players.PlayerLeft.MoveRevealedCardsToDiscard(gameState);
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

    public class Upgrade
       : Card
    {
        public static Upgrade card = new Upgrade();

        private Upgrade()
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

    public class WishingWell
       : Card
    {
        public static WishingWell card = new WishingWell();

        private WishingWell()
            : base("Wishing Well", coinCost: 3, plusCards: 1, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardType = currentPlayer.GuessCardTopOfDeck(gameState);

            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
            if (revealedCard != cardType)
            {
                currentPlayer.MoveAllRevealedCardsToHand();
            }
            else
            {
                currentPlayer.MoveRevealedCardsToDiscard(gameState);
            }
        }
    }

}