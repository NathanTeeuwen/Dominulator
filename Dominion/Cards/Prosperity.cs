using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Colony 
        : Card 
    { 
        public static Colony card = new Colony();

        private Colony() 
            : base("Colony", coinCost: 11, victoryPoints: playerState => 10) 
        { 
        } 
    }

    public class Platinum 
        : Card 
    { 
        public static Platinum card = new Platinum();

        private Platinum() 
            : base("Platinum", coinCost: 9, plusCoins: 5, isTreasure: true) 
        { 
        } 
    }

    public class Monument 
        : Card 
    { 
        public static Monument card = new Monument();

        private Monument() 
            : base("Monument", coinCost: 4, isAction: true, plusCoins: 2, plusVictoryToken: 1) 
        { 
        } 
    }

    public class WorkersVillage 
        : Card 
    { 
        public static WorkersVillage card = new WorkersVillage();

        private WorkersVillage() 
            : base("Workers Village", coinCost: 4, isAction: true, plusCards: 1, plusActions: 2, plusBuy: 1) 
        { 
        } 
    }

    public class Bank
      : Card
    {
        public static Bank card = new Bank();

        private Bank()
            : base("Bank", coinCost: 7, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int bankValue = currentPlayer.cardsBeingPlayed.Where(card => card.isTreasure).Count(); // +1 because bank is already in the played set
            currentPlayer.AddCoins(bankValue);
        }
    }

    public class Bishop
      : Card
    {
        public static Bishop card = new Bishop();

        private Bishop()
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

    public class City
      : Card
    {
        public static City card = new City();

        private City()
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

    public class Contraband
       : Card
    {
        public static Contraband card = new Contraband();

        private Contraband()
            : base("Contraband", coinCost: 5, plusCoins: 3, plusBuy:1, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardTypeToBan = gameState.players.PlayerLeft.actions.BanCardForCurrentPlayerPurchase(gameState);

            currentPlayer.turnCounters.cardsBannedFromPurchase.Add(cardTypeToBan);
        }
    }

    public class CountingHouse
       : Card
    {
        public static CountingHouse card = new CountingHouse();

        private CountingHouse()
            : base("CountingHouse", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int maxCoppers = currentPlayer.discard.CountWhere(card => card == Cards.Copper);
            int cardsToReveal = currentPlayer.actions.GetNumberOfCardsFromDiscardToPutInHand(gameState, maxCoppers);

            if (cardsToReveal < 0 || cardsToReveal > maxCoppers)
            {
                throw new Exception("Requested number of cards to reveal is out of range");
            }

            if (cardsToReveal > 0)
            {
                currentPlayer.RevealCardsFromDiscard(cardsToReveal, card => card == Cards.Copper);
                currentPlayer.MoveAllRevealedCardsToHand();
            }
        }
    }

    public class Expand
       : Card
    {
        public static Expand card = new Expand();

        private Expand()
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

    public class Forge
       : Card
    {
        public static Forge card = new Forge();

        private Forge()
            : base("Forge", coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO: trashing potion card is included in total cost
            // throw NotImplemented()
            Card[] trashedCards = currentPlayer.RequestPlayerTrashCardsFromHand(gameState, currentPlayer.Hand.Count, isOptional: true);

            int totalCost = trashedCards.Select(card => card.CurrentCoinCost(currentPlayer)).Sum();
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState, 
                card => card.CurrentCoinCost(currentPlayer) == totalCost, 
                "Must gain a card costing exactly equal to the total cost of the trashed cards>", 
                isOptional: false);
        }
    }

    public class Goons
       : Card
    {
        public static Goons card = new Goons();

        private Goons()
            : base("Goons", coinCost: 6, isAction: true, isAttack: true)
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            currentPlayer.victoryTokenCount += 1;
        }
    }

    public class GrandMarket
       : Card
    {
        public static GrandMarket card = new GrandMarket();

        private GrandMarket()
            : base("Grand Market", coinCost: 6, isAction: true, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 2)
        {
        }

        public override bool IsRestrictedFromBuy(PlayerState currentPlayer, GameState gameState)
        {
            return currentPlayer.CardsInPlay.Where(card => card == Copper.card).Any();
        }
    }

    public class Hoard
      : Card
    {
        public static Hoard card = new Hoard();

        private Hoard()
            : base("Hoard", coinCost: 6, isTreasure: true, plusCoins: 2)
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            if (boughtCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(gameState, Cards.Gold);
            }
        }
    }

    public class KingsCourt
        : Card
    {
        public static KingsCourt card = new KingsCourt();

        private KingsCourt()
            : base("Kings Court", coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToPlay = currentPlayer.RequestPlayerChooseCardToRemoveFromHandForPlay(gameState, Delegates.IsActionCardPredicate, isTreasure: false, isAction: true, isOptional: true);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 3);
            }
        }
    }

    public class Loan
        : Card
    {
        public static Loan card = new Loan();

        private Loan()
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

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Mint
        : Card
    {
        public static Mint card = new Mint();

        private Mint()
            : base("Mint", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.isTreasure, gameState);

            if (revealedCard != null)
            {
                currentPlayer.GainCardFromSupply(gameState, revealedCard);
            }

            currentPlayer.MoveRevealedCardToHand(revealedCard);
        }
    }

    public class Mountebank
        : Card
    {
        public static Mountebank card = new Mountebank();

        private Mountebank()
            : base("Mountebank", coinCost: 5, isAction: true, isAttack: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (!otherPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.isCurse, true))
            {
                otherPlayer.GainCardFromSupply(Cards.Curse, gameState);
                otherPlayer.GainCardFromSupply(Cards.Copper, gameState);
            }
        }
    }

    public class Peddler
        : Card
    {
        public static Peddler card = new Peddler();

        private Peddler()
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
        public static Quarry card = new Quarry();

        private Quarry()
            : base("Quarry", coinCost: 4, isTreasure: true, plusCoins: 1)
        {
            this.provideDiscountForWhileInPlay = ProvideDiscountForWhileInPlay;
        }

        private new int ProvideDiscountForWhileInPlay(Card card)
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
        public static Rabble card = new Rabble();

        private Rabble()
            : base("Rabble", coinCost: 5, isAction: true, plusCards: 3, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(3);
            otherPlayer.MoveRevealedCardsToDiscard(card => card.isAction || card.isTreasure, gameState);
            otherPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class RoyalSeal
        : Card
    {
        public static RoyalSeal card = new RoyalSeal();

        private RoyalSeal()
            : base("Royal Seal", coinCost: 5, isTreasure: true, plusCoins: 2)
        {
            this.doSpecializedActionOnGainWhileInPlay = DoSpecializedActionOnGainWhileInPlay;
        }

        private new DeckPlacement DoSpecializedActionOnGainWhileInPlay(PlayerState currentPlayer, GameState gameState, Card gainedCard)
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
        public static Talisman card = new Talisman();

        private Talisman()
            : base("Talisman", coinCost: 4, isTreasure: true, plusCoins: 1)
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            if (boughtCard.CurrentCoinCost(currentPlayer) <= 4 && boughtCard.potionCost == 0 && !boughtCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(gameState, boughtCard);
            }
        }
    }

    public class TradeRoute
        : Card
    {
        public static TradeRoute card = new TradeRoute();

        private TradeRoute()
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
        public static Vault card = new Vault();

        private Vault()
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
        public static Venture card = new Venture();

        private Venture()
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

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Watchtower
        : Card
    {
        public static Watchtower card = new Watchtower();

        private Watchtower()
            : base("Watchtower", coinCost: 3, isReaction:true, isAction:true)
        {
            this.doSpecializedActionOnGainWhileInHand = DoSpecializedActionOnGainWhileInHand;
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawUntilCountInHand(6);
        }

        public new DeckPlacement DoSpecializedActionOnGainWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (currentPlayer.actions.ShouldRevealCardFromHand(gameState, this))
            {                
                return currentPlayer.RequestPlayerChooseTrashOrTopDeck(gameState, gainedCard);
            }
            else
            {
                return DeckPlacement.Default;
            }            
        }             
    }

}