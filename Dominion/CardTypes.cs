using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Curse : Card { public Curse() : base("Curse", coinCost: 0, isCurse: true) { } }
    public class Estate : Card { public Estate() : base("Estate", coinCost: 2, victoryPoints: playerState => 1) { } }
    public class Duchy : Card { public Duchy() : base("Duchy", coinCost: 5, victoryPoints: playerState => 3) { } }
    public class Province : Card { public Province() : base("Province", coinCost: 8, victoryPoints: playerState => 6) { } }
    public class Colony : Card { public Colony() : base("Colony", coinCost: 11, victoryPoints: playerState => 10) { } }
    public class Copper : Card { public Copper() : base("Copper", coinCost: 0, plusCoins: 1, isTreasure: true) { } }
    public class Silver : Card { public Silver() : base("Silver", coinCost: 3, plusCoins: 2, isTreasure: true) { } }
    public class Gold : Card { public Gold() : base("Gold", coinCost: 6, plusCoins: 3, isTreasure: true) { } }
    public class Platinum : Card { public Platinum() : base("Platinum", coinCost: 9, plusCoins: 5, isTreasure: true) { } }

    // Base Cards

    public class Festival : Card { public Festival() : base("Festival", coinCost: 5, plusActions: 2, plusBuy: 1, plusCoins: 2, isAction: true) { } }
    public class Laboratory : Card { public Laboratory() : base("Laboratory", coinCost: 5, plusCards: 2, plusActions: 1, isAction: true) { } }
    public class Market : Card { public Market() : base("Market", coinCost: 5, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 1, isAction: true) { } }
    public class Smithy : Card { public Smithy() : base("Smithy", coinCost: 4, plusCards: 3, isAction: true) { } }
    public class Village : Card { public Village() : base("Village", coinCost: 3, plusCards: 1, plusActions: 2, isAction: true) { } }
    public class WoodCutter : Card { public WoodCutter() : base("WoodCutter", coinCost: 3, plusBuy: 1, plusCoins: 2, isAction: true) { } }

    // Intrigue

    public class GreatHall : Card { public GreatHall() : base("Great Hall", coinCost: 3, victoryPoints: playerState => 1, plusCards: 1, plusActions: 1, isAction: true) { } }
    public class Harem : Card { public Harem() : base("Harem", coinCost: 6, victoryPoints: playerState => 2, plusCoins: 2, isTreasure: true) { } }

    // Alchemy
    public class Potion : Card { public Potion() : base("Potion", coinCost: 4) { } }

    public class Sample
        : Card
    {
        public Sample()
            : base("Sample", coinCost: -1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {

        }
    }

    // Base Cards

    public class Adventurer
        : Card
    {
        public Adventurer()
            : base("Adventurer", coinCost: 6, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countTreasureFound = 0;
            while (countTreasureFound < 2)
            {
                Card card = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (card == null)
                {
                    break;
                }

                if (card.isTreasure)
                {
                    countTreasureFound += 1;
                    currentPlayer.MoveRevealedCardToHand(card);
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Bureaucrat :
       Card
    {
        public Bureaucrat()
            : base("Bureaucrat", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            gameState.PlayerGainCardFromSupply<Silver>(currentPlayer);

            CardPredicate acceptableCard = card => card.isVictory;

            foreach (PlayerState player in gameState.players.OtherPlayers)
            {
                Card cardTopDecked = player.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard, isOptional: false);
                if (cardTopDecked == null)
                {
                    player.RevealHand();
                }
                else
                {                    
                    player.RevealCard(cardTopDecked, DeckPlacement.TopOfDeck);
                }
            }
        }
    }

    public class Cellar
        : Card
    {
        public Cellar()
            : base("Cellar", coinCost: 2, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countCardsDiscarded = 0;
            while (!currentPlayer.hand.IsEmpty)
            {
                if (!currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: true))
                {
                    break;
                }
                ++countCardsDiscarded;
            }

            currentPlayer.DrawAdditionalCardsIntoHand(countCardsDiscarded);
        }
    }

    public class Chancellor
        : Card
    {
        public Chancellor()
            : base("Chancellor", coinCost: 3, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldPutDeckInDiscard(gameState))
            {
                currentPlayer.MoveDeckToDiscard();
            }
        }
    }

    public class Chapel
      : Card
    {
        public Chapel()
            : base("Chapel", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 4, isOptional: true);
        }
    }

    public class CouncilRoom
       : Card
    {
        public CouncilRoom()
            : base("CouncilRoom", coinCost: 5, plusCards: 4, plusBuy: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.OtherPlayers)
            {
                player.turnCounters.availableBuys += 1;
            }
        }
    }

    public class Feast
       : Card
    {
        public Feast()
            : base("Feast", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) < 5
                , "cost of card < 5");
        }
    }

    public class Gardens
        : Card
    {
        public Gardens()
            : base("Gardens", coinCost: 4)
        {
            this.victoryPointCounter = delegate(PlayerState playerState)
            {
                return playerState.AllOwnedCards.Where(card => card.isVictory).Count() / 4;
            };
        }
    }

    public class Library
        : Card
    {
        public Library()
            : base("Library", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (currentPlayer.hand.Count < 7)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (revealedCard == null)
                {
                    break;
                }

                bool putCardInHand = revealedCard.isAction && currentPlayer.actions.ShouldPutCardInHand(gameState, revealedCard);

                if (putCardInHand)
                {
                    currentPlayer.MoveRevealedCardToHand(revealedCard);
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Militia
        : Card
    {
        public Militia()
            : base("Militia", coinCost: 4, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);                
            }
        }
    }

    public class Mine
        : Card
    {
        public Mine()
            : base("Mine", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                        gameState,
                        card => card.isTreasure,
                        CostConstraint.UpTo,
                        3,
                        CardRelativeCost.RelativeCost,
                        isOptionalToTrash: false,
                        isOptionalToGain: false,
                        defaultLocation: DeckPlacement.Hand);
        }
    }

    public class Moat
        : Card
    {
        public Moat()
            : base("Moat", coinCost: 2, plusCards: 2, isAction: true)
        {
        }

        public override bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.Is<CardTypes.Moat>(), gameState);
            currentPlayer.MoveRevealedCardToHand(revealedCard);
            return revealedCard != null;
        }
    }

    public class Moneylender
       : Card
    {
        public Moneylender()
            : base("Moneylender", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.TrashCardFromHandOfType(gameState, typeof(Copper), guaranteeInHand: false);
            if (card != null)
            {
                currentPlayer.AddCoins(3);
            }
        }
    }

    public class Remodel
       : Card
    {
        public Remodel()
            : base("Remodel", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                        gameState,
                        card => true,
                        CostConstraint.UpTo,
                        2,
                        CardRelativeCost.RelativeCost);
        }
    }

    public class Spy
      : Card
    {
        public Spy()
            : base("Spy", coinCost: 4, plusCards: 1, plusActions: 1, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            ApplySpyEffect(currentPlayer, currentPlayer, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            ApplySpyEffect(otherPlayer, currentPlayer, gameState);
        }

        private static void ApplySpyEffect(PlayerState playerAffected, PlayerState decidingPlayer, GameState gameState)
        {
            Card revealedCard = playerAffected.DrawAndRevealOneCardFromDeck();
            if (revealedCard != null)
            {
                if (decidingPlayer.actions.ShouldPlayerDiscardCardFromDeck(gameState, playerAffected, revealedCard))
                {
                    playerAffected.MoveRevealedCardsToDiscard();
                }
                else
                {
                    playerAffected.MoveRevealedCardToTopOfDeck(revealedCard);
                }
            }
        }
    }

    public class Thief
      : Card
    {
        public Thief()
            : base("Thief", coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(2);

            Card cardtoTrash = null;
            CardPredicate acceptableCards = card => card.isTreasure;
            if (otherPlayer.cardsBeingRevealed.HasCard(acceptableCards))
            {
                Type cardTypeToTrash = currentPlayer.actions.GetCardFromRevealedCardsToTrash(otherPlayer, otherPlayer.cardsBeingRevealed, acceptableCards);

                cardtoTrash = otherPlayer.cardsBeingRevealed.RemoveCard(cardTypeToTrash);
                if (cardtoTrash == null)
                {
                    throw new Exception("Must choose a revealed card to trash");
                }

                if (!acceptableCards(cardtoTrash))
                {
                    throw new Exception("Player Must choose a treasure card to trash");
                }

                otherPlayer.MoveCardToTrash(cardtoTrash, gameState);
            }

            if (cardtoTrash != null)
            {
                if (currentPlayer.actions.ShouldGainCard(gameState, cardtoTrash))
                {
                    Card cardToGain = gameState.trash.RemoveCard(cardtoTrash.GetType());
                    currentPlayer.GainCard(gameState, cardToGain, DeckPlacement.Discard);
                }
            }

            otherPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class ThroneRoom
        : Card
    {
        public ThroneRoom()
            : base("Throne Room", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type actionTypeToPlay = currentPlayer.actions.GetActionFromHandToPlay(gameState, false);

            Card cardToPlay = currentPlayer.RequestPlayerChooseActionToRemoveFromHandForPlay(gameState, isOptional: false);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 2);
            }
        }
    }

    public class Witch
        : Card
    {
        public Witch()
            : base("Witch", coinCost: 5, plusCards: 2, isAction: true, isAttack: true)
        {

        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            gameState.PlayerGainCardFromSupply<Curse>(otherPlayer);
        }
    }

    public class Workshop
        : Card
    {
        public Workshop()
            : base("Workshop", coinCost: 3, isAction: true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4,
                "Card must cost up to 4");
        }
    }

    // Intrigue

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

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.turnCounters.cardCoinDiscount += 1;
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
                currentPlayer.turnCounters.availableActionCount += 1;
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
            : base("IronWorks", coinCost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, acceptableCard => true, "Any card");

            if (gainedCard.isAction)
            {
                currentPlayer.turnCounters.availableActionCount += 1;
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
                    currentPlayer.AddCoins(2);
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
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseAction(
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
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseAction(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.PlusCard || acceptableChoice == PlayerActionChoice.PlusAction);

            if (actionChoice == PlayerActionChoice.PlusCard)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(3);
            }
            else
            {
                currentPlayer.turnCounters.availableActionCount += 2;
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

            PlayerActionChoice firstChoice = currentPlayer.RequestPlayerChooseAction(
                gameState,
                acceptableFirstChoice);

            PlayerActionChoice secondChoice = currentPlayer.RequestPlayerChooseAction(
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
                case PlayerActionChoice.PlusAction: currentPlayer.turnCounters.availableActionCount += 1; break;
                case PlayerActionChoice.PlusBuy: currentPlayer.turnCounters.availableBuys += 1; break;
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
            : base("Scout", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);
            currentPlayer.MoveRevealedCardsToHand(card => card.isVictory);
            while (currentPlayer.cardsBeingRevealed.Any)
            {
                currentPlayer.RequestPlayerTopDeckCardFromRevealed(gameState);
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
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseAction(
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
            PlayerActionChoice playerChoice = otherPlayer.RequestPlayerChooseAction(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard ||
                                    acceptableChoice == PlayerActionChoice.GainCard);

            switch (playerChoice)
            {
                case PlayerActionChoice.Discard: otherPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2); break;
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
                currentPlayer.turnCounters.availableActionCount += 2;
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
            Type cardType = currentPlayer.actions.GuessCardTopOfDeck(gameState);

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

    // Promo

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
            //TODO
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
            Type cardType = gameState.players.PlayerLeft.actions.BanCardForCurrentPlayerRevealedCards(gameState);
            if (!currentPlayer.cardsBeingRevealed.HasCard(card => card.Is(cardType)))
            {
                throw new Exception("Must ban a card currently being revealed");
            }
            currentPlayer.MoveRevealedCardToTopOfDeck(cardType);
            currentPlayer.MoveRevealedCardsToHand(acceptableCard => true);
        }
    }

    public class Governor :
        Card
    {
        public Governor()
            : base("Governor", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice playerChoice = currentPlayer.RequestPlayerChooseAction(gameState, acceptableChoice =>
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
                        currentPlayer.GainCardFromSupply(gameState, typeof(Gold));
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.GainCardFromSupply(gameState, typeof(Silver));
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
        }

        public override void DoSpecializedCleanup(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    // Prosperity

    public class Bank :
       Card
    {
        public Bank()
            : base("Bank", coinCost: 7, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int bankValue = currentPlayer.cardsBeingPlayed.Where(card => card.isTreasure).Count(); // +1 because bank is already in the played set
            currentPlayer.AddCoins(bankValue);
        }
    }

    public class Bishop :
       Card
    {
        public Bishop()
            : base("Bishop", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.victoryTokenCount += 1;
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: false);
            if (trashedCard != null)
            {
                currentPlayer.victoryTokenCount += trashedCard.CurrentCoinCost(currentPlayer) / 2;
            }

            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: true);
            }
        }
    }

    public class City :
       Card
    {
        public City()
            : base("City", coinCost: 5, plusActions: 2, plusCards: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countEmpty = gameState.supplyPiles.Where(pile => pile.IsEmpty).Count();

            if (countEmpty >= 1)
            {
                currentPlayer.DrawOneCardIntoHand();

                if (countEmpty >= 2)
                {
                    currentPlayer.AddCoins(1);
                    currentPlayer.turnCounters.availableBuys += 1;
                }
            }
        }
    }

    public class Contraband :
        Card
    {
        public Contraband()
            : base("Contraband", coinCost: 5, plusCoins: 3, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type cardTypeToBan = gameState.players.PlayerLeft.actions.BanCardForCurrentPlayerPurchase(gameState);

            currentPlayer.turnCounters.cardsBannedFromPurchase.Add(cardTypeToBan);
        }
    }

    public class CountingHouse :
        Card
    {
        public CountingHouse()
            : base("CountingHouse", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int maxCoppers = currentPlayer.discard.CountCards(card => card.Is<CardTypes.Copper>());
            int cardsToReveal = currentPlayer.actions.GetNumberOfCardsFromDiscardToPutInHand(gameState, maxCoppers);

            if (cardsToReveal < 0 || cardsToReveal > maxCoppers)
            {
                throw new Exception("Requested number of cards to reveal is out of range");
            }

            if (cardsToReveal > 0)
            {
                currentPlayer.RevealCardsFromDiscard(cardsToReveal, card => card.Is<CardTypes.Copper>());                
                currentPlayer.MoveRevealedCardsToHand(card => true);
            }
        }
    }

    public class Expand :
        Card
    {
        public Expand()
            : base("Expand", coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToTrash = currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(gameState,
                card => true,
                CostConstraint.UpTo,
                3,
                CardRelativeCost.RelativeCost);
        }
    }

    public class Forge :
        Card
    {
        public Forge()
            : base("Forge", coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card[] trashedCards = currentPlayer.RequestPlayerTrashCardsFromHand(gameState, -1, isOptional: true);

            int totalCost = trashedCards.Select(card => card.CurrentCoinCost(currentPlayer)).Sum();            
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) == totalCost, "Must gain a card costing exactly equal to the total cost of the trashed cards>", isOptional: false);
        }
    }

    public class Goons :
        Card
    {
        public Goons()
            : base("Goons", coinCost:6, isAction: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
            }
        }

        public override void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            currentPlayer.victoryTokenCount += 1;
        }
    }

    public class GrandMarket : 
        Card
    {
         public GrandMarket()
            : base("Grand Market", coinCost:6, isAction: true, plusCards:1, plusActions:1, plusBuy:1, plusCoins:2)
        {
        }

         public override bool IsRestrictedFromBuy(PlayerState currentPlayer, GameState gameState)
         {
             return currentPlayer.cardsInPlay.HasCard<CardTypes.Copper>();
         }
    }

    public class Hoard :
       Card
    {
        public Hoard()
            : base("Hoard", coinCost: 6, isTreasure: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
 	        if (boughtCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Gold));
            }
        }        
    }

    public class KingsCourt
        : Card
    {
        public KingsCourt()
            : base("Kings Court", coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type actionTypeToPlay = currentPlayer.actions.GetActionFromHandToPlay(gameState, false);

            Card cardToPlay = currentPlayer.RequestPlayerChooseActionToRemoveFromHandForPlay(gameState, isOptional: true);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 3);
            }
        }
    }

    public class Loan
        : Card
    {
        public Loan()
            : base("Loan", coinCost:3, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while(true)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (revealedCard.isTreasure)
                {
                    if (currentPlayer.actions.ShouldTrashCard(gameState, revealedCard))
                    {
                        currentPlayer.MoveRevealedCardToTrash(revealedCard, gameState);
                    }
                    else
                    {
                        currentPlayer.MoveRevealedCardToDiscard(revealedCard, gameState);
                    }
                    break;
                }                
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Mint
        : Card
    {
        public Mint()
            : base("Mint", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.isTreasure, gameState);

            if (revealedCard != null)
            {
                currentPlayer.GainCardFromSupply(gameState, revealedCard.GetType());
            }

            currentPlayer.MoveRevealedCardToHand(revealedCard);
        }
    }

    // Hinterlands

    public class CrossRoads :
        Card
    {
        public CrossRoads()
            : base("CrossRoads", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (!currentPlayer.cardsInPlay.HasCard<CrossRoads>())
            {
                currentPlayer.turnCounters.availableActionCount += 3;
            }

            int countVictoryCards = currentPlayer.hand.Count(card => card.isVictory);

            currentPlayer.DrawAdditionalCardsIntoHand(countVictoryCards);
        }
    }

    // Seaside

    public class Warehouse :
        Card
    {
        public Warehouse()
            : base("Warehouse", coinCost: 3, isAction: true, plusActions:1, plusCards:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 3);
        }       
    }


    // Dark Ages    

    public class Ruin :
        Card
    {
        public Ruin()
            : base("Ruin", coinCost: 0, isAction: true, isRuin: true)
        {
        }
    }

    public class AbandonedMine :
        Card
    {
        public AbandonedMine()
            : base("Abandoned Mine", coinCost: 0, isAction: true, plusCoins: 1, isRuin: true)
        {
        }
    }

    public class RuinedLibrary :
        Card
    {
        public RuinedLibrary()
            : base("Ruined Library", coinCost: 0, isAction: true, plusCards:1, isRuin: true)
        {
        }
    }

    public class RuinedMarket :
        Card
    {
        public RuinedMarket()
            : base("Ruined Market", coinCost: 0, isAction: true, plusBuy: 1, isRuin: true)
        {
        }
    }

    public class RuinedVillage :
        Card
    {
        public RuinedVillage()
            : base("Ruined Village", coinCost: 0, isAction: true, plusActions: 1, isRuin: true)
        {
        }
    }

    public class Survivors :
        Card
    {
        public Survivors()
            : base("Survivors", coinCost: 0, isAction: true, isRuin: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(2);
            // TODO: Require option to put ruins back.
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class DeathCart :
        Card
    {
        public DeathCart()
            : base("Death Cart", coinCost: 4, isAction: true, plusCoins:5, requiresRuins:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToTrash = currentPlayer.RequestPlayerTrashCardFromHand(gameState,
                card => card.isAction,
                true);
            if (cardToTrash == null)
            {
                currentPlayer.MoveCardFromPlayToTrash(gameState);
            }
        }

        public override void DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruin));
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruin));
        }
    }

    public class Develop :
        Card
    {
        public Develop()
            : base("Develop", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, false);

            int trashedCardCost = trashedCard.CurrentCoinCost(currentPlayer);

            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) == (trashedCardCost - 1), "Must gain a card costing one less than the trashed card.", isOptional: false, defaultLocation: DeckPlacement.TopOfDeck);
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) == (trashedCardCost + 1), "Must gain a card costing exactly one more than the trashed card.", isOptional: false, defaultLocation: DeckPlacement.TopOfDeck);

            // TODO:  put the cards on top of your deck in either order.
        }
    }

    public class Embassy :
        Card
    {
        public Embassy()
            : base("Embassy", coinCost: 5, isAction: true, plusCards:5)
        {
        }

        public override void DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Silver));
            }
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 3);
        }
    }


    public class Feodum :
        Card
    {
        public Feodum()
            : base("Feodum", coinCost: 4, victoryPoints: CountVictoryPoints)
        {
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardsFromSupply(gameState, typeof(CardTypes.Silver), 3);
        }

        private static int CountVictoryPoints(PlayerState player)
        {
            return VictoryCountForSilver(player.AllOwnedCards.Where(card => card.Is<CardTypes.Silver>()).Count());
        }

        public static int VictoryCountForSilver(int silvercount)
        {
            return silvercount / 3;
        }
    }    
}
