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
            : base("Colony", Expansion.Prosperity, coinCost: 11, victoryPoints: playerState => 10) 
        { 
        } 
    }

    public class Platinum 
        : Card 
    { 
        public static Platinum card = new Platinum();

        private Platinum() 
            : base("Platinum", Expansion.Prosperity, coinCost: 9, plusCoins: 5, isTreasure: true) 
        { 
        } 
    }

    public class Monument 
        : Card 
    { 
        public static Monument card = new Monument();

        private Monument() 
            : base("Monument", Expansion.Prosperity, coinCost: 4, isAction: true, plusCoins: 2, plusVictoryToken: 1) 
        { 
        } 
    }

    public class WorkersVillage 
        : Card 
    { 
        public static WorkersVillage card = new WorkersVillage();

        private WorkersVillage() 
            : base("Worker's Village", Expansion.Prosperity, coinCost: 4, isAction: true, plusCards: 1, plusActions: 2, plusBuy: 1) 
        { 
        } 
    }

    public class Bank
      : Card
    {
        public static Bank card = new Bank();

        private Bank()
            : base("Bank", Expansion.Prosperity, coinCost: 7, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int bankValue = currentPlayer.cardsBeingPlayed.CountWhere(card => card.isTreasure); // +1 because bank is already in the played set
            currentPlayer.AddCoins(bankValue);
        }
    }

    public class Bishop
      : Card
    {
        public static Bishop card = new Bishop();

        private Bishop()
            : base("Bishop", Expansion.Prosperity, coinCost: 4, isAction: true, plusCoins:1 )
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int cardValue = 0;
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: false);
            if (trashedCard != null)
            {
                cardValue = trashedCard.CurrentCoinCost(currentPlayer) / 2;
            }            

            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: true);
            }

            currentPlayer.AddVictoryTokens(1 + cardValue);
        }
    }

    public class City
      : Card
    {
        public static City card = new City();

        private City()
            : base("City", Expansion.Prosperity, coinCost: 5, plusActions: 2, plusCards: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countEmpty = gameState.supplyPiles.Where(pile => pile.IsEmpty).Count();

            if (countEmpty >= 1)
            {
                currentPlayer.DrawOneCardIntoHand(gameState);

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
            : base("Contraband", Expansion.Prosperity, coinCost: 5, plusCoins: 3, plusBuy:1, isTreasure: true)
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
            : base("Counting House", Expansion.Prosperity, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int maxCoppers = currentPlayer.discard.CountWhere(card => card == Cards.Copper);
            int cardsToPutInHand = currentPlayer.actions.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxCoppers);

            if (cardsToPutInHand < 0 || cardsToPutInHand > maxCoppers)
            {
                throw new Exception("Requested number of cards to reveal is out of range");
            }

            if (cardsToPutInHand > 0)
            {
                currentPlayer.RevealCardsFromDiscard(cardsToPutInHand, card => card == Cards.Copper);
                currentPlayer.MoveAllRevealedCardsToHand();
            }
        }
    }

    public class Expand
       : Card
    {
        public static Expand card = new Expand();

        private Expand()
            : base("Expand", Expansion.Prosperity, coinCost: 7, isAction: true)
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
            : base("Forge", Expansion.Prosperity, coinCost: 7, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            CollectionCards trashedCards = currentPlayer.RequestPlayerTrashCardsFromHand(gameState, currentPlayer.Hand.Count, isOptional: true);

            int totalCoinCost = trashedCards.Select(card => card.CurrentCoinCost(currentPlayer)).Sum();
            int totalPotionCost = trashedCards.Select(card => card.potionCost).Sum();

            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) == totalCoinCost && card.potionCost == totalPotionCost, 
                "Must gain a card costing exactly equal to the total cost of the trashed cards>", 
                isOptional: false);
        }
    }

    public class Goons
       : Card
    {
        public static Goons card = new Goons();

        private Goons()
            : base("Goons", Expansion.Prosperity, coinCost: 6, isAction: true, isAttack: true, plusBuy:1, plusCoins:2, pluralName:"Goons")
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            currentPlayer.AddVictoryTokens(1);
        }
    }

    public class GrandMarket
       : Card
    {
        public static GrandMarket card = new GrandMarket();

        private GrandMarket()
            : base("Grand Market", Expansion.Prosperity, coinCost: 6, isAction: true, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 2)
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
            : base("Hoard", Expansion.Prosperity, coinCost: 6, isTreasure: true, plusCoins: 2)
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
            : base("King's Court", Expansion.Prosperity, coinCost: 7, isAction: true)
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
            : base("Loan", Expansion.Prosperity, coinCost: 3, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (true)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (revealedCard == null)
                    break;
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
            : base("Mint", Expansion.Prosperity, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.isTreasure, gameState);

            if (revealedCard != null)
            {
                currentPlayer.GainCardFromSupply(gameState, revealedCard);                
            }            
        }

        public override void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardsFromPlayedAreaToTrash(card => card.isTreasure, gameState);            
        } 
    }

    public class Mountebank
        : Card
    {
        public static Mountebank card = new Mountebank();

        private Mountebank()
            : base("Mountebank", Expansion.Prosperity, coinCost: 5, isAction: true, isAttack: true, plusCoins: 2)
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
            : base("Peddler", Expansion.Prosperity, coinCost: 8, isAction: true, plusActions: 1, plusCards: 1, plusCoins: 1)
        {
        }

        public override int ProvideSelfDiscount(PlayerState playerState)
        {
            if (playerState.PlayPhase == PlayPhase.Buy)
            {
                return playerState.CardsInPlay.CountWhere(card => card.isAction) * 2;
            }

            return 0;
        }
    }

    public class Quarry
        : Card
    {
        public static Quarry card = new Quarry();

        private Quarry()
            : base("Quarry", Expansion.Prosperity, coinCost: 4, isTreasure: true, plusCoins: 1)
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
            : base("Rabble", Expansion.Prosperity, coinCost: 5, isAction: true, plusCards: 3, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(3, gameState);
            otherPlayer.MoveRevealedCardsToDiscard(card => card.isAction || card.isTreasure, gameState);
            otherPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class RoyalSeal
        : Card
    {
        public static RoyalSeal card = new RoyalSeal();

        private RoyalSeal()
            : base("Royal Seal", Expansion.Prosperity, coinCost: 5, isTreasure: true, plusCoins: 2)
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
            : base("Talisman", Expansion.Prosperity, coinCost: 4, isTreasure: true, plusCoins: 1)
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
            : base("Trade Route", Expansion.Prosperity, coinCost: 3, isAction: true, plusBuy: 1)
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
            : base("Vault", Expansion.Prosperity, coinCost: 5, isAction: true, plusCards: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int cardDiscardCount = currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, int.MaxValue, isOptional: true);
            currentPlayer.AddCoins(cardDiscardCount);

            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                PlayerActionChoice choice = otherPlayer.RequestPlayerChooseBetween(gameState, 
                    isValidChoice => isValidChoice == PlayerActionChoice.Discard || 
                                     isValidChoice == PlayerActionChoice.Nothing);

                switch (choice)
                {
                    case PlayerActionChoice.Discard:
                        {
                            int cardCount = otherPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
                            if (cardCount == 2)
                            {
                                otherPlayer.DrawOneCardIntoHand(gameState);
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
            : base("Venture", Expansion.Prosperity, coinCost: 5, plusCoins: 1, isTreasure:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToPlay = null;
            while (true)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (revealedCard == null)
                {
                    break;
                }
                if (revealedCard.isTreasure)
                {
                    cardToPlay = currentPlayer.cardsBeingRevealed.RemoveCard(revealedCard);                    
                    break;
                }
            }
            currentPlayer.MoveRevealedCardsToDiscard(gameState);

            if (cardToPlay != null)
            {
                currentPlayer.DoPlayTreasure(cardToPlay, gameState);
            }

        }
    }

    public class Watchtower
        : Card
    {
        public static Watchtower card = new Watchtower();

        private Watchtower()
            : base("Watchtower", Expansion.Prosperity, coinCost: 3, isReaction:true, isAction:true)
        {
            this.doSpecializedActionOnGainWhileInHand = DoSpecializedActionOnGainWhileInHand;
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawUntilCountInHand(6, gameState);
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