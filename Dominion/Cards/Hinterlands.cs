using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class BorderVillage :
      Card
    {
        public static BorderVillage card; private BorderVillage()
            : base("Border Village", coinCost: 6, isAction: true, plusActions:2, plusCards:1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) < this.CurrentCoinCost(currentPlayer) && card.potionCost == 0,
                "Must gain a card costing less than this");

            return DeckPlacement.Default;
        }
    }

    public class Cache :
      Card
    {
        public static Cache card; private Cache()
            : base("Cache", coinCost: 5, isTreasure:true, plusCoins:3)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardsFromSupply<Copper>(gameState, 2);
            return DeckPlacement.Default;
        }
    }

    public class Cartographer :
        Card
    {
        public static Cartographer card; private Cartographer()
            : base("Cartographer", coinCost: 5, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);

            while (currentPlayer.cardsBeingRevealed.Any())
            {
                if (currentPlayer.RequestPlayerTopDeckCardFromRevealed(gameState, true) == null)
                {
                    break;
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class CrossRoads :
        Card
    {
        public static CrossRoads card; private CrossRoads()
            : base("CrossRoads", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (!currentPlayer.CardsInPlay.Where(card => card.Is<CrossRoads>()).Any())
            {
                currentPlayer.AddActions(3);
            }

            int countVictoryCards = currentPlayer.hand.Count(card => card.isVictory);

            currentPlayer.DrawAdditionalCardsIntoHand(countVictoryCards);
        }
    }

    public class Develop :
        Card
    {
        public static Develop card; private Develop()
            : base("Develop", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, false);

            int trashedCardCost = trashedCard.CurrentCoinCost(currentPlayer);

            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) == (trashedCardCost - 1) && card.potionCost == trashedCard.potionCost, "Must gain a card costing one less than the trashed card.", isOptional: false, defaultLocation: DeckPlacement.TopOfDeck);
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) == (trashedCardCost + 1) && card.potionCost == trashedCard.potionCost, "Must gain a card costing exactly one more than the trashed card.", isOptional: false, defaultLocation: DeckPlacement.TopOfDeck);

            // TODO:  put the cards on top of your deck in either order.
            //throw new NotImplementedException();
        }
    }

    public class Duchess :
        Card
    {
        public static Duchess card; private Duchess()
            : base("Duchess", coinCost: 2, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                Card card = player.DrawAndLookAtOneCardFromDeck();
                if (card != null)
                {
                    if (player.actions.ShouldPlayerDiscardCardFromDeck(gameState, player, card))
                    {
                        player.MoveLookedAtCardsToDiscard(gameState);
                    }
                    else
                    {
                        player.MoveLookedAtCardToTopOfDeck(card);
                    }
                }
            }
        }
    }

    public class Embassy :
        Card
    {
        public static Embassy card; private Embassy()
            : base("Embassy", coinCost: 5, isAction: true, plusCards: 5)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.GainCardFromSupply(gameState, Card.Type<CardTypes.Silver>());
            }

            return DeckPlacement.Default;
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 3, isOptional: false);
        }
    }

    public class Farmland :
        Card
    {
        public static Farmland card; private Farmland()
            : base("Farmland", coinCost: 6, victoryPoints: PlayerState => 2)
        {
        }

        public override void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                gameState, 
                acceptableCard => true,
                CostConstraint.Exactly,
                2,
                CardRelativeCost.RelativeCost);                
        }        
    }

    public class FoolsGold :
        Card
    {
        public static FoolsGold card; private FoolsGold()
            : base("FoolsGold", coinCost: 2, isTreasure:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (!currentPlayer.CardsInPlay.Where(c => c.Is<FoolsGold>()).Any())
            {
                currentPlayer.AddCoins(1);
            }
            else
            {
                currentPlayer.AddCoins(4);
            }
        }
    }

    public class Haggler :
        Card
    {
        public static Haggler card; private Haggler()
            : base("Haggler", coinCost: 5, isAction:true, plusCoins:2)
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                card => !card.isVictory && card.CurrentCoinCost(currentPlayer) < boughtCard.CurrentCoinCost(currentPlayer) && card.potionCost <= boughtCard.potionCost,
                "Must gain a non victory card costing less than the bought card");
        }
    }

    public class Highway :
        Card
    {
        public static Highway card; private Highway()
            : base("Highway", coinCost: 5, isAction: true, plusCards: 1, plusActions: 1)
        {
            this.provideDiscountForWhileInPlay = ProvideDiscountForWhileInPlay;
        }

        // TODO:  cant throne room highway, but can throne room bridge.
        private new int ProvideDiscountForWhileInPlay(Card card)
        {
            return 1;
        }
    }

    public class IllGottenGains :
        Card
    {
        public static IllGottenGains card; private IllGottenGains()
            : base("Ill-Gotten Gains", coinCost: 5, isTreasure:true, plusCoins:1)
        {            
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.OtherPlayers)
            {
                player.GainCardFromSupply<CardTypes.Curse>(gameState);
            }

            return DeckPlacement.Default;
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.Is<CardTypes.Copper>(), "you may gain a copper", isOptional:true, defaultLocation: DeckPlacement.Hand);
        }
    }

    public class Inn :
        Card
    {
        public static Inn card; private Inn()
            : base("Inn", coinCost: 5, isAction: true, plusCards: 2, plusActions: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class JackOfAllTrades :
        Card
    {
        public static JackOfAllTrades card; private JackOfAllTrades()
            : base("JackOfAllTrades", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<Silver>(gameState);
            
            // look at the top card of the deck and discard or put it back
            Card card = currentPlayer.DrawAndLookAtOneCardFromDeck();
            if (card != null)
            {
                if (currentPlayer.actions.ShouldPlayerDiscardCardFromDeck(gameState, currentPlayer, card))
                {
                    currentPlayer.gameLog.PushScope();
                    currentPlayer.MoveLookedAtCardsToDiscard(gameState);
                    currentPlayer.gameLog.PlayerDiscardCard(currentPlayer, card);
                    currentPlayer.gameLog.PopScope();
                }
                else
                {
                    currentPlayer.MoveLookedAtCardToTopOfDeck(card);
                }
            }

            currentPlayer.DrawUntilCountInHand(5);

            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => !acceptableCard.isTreasure, isOptional: true);
        }        
    }

    public class Mandarin :
        Card
    {
        public static Mandarin card; private Mandarin()
            : base("Mandarin", coinCost: 5, isAction: true, plusCoins:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false);
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Margrave :
        Card
    {
        public static Margrave card; private Margrave()
            : base("Margrave", coinCost: 5, isAction: true, plusCards:3, plusBuy:1, isAttack:true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.DrawOneCardIntoHand();
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
        }
    }

    public class NobleBrigand :
        Card
    {
        public static NobleBrigand card; private NobleBrigand()
            : base("NobleBrigand", coinCost: 4, isAction: true, plusCoins:1, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(2);

            Card cardtoTrash = null;
            CardPredicate acceptableCards = card => card.Is<Silver>() || card.Is<Gold>();
            if (otherPlayer.cardsBeingRevealed.HasCard(acceptableCards))
            {
                Card cardTypeToTrash = currentPlayer.actions.GetCardFromRevealedCardsToTrash(gameState, otherPlayer, acceptableCards);

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
            
            if (!otherPlayer.CardsBeingRevealed.Where(card => card.isTreasure).Any())
            {
                otherPlayer.GainCardFromSupply<Copper>(gameState);
            }

            if (cardtoTrash != null)
            {
                Card cardToGain = gameState.trash.RemoveCard(cardtoTrash);
                currentPlayer.GainCard(gameState, cardToGain, DeckPlacement.Trash, DeckPlacement.Discard);                
            }

            otherPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class NomadCamp :
        Card
    {
        public static NomadCamp card; private NomadCamp()
            : base("NomadCamp", coinCost: 4, isAction: true, plusCoins: 2, plusBuy:1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldPutCardOnTopOfDeck(this, gameState))
            {
                return DeckPlacement.TopOfDeck;
            }

            return DeckPlacement.Default;
        }
    }

    public class Oasis :
        Card
    {
        public static Oasis card; private Oasis()
            : base("Oasis", coinCost: 3, isAction: true, plusCoins: 1, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: false);
        }
    }

    public class Oracle :
        Card
    {
        public static Oracle card; private Oracle()
            : base("Oracle", coinCost: 3, isAction: true, plusCards: 2, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Scheme :
        Card
    {
        public static Scheme card; private Scheme()
            : base("Scheme", coinCost: 3, isAction: true, plusCards: 1, plusActions:1)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTopDeckCardsFromPlay(gameState,
                acceptableCard => acceptableCard.isAction,
                isOptional: true);
        }
    }

    public class SilkRoad :
        Card
    {
        public static SilkRoad card; private SilkRoad()
            : base("SilkRoad", coinCost: 4, victoryPoints: playerState => playerState.AllOwnedCards.Where(card => card.isVictory).Count()/4)
        {
        }
    }

    public class SpiceMerchant :
        Card
    {
        public static SpiceMerchant card; private SpiceMerchant()
            : base("SpiceMerchant", coinCost: 4, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => card.isTreasure, isOptional:true) != null)
            {
                PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(
                    gameState,
                    acceptableChoice => acceptableChoice == PlayerActionChoice.PlusCard || acceptableChoice == PlayerActionChoice.PlusCoin);

                switch (choice)
                {
                    case PlayerActionChoice.PlusCard:
                    {
                        currentPlayer.DrawAdditionalCardsIntoHand(2);
                        currentPlayer.AddActions(1);
                        break;
                    }
                    case PlayerActionChoice.PlusCoin:
                    {
                        currentPlayer.AddCoins(2);
                        currentPlayer.AddBuys(1);
                        break;
                    }
                    default:
                        throw new Exception();
                }
            }
        }
    }

    public class Stables :
        Card
    {
        public static Stables card; private Stables()
            : base("Stables", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.isTreasure, isOptional: true))
            {
                currentPlayer.DrawAdditionalCardsIntoHand(3);
                currentPlayer.AddActions(1);
            }
        }
    }

    public class Trader :
        Card
    {
        public static Trader card; private Trader()
            : base("Trader", coinCost: 4, isReaction:true, isAction: true)
        {
            this.doSpecializedActionOnGainWhileInHand = DoSpecializedActionOnGainWhileInHand;
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
            if (trashedCard != null)
            {
                currentPlayer.GainCardsFromSupply<Silver>(gameState, trashedCard.CurrentCoinCost(currentPlayer));
            }
        }

        public new DeckPlacement DoSpecializedActionOnGainWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (currentPlayer.actions.ShouldRevealCardFromHandForCard(gameState, this, gainedCard))
            {
                currentPlayer.RevealAndReturnCardToHand(this, gameState);
                currentPlayer.GainCardFromSupply<Silver>(gameState);
                return DeckPlacement.None;
            }            

            return DeckPlacement.Default;
        }        
    }

    public class Tunnel :
        Card
    {
        public static Tunnel card; private Tunnel()
            : base("Tunnel", coinCost: 3, isReaction: true, victoryPoints: playerState => 2)
        {            
        }

        public override void DoSpecializedDiscardNonCleanup(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<Gold>(gameState);
        }        
    }
}