using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Fairgrounds
       : Card
    {
        public static Fairgrounds card = new Fairgrounds();

        private Fairgrounds()
            : base("Fairgrounds", coinCost: 6, victoryPoints: playerState => playerState.AllOwnedCards.GroupBy(card => card).Count() / 5 * 2)
        {            
        }
    }

    public class FarmingVillage 
        : Card
    {
        public static FarmingVillage card = new FarmingVillage();

        private FarmingVillage()
            : base("FarmingVillage", coinCost: 4, isAction: true, plusActions:2)
        {
        }
        
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card foundCard = null;
            gameState.gameLog.PushScope();
            while (true)
            {
                foundCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (foundCard == null)
                    break;

                if (foundCard.isAction || foundCard.isTreasure)
                {
                    break;
                }
            }
            gameState.gameLog.PopScope();

            if (foundCard != null)
            {
                currentPlayer.MoveRevealedCardToHand(foundCard);
            }

            currentPlayer.MoveRevealedCardsToDiscard(cardToMove => true, gameState);
        }
    }

    public class FortuneTeller
        : Card
    {
        public static FortuneTeller card = new FortuneTeller();

        private FortuneTeller()
            : base("FortuneTeller", coinCost: 3, isAction: true, plusCoins:2, isAttack:true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // Each other player reveals cards from the top of his deck 
            Card revealedCard = otherPlayer.DrawAndRevealOneCardFromDeck();
            while (revealedCard != null)
            {
                // until he reveals a victory or curse card
                if (revealedCard.isVictory || revealedCard.isCurse)                
                {
                    otherPlayer.MoveRevealedCardToTopOfDeck(revealedCard);
                    break;
                }
                revealedCard = otherPlayer.DrawAndRevealOneCardFromDeck();
            }            

            otherPlayer.MoveRevealedCardsToDiscard(gameState);
        }        
    }

    public class Hamlet
        : Card
    {
        public static Hamlet card = new Hamlet();

        private Hamlet()
            : base("Hamlet", coinCost: 2, isAction: true, plusActions: 1, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO:  How does the player know whether he is discarding for action or buy?
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => true, isOptional: true))
            {
                currentPlayer.AddActions(1);
            }
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => true, isOptional: true))
            {
                currentPlayer.AddBuys(1);
            }
        }
    }

    public class Harvest
        : Card
    {
        public static Harvest card = new Harvest();

        private Harvest()
            : base("Harvest", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(4);
            currentPlayer.AddCoins(currentPlayer.cardsBeingRevealed.CountTypes);
            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class HornOfPlenty
        : Card
    {
        public static HornOfPlenty card = new HornOfPlenty();

        private HornOfPlenty()
            : base("HornOfPlenty", coinCost: 5, isTreasure: true)
        {
        }
  
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int cardValue = currentPlayer.CardsInPlay.CountTypes;

            Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= cardValue && acceptableCard.potionCost == 0,
                "Must gain a card costing up to 1 per differently named card.");

            if (gainedCard.isVictory)
            {
                currentPlayer.MoveCardFromPlayToTrash(gameState);
            }
        }
    }

    public class HorseTraders
        : Card
    {
        public static HorseTraders card = new HorseTraders();

        private HorseTraders()
            : base("HorseTraders", coinCost: 4, isAction: true, isReaction: true, plusBuy: 1, plusCoins: 3)
        {
        }
        
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override bool DoReactionToAttackWhileInHand(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;            
            if (currentPlayer.actions.ShouldRevealCardFromHand(gameState, this))
            {
                currentPlayer.SetAsideCardFromHandToNextTurn(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void DoSpecializedActionOnReturnToHand(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawOneCardIntoHand();
        }
    }


    public class HuntingParty
        : Card
    {
        public static HuntingParty card = new HuntingParty();

        private HuntingParty()
            : base("HuntingParty", coinCost: 5, isAction: true, plusActions: 1, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();            

            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
            while (revealedCard != null)
            {                
                if (!currentPlayer.Hand.HasCard(revealedCard))
                {
                    currentPlayer.MoveRevealedCardToHand(revealedCard);
                    break;
                }
                revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
            }

            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }        
    }

    public class Jester
        : Card
    {
        public static Jester card = new Jester();

        private Jester()
            : base("Jester", coinCost: 5, isAction: true, isAttack: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            Card discardedCard = otherPlayer.DiscardCardFromTopOfDeck();
            if (discardedCard != null)
            {
                if (discardedCard.isVictory)
                {
                    otherPlayer.GainCardFromSupply(Cards.Curse, gameState);
                }
                else if (currentPlayer.actions.ShouldGainCard(gameState, discardedCard))
                {
                    currentPlayer.GainCardFromSupply(gameState, discardedCard);
                }
                else
                {
                    otherPlayer.GainCardFromSupply(gameState, discardedCard);
                }
            }
        }
    }

    public class Menagerie
        : Card
    {
        public static Menagerie card = new Menagerie();

        private Menagerie()
            : base("Menagerie", coinCost: 3, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();            

            if (currentPlayer.hand.HasDuplicates())
            {
                currentPlayer.DrawOneCardIntoHand();
            }
            else
            {
                currentPlayer.DrawAdditionalCardsIntoHand(3);
            }
        }
    }

    public class Remake
        : Card
    {
        public static Remake card = new Remake();

        private Remake()
            : base("Remake", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            for (int i = 0; i < 2; ++i)
            {
                Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                    gameState, 
                    card => true,
                    CostConstraint.Exactly,
                    1,
                    CardRelativeCost.RelativeCost);                
            }
        }
    }

    public class Tournament
        : Card
    {
        public static Tournament card = new Tournament();

        private Tournament()
            : base("Tournament", coinCost: 4, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            bool someOtherPlayerRevealedProvince = false;

            bool didRevealProvince = currentPlayer.RequestPlayerRevealCardFromHand(card => card == Cards.Province, gameState) != null;            

            foreach (PlayerState player in gameState.players.OtherPlayers)
            {                
                someOtherPlayerRevealedProvince |= player.RequestPlayerRevealCardFromHand(card => card == Cards.Province, gameState) != null;
                player.MoveAllRevealedCardsToHand();
            }

            if (didRevealProvince)
            {
                currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                    card => card == Cards.Duchy || card.IsType(Cards.Prize),
                    "Must gain a duchy or a prize",
                    isOptional: false,
                    defaultLocation: DeckPlacement.TopOfDeck);
            }

            if (!someOtherPlayerRevealedProvince)
            {
                currentPlayer.DrawOneCardIntoHand();
                currentPlayer.AddCoins(1);
            }
        }
    }

    public class Prize
        : Card
    {
        public static Prize card = new Prize();

        private Prize()
            : this("Prize")
        {
        }

        protected Prize(
            string name,
            bool isAction = false,
            bool isTreasure = false,
            bool isAttack = false,
            int plusCoins = 0,
            int plusCards = 0,
            int plusBuy = 0,
            int plusActions = 0)
            : base(name, coinCost: 0, isAction: isAction, isTreasure: isTreasure, isAttack: isAttack, isPrize: true, plusCoins: plusCoins, plusCards: plusCards, plusBuy: plusBuy, plusActions: plusActions)
        {
        }
    }

    public class BagOfGold
        : Prize
    {
        public static BagOfGold card = new BagOfGold();

        private BagOfGold()
            : base("BagOfGold", plusActions: 1, isAction:true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Gold, gameState, DeckPlacement.TopOfDeck);
        }
    }

    public class Diadem
       : Prize
    {
        public static Diadem card = new Diadem();

        private Diadem()
            : base("Diadem", plusCoins:2, isTreasure:true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(currentPlayer.AvailableActions);
        }
    }

    public class Followers
       : Prize
    {
        public static Followers card = new Followers();

        private Followers()
            : base("Followers", isAction: true, plusCards:2, isAttack:true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Estate, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(Cards.Curse, gameState);
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
        }
    }

    public class Princess
       : Prize
    {
        public static Princess card = new Princess();

        private Princess()
            : base("Princess", isAction: true, plusBuy: 1)
        {
            this.provideDiscountForWhileInPlay = ProvideDiscountForWhileInPlay;
        }

        private new int ProvideDiscountForWhileInPlay(Card card)
        {
            return 2;
        }
    }

    public class TrustySteed
       : Prize
    {
        public static TrustySteed card = new TrustySteed();

        private TrustySteed()
            : base("TrustySteed", isAction: true)
        {            
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            IsValidChoice choices = delegate(PlayerActionChoice choice)
            {
                return choice == PlayerActionChoice.PlusCard ||
                       choice == PlayerActionChoice.PlusAction ||
                       choice == PlayerActionChoice.PlusCoin ||
                       choice == PlayerActionChoice.GainCard;
            };
            
            PlayerActionChoice choice1 = currentPlayer.RequestPlayerChooseBetween(gameState, choices);
            PlayerActionChoice choice2 = currentPlayer.RequestPlayerChooseBetween(gameState, c => choices(c) && c != choice1);

            ApplyChoice(choice1, currentPlayer, gameState);
            ApplyChoice(choice2, currentPlayer, gameState);
        }

        private static void ApplyChoice(PlayerActionChoice choice, PlayerState currentPlayer, GameState gameState)
        {
            switch (choice)
            {
                case PlayerActionChoice.PlusCard: currentPlayer.DrawAdditionalCardsIntoHand(2); break;
                case PlayerActionChoice.PlusAction: currentPlayer.AddActions(2); break;
                case PlayerActionChoice.PlusCoin: currentPlayer.AddCoins(2); break;
                case PlayerActionChoice.GainCard: currentPlayer.GainCardsFromSupply(gameState, Cards.Silver, 4); break;
                default: throw new Exception();
            }
        }
    }

    public class YoungWitch
       : Card
    {
        public static YoungWitch card = new YoungWitch();

        private YoungWitch()
            : base("Young Witch", coinCost: 4, plusCards: 2, isAction: true, isAttack: true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // TODO: BANE CARD
            // throw new NotImplementedException()
            otherPlayer.GainCardFromSupply(Curse.card, gameState);
        }
    }

}