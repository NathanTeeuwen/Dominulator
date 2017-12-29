using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Curse 
        : Card 
    { 
        public static Curse card = new Curse(); 
        
        private Curse()
            : base("Curse", Expansion.Base, coinCost: 0, isCurse: true, isKingdomCard: false) 
        {
        } 
    }

    public class Estate 
        : Card 
    { 
        public static Estate card = new Estate();

        private Estate()
            : base("Estate", Expansion.Base, coinCost: 2, victoryPoints: playerState => 1, isKingdomCard: false) 
        {
        } 
    }
    
    public class Duchy 
        : Card 
    { 
        public static Duchy card = new Duchy();

        private Duchy()
            : base("Duchy", Expansion.Base, pluralName: "Duchies", coinCost: 5, victoryPoints: playerState => 3, isKingdomCard: false) 
        { 
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            if (gameState.DoesGameHaveCard(Duchess.card))
            {
                currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card == Duchess.card, "may gain a duchess", isOptional: true);                
            }

            return DeckPlacement.Default;
        }
    }

    public class Province 
        : Card 
    { 
        public static Province card = new Province();

        private Province()
            : base("Province", Expansion.Base, coinCost: 8, victoryPoints: playerState => 6, isKingdomCard: false) 
        {
        } 
    }

    public class Copper 
        : Card 
    { 
        public static Copper card = new Copper();

        private Copper()
            : base("Copper", Expansion.Base, coinCost: 0, plusCoins: 1, isTreasure: true, isKingdomCard: false, defaultSupplyCount:60) 
        {
        } 
    }

    public class Silver 
        : Card 
    { 
        public static Silver card = new Silver();

        private Silver()
            : base("Silver", Expansion.Base, coinCost: 3, plusCoins: 2, isTreasure: true, isKingdomCard: false, defaultSupplyCount:40) 
        {
        } 
    }

    public class Gold 
        : Card 
    { 
        public static Gold card = new Gold();

        private Gold()
            : base("Gold", Expansion.Base, coinCost: 6, plusCoins: 3, isTreasure: true, isKingdomCard: false, defaultSupplyCount:30) 
        {
        } 
    }

    public class Festival 
        : Card 
    { 
        public static Festival card = new Festival();

        private Festival() 
            : base("Festival", Expansion.Base, coinCost: 5, plusActions: 2, plusBuy: 1, plusCoins: 2, isAction: true) 
        {
        } 
    }

    public class Laboratory 
        : Card 
    { 
        public static Laboratory card = new Laboratory();

        private Laboratory() 
            : base("Laboratory", Expansion.Base, coinCost: 5, plusCards: 2, plusActions: 1, isAction: true) 
        {
        } 
    }

    public class Market 
        : Card 
    { 
        public static Market card = new Market();

        private Market() 
            : base("Market", Expansion.Base, coinCost: 5, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 1, isAction: true) 
        {
        } 
    }

    public class Smithy 
        : Card 
    { 
        public static Smithy card = new Smithy();

        private Smithy() 
            : base("Smithy", Expansion.Base, coinCost: 4, plusCards: 3, isAction: true) 
        {
        } 
    }

    public class Village 
        : Card 
    { 
        public static Village card = new Village();

        private Village() 
            : base("Village", Expansion.Base, coinCost: 3, plusCards: 1, plusActions: 2, isAction: true) 
        {
        } 
    }

    public class WoodCutter 
        : Card 
    
    { 
        public static WoodCutter card = new WoodCutter();

        private WoodCutter() 
            : base("WoodCutter", Expansion.Base, coinCost: 3, plusBuy: 1, plusCoins: 2, isAction: true) 
        { 
        } 
    }

    public class Adventurer
        : Card
    {
        public static Adventurer card = new Adventurer();

        private Adventurer()
            : base("Adventurer", Expansion.Base, coinCost: 6, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countTreasureFound = 0;
            while (countTreasureFound < 2)
            {
                Card card = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
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

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Bureaucrat
      : Card
    {
        public static Bureaucrat card = new Bureaucrat();

        private Bureaucrat()
            : base("Bureaucrat", Expansion.Base, coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Silver.card, gameState, DeckPlacement.TopOfDeck);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            CardPredicate acceptableCard = card => card.isVictory;
            Card cardTopDecked = otherPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard, isOptional: false);
            if (cardTopDecked == null)
            {
                otherPlayer.RevealHand();
            }
            else
            {
                otherPlayer.RevealCard(cardTopDecked, DeckPlacement.TopOfDeck);
            }
        }
    }

    public class Cellar
        : Card
    {
        public static Cellar card = new Cellar();

        private Cellar()
            : base("Cellar", Expansion.Base, coinCost: 2, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.numberOfCardsToBeDrawn = 0;
            while (!currentPlayer.hand.IsEmpty)
            {
                if (!currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: true))
                {
                    break;
                }
                ++currentPlayer.numberOfCardsToBeDrawn;
            }

            currentPlayer.DrawAdditionalCardsIntoHand(currentPlayer.numberOfCardsToBeDrawn, gameState);
            currentPlayer.numberOfCardsToBeDrawn = 0;
        }
    }

    public class Chancellor
        : Card
    {
        public static Chancellor card = new Chancellor();

        private Chancellor()
            : base("Chancellor", Expansion.Base, coinCost: 3, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldPutDeckInDiscard(gameState))
            {
                currentPlayer.MoveDeckToDiscard(gameState);
            }
        }
    }

    public class Chapel
      : Card
    {
        public static Chapel card = new Chapel();

        private Chapel()
            : base("Chapel", Expansion.Base, coinCost: 2, isAction: true)
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
        public static CouncilRoom card = new CouncilRoom();

        private CouncilRoom()
            : base("Council Room", Expansion.Base, coinCost: 5, plusCards: 4, plusBuy: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.OtherPlayers)
            {
                player.DrawAdditionalCardsIntoHand(1, gameState);
            }
        }
    }

    public class Feast
       : Card
    {
        public static Feast card = new Feast();

        private Feast()
            : base("Feast", Expansion.Base, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) <= 5 && card.potionCost == 0
                , "cost of card up to 5");
        }
    }

    public class Gardens
        : Card
    {
        public static Gardens card = new Gardens();

        private Gardens()
            : base("Gardens", Expansion.Base, coinCost: 4)
        {
            this.victoryPointCounter = delegate(PlayerState playerState)
            {
                return playerState.AllOwnedCards.Count / 10;
            };
        }
    }

    public class Library
        : Card
    {
        public static Library card = new Library();

        private Library()
            : base("Library", Expansion.Base, pluralName:"Libraries", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (currentPlayer.hand.Count < 7)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (revealedCard == null)
                {
                    break;
                }

                bool putCardInHand = !revealedCard.isAction || currentPlayer.actions.ShouldPutCardInHand(gameState, revealedCard);

                if (putCardInHand)
                {
                    currentPlayer.MoveRevealedCardToHand(revealedCard);
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Militia
        : Card
    {
        public static Militia card = new Militia();

        private Militia()
            : base("Militia", Expansion.Base, coinCost: 4, plusCoins: 2, isAction: true)
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
        public static Mine card = new Mine();

        private Mine()
            : base("Mine", Expansion.Base, coinCost: 5, isAction: true)
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
        public static Moat card = new Moat();

        private Moat()
            : base("Moat", Expansion.Base, coinCost: 2, plusCards: 2, isAction: true, isReaction:true)
        {
        }

        public override bool DoReactionToAttackWhileInHand(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            if (currentPlayer.actions.ShouldRevealCardFromHand(gameState, this))
            {
                currentPlayer.RevealAndReturnCardToHand(this, gameState);                
                cancelsAttack = true;
                return true;
            }
            else
            {
                cancelsAttack = false;
                return false;
            }            
        }
    }

    public class Moneylender
       : Card
    {
        public static Moneylender card = new Moneylender();

        private Moneylender()
            : base("Moneylender", Expansion.Base, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.TrashCardFromHandOfType(Copper.card, gameState, guaranteeInHand: false);
            if (card != null)
            {
                currentPlayer.AddCoins(3);
            }
        }
    }

    public class Remodel
       : Card
    {
        public static Remodel card = new Remodel();

        private Remodel()
            : base("Remodel", Expansion.Base, coinCost: 4, isAction: true)
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
        public static Spy card = new Spy();

        private Spy()
            : base("Spy", Expansion.Base, coinCost: 4, plusCards: 1, plusActions: 1, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerInspectTopOfDeckForDiscard(currentPlayer, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerInspectTopOfDeckForDiscard(currentPlayer, gameState);            
        }        
    }

    public class Thief
      : Card
    {
        public static Thief card = new Thief();

        private Thief()
            : base("Thief", Expansion.Base, coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(2, gameState);

            Card trashedCard = currentPlayer.RequestPlayerTrashOtherPlayersRevealedCard(gameState, card => card.isTreasure, otherPlayer);

            if (trashedCard != null)
            {
                if (currentPlayer.actions.ShouldGainCard(gameState, trashedCard))
                {
                    Card cardToGain = gameState.trash.RemoveCard(trashedCard);
                    currentPlayer.GainCard(gameState, cardToGain, originalLocation:DeckPlacement.Trash, defaultPlacement:DeckPlacement.Discard);
                }
            }

            otherPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class ThroneRoom
        : Card
    {
        public static ThroneRoom card = new ThroneRoom();

        private ThroneRoom()
            : base("Throne Room", Expansion.Base, coinCost: 4, isAction: true, mightMultiplyActions:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO  Make sure throne room, Kings Court and procession stay in play when they play a duration card.
            // throw new NotImplementedException
            Card cardToPlay = currentPlayer.RequestPlayerChooseCardToRemoveFromHandForPlay(gameState, Delegates.IsActionCardPredicate, isTreasure: false, isAction: true, isOptional: false);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 2);
            }
        }
    }

    public class Witch
        : Card
    {
        public static Witch card = new Witch();

        private Witch()
            : base("Witch", Expansion.Base, coinCost: 5, plusCards: 2, isAction: true, isAttack: true)
        {

        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(Curse.card, gameState);
        }
    }

    public class Workshop
        : Card
    {
        public static Workshop card = new Workshop();

        private Workshop()
            : base("Workshop", Expansion.Base, coinCost: 3, isAction: true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4 && acceptableCard.potionCost == 0,
                "Card must cost up to 4");
        }
    }

}