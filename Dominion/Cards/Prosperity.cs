using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Colony : Card { public Colony() : base("Colony", coinCost: 11, victoryPoints: playerState => 10) { } }
    public class Platinum : Card { public Platinum() : base("Platinum", coinCost: 9, plusCoins: 5, isTreasure: true) { } }

    public class Monument : Card { public Monument() : base("Monument", coinCost: 4, isAction: true, plusCoins: 2, plusVictoryToken: 1) { } }
    public class WorkersVillage : Card { public WorkersVillage() : base("Workers Village", coinCost: 4, isAction: true, plusCards: 1, plusActions: 2, plusBuy: 1) { } }

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
                    currentPlayer.AddBuys(1);
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
            : base("Goons", coinCost: 6, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
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
            : base("Grand Market", coinCost: 6, isAction: true, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 2)
        {
        }

        public override bool IsRestrictedFromBuy(PlayerState currentPlayer, GameState gameState)
        {
            return currentPlayer.CardsInPlay.Where(card => card.Is<Copper>()).Any();
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
            : base("Loan", coinCost: 3, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (true)
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

    public class Mountebank
        : Card
    {
        public Mountebank()
            : base("Mountebank", coinCost: 5, isAction: true, isAttack: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (!otherPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.isCurse, true))
            {
                otherPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Curse));
                otherPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Copper));
            }
        }
    }

    public class Peddler
        : Card
    {
        public Peddler()
            : base("Peddler", coinCost: 8, isAction: true, plusActions: 1, plusCards: 1, plusCoins: 1)
        {
        }

        public override int ProvideSelfDiscount(PlayerState playerState)
        {
            if (playerState.playPhase == PlayPhase.Buy)
            {
                return playerState.CardsInPlay.Where(card => card.isAction).Count() * 2;
            }

            return 0;
        }
    }

    public class Quarry
        : Card
    {
        public Quarry()
            : base("Quarry", coinCost: 4, isTreasure: true, plusCoins: 1)
        {
        }

        override public int ProvideDiscountForWhileInPlay(Card card)
        {
            if (card.isAction)
            {
                return 2;
            }
            return 0;
        }
    }

    public class Rabble
        : Card
    {
        public Rabble()
            : base("Rabble", coinCost: 5, isAction: true, plusCards: 3, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(3);
            otherPlayer.MoveRevealedCardToDiscard(card => card.isAction || card.isTreasure);
            otherPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class RoyalSeal
        : Card
    {
        public RoyalSeal()
            : base("Royal Seal", coinCost: 5, isTreasure: true, plusCoins: 2)
        {
        }

        public override DeckPlacement DoSpecializedActionOnGainWhileInPlay(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (currentPlayer.actions.ShouldPutCardOnTopOfDeck(gainedCard, gameState))
            {
                return DeckPlacement.TopOfDeck;
            }

            return DeckPlacement.Default;
        }
    }

    public class Talisman
        : Card
    {
        public Talisman()
            : base("Talisman", coinCost: 4, isTreasure: true, plusCoins: 1)
        {
        }

        public override void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            if (boughtCard.CurrentCoinCost(currentPlayer) <= 4 && !boughtCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(gameState, boughtCard.GetType());
            }
        }
    }

    public class TradeRoute
        : Card
    {
        public TradeRoute()
            : base("Trade Route", coinCost: 3, isAction: true, plusBuy: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int additionalCoins = gameState.supplyPiles.Where(pile => pile.ProtoTypeCard.isVictory && gameState.HasCardEverBeenGainedFromPile(pile)).Count();
            currentPlayer.AddCoins(additionalCoins);

            currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
        }
    }

    public class Vault
        : Card
    {
        public Vault()
            : base("Vault", coinCost: 5, isAction: true, plusCards: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int cardDiscardCount = currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, int.MaxValue, isOptional: true);
            currentPlayer.AddCoins(cardDiscardCount);

            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                PlayerActionChoice choice = otherPlayer.RequestPlayerChooseBetween(gameState, isValidChoice => isValidChoice == PlayerActionChoice.Discard || isValidChoice == PlayerActionChoice.Nothing);

                switch (choice)
                {
                    case PlayerActionChoice.Discard:
                        {
                            int cardCount = otherPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
                            if (cardCount == 2)
                            {
                                otherPlayer.DrawOneCardIntoHand();
                            }
                            break;
                        }
                    case PlayerActionChoice.Nothing:
                        break;
                }
            }
        }
    }

    public class Venture
        : Card
    {
        public Venture()
            : base("Venture", coinCost: 5, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (true)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (revealedCard == null)
                {
                    break;
                }
                if (revealedCard.isTreasure)
                {
                    currentPlayer.cardsBeingRevealed.RemoveCard(revealedCard);
                    currentPlayer.DoPlayTreasure(revealedCard, gameState);
                    break;
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Watchtower
        : Card
    {
        public Watchtower()
            : base("Watchtower", coinCost: 3, isReaction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawUntilCountInHand(6);
        }

        override public DeckPlacement DoSpecializedActionOnGainWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.Equals(this), gameState);
            if (revealedCard == null)
            {
                return DeckPlacement.Default;
            }

            // how does the player know what card is being asked about?
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Trash ||
                                    acceptableChoice == PlayerActionChoice.TopDeck);

            switch (choice)
            {
                case PlayerActionChoice.Trash: return DeckPlacement.Trash;
                case PlayerActionChoice.TopDeck: return DeckPlacement.TopOfDeck;                
                default: throw new Exception("Invalid choice");
            }
        }             
    }

}