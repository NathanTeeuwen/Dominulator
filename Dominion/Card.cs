using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class Card
        : IEquatable<Card>
    {
        public readonly string name;
        private readonly int coinCost;
        public readonly int potionCost;
        public readonly int plusAction;
        public readonly int plusBuy;
        public readonly int plusCard;
        public readonly int plusCoin;
        public readonly int plusVictoryToken;
        public readonly int defaultSupplyCount;        
        public readonly bool isAction;
        public readonly bool isAttack;
        public readonly bool attackDependsOnPlayerChoice;
        public readonly bool isAttackBeforeAction;
        public readonly bool isCurse;
        public readonly bool isReaction;
        public readonly bool isRuins;
        public readonly bool isTreasure;
        public readonly bool isDuration;
        public readonly bool requiresRuins;
        public readonly bool requiresSpoils;
        public readonly bool isShelter;
        public readonly bool canOverpay;
        protected VictoryPointCounter victoryPointCounter;              // readonly
        protected GameStateMethod doSpecializedCleanupAtStartOfCleanup; // readonly
        protected CardIntValue provideDiscountForWhileInPlay;           // readonly
        protected GameStateCardMethod doSpecializedActionOnBuyWhileInPlay; // readonly
        protected GameStateCardPredicate doSpecializedActionOnTrashWhileInHand; //readonly

        internal Card(
            string name,
            int coinCost,
            int potionCost = 0,
            int plusActions = 0,
            int plusBuy = 0,
            int plusCards = 0,
            int plusCoins = 0,
            VictoryPointCounter victoryPoints = null,
            int plusVictoryToken = 0,
            int defaultSupplyCount = 10,
            bool isAction = false,
            bool isAttack = false,
            bool attackDependsOnPlayerChoice = false,
            bool isAttackBeforeAction = false,
            bool isCurse = false,
            bool isReaction = false,
            bool isRuin = false,
            bool isTreasure = false,
            bool isDuration = false,
            bool requiresRuins = false,
            bool requiresSpoils = false,
            bool isShelter = false,
            bool canOverpay = false,
            CardIntValue provideDiscountForWhileInPlay = null,
            GameStateMethod doSpecializedCleanupAtStartOfCleanup = null,
            GameStateCardMethod doSpecializedActionOnBuyWhileInPlay = null,
            GameStateCardPredicate doSpecializedActionOnTrashWhileInHand = null)
        {
            this.name = name;
            this.coinCost = coinCost;
            this.potionCost = potionCost;
            this.plusAction = plusActions;
            this.plusBuy = plusBuy;
            this.plusCard = plusCards;
            this.plusCoin = plusCoins;
            this.victoryPointCounter = victoryPoints;
            this.plusVictoryToken = plusVictoryToken;
            this.isAction = isAction;
            this.isAttack = isAttack;
            this.attackDependsOnPlayerChoice = attackDependsOnPlayerChoice;
            this.isAttackBeforeAction = isAttackBeforeAction;
            this.isCurse = isCurse;
            this.isReaction = isReaction;
            this.isRuins = isRuin;
            this.isTreasure = isTreasure;
            this.defaultSupplyCount = defaultSupplyCount;
            this.requiresRuins = requiresRuins;
            this.isDuration = isDuration;
            this.isShelter = isShelter;
            this.requiresSpoils = requiresSpoils;
            this.canOverpay = canOverpay;
            this.provideDiscountForWhileInPlay = provideDiscountForWhileInPlay;
            this.doSpecializedCleanupAtStartOfCleanup = doSpecializedCleanupAtStartOfCleanup;
            this.doSpecializedActionOnBuyWhileInPlay = doSpecializedActionOnBuyWhileInPlay;
            this.doSpecializedActionOnTrashWhileInHand = doSpecializedActionOnTrashWhileInHand;
        }

        public bool Is(Card card)
        {
            return this.Equals(card);
        }

        public bool Is<T>()
            where T : Card, new()
        {
            return this.Is(Card.Type<T>());
        }        

        public bool isVictory
        {
            get
            {
                return this.victoryPointCounter != null;
            }
        }

        public int VictoryPoints(PlayerState playerState)
        {
            return this.victoryPointCounter(playerState);
        }

        public bool Equals(Card other)
        {
            if (other == null)
                return false;
            return this.GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((Card)obj);
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        public int DefaultCoinCost
        {
            get
            {
                return this.coinCost;
            }
        }

        public int CurrentCoinCost(PlayerState player)
        {
            int effectiveCost = this.coinCost;

            if (player.ownsCardThatMightProvideDiscountWhileInPlay)
            {
                foreach (Card cardInPlay in player.CardsInPlay)
                {                    
                    effectiveCost -= cardInPlay.ProvideDiscountForWhileInPlay(this);
                }
            }

            effectiveCost -= this.ProvideSelfDiscount(player);

            return (effectiveCost >= 0) ? effectiveCost : 0;
        }

        
        virtual public void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
        }

        virtual public void DoSpecializedDiscardNonCleanup(PlayerState currentPlayer, GameState gameState)
        {

        }

        virtual public void DoSpecializedDiscardFromPlay(PlayerState currentPlayer, GameState gameState)
        {

        }

        // return true if the card chose to react in some way;
        virtual public bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;
            // by default, all cards are affected by attack
            return false;
        }

        // return true if blocks attack;
        virtual public bool DoReactionToAttackWhileInPlay(PlayerState currentPlayer, GameState gameState)
        {
            // by default, all cards are affected by attack
            return false;
        }

        virtual public void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
        }

        virtual public void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
        }

        public bool HasSpecializedCleanupAtStartOfCleanup
        {
            get
            {
                return this.doSpecializedCleanupAtStartOfCleanup != null;
            }
        }

        public void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            if (this.HasSpecializedCleanupAtStartOfCleanup)
            {
                this.doSpecializedCleanupAtStartOfCleanup(currentPlayer, gameState);
            }
        }

        virtual public DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            return DeckPlacement.Default;
        }

        virtual public void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
        }

        public bool HasSpecializedActionOnBuyWhileInPlay
        {
            get
            {
                return this.doSpecializedActionOnBuyWhileInPlay != null;
            }
       }

        public void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            if (this.HasSpecializedActionOnBuyWhileInPlay)
            {
                this.doSpecializedActionOnBuyWhileInPlay(currentPlayer, gameState, boughtCard);
            }
        }

        public bool HasSpecializedActionOnTrashWhileInHand
        {
            get
            {
                return this.doSpecializedActionOnTrashWhileInHand != null;
            }
        }

        public bool DoSpecializedActionOnTrashWhileInHand(PlayerState currentPlayer, GameState gameState, Card card)
        {
            if (this.HasSpecializedActionOnTrashWhileInHand)
            {
                return this.doSpecializedActionOnTrashWhileInHand(currentPlayer, gameState, card);
            }
            return false;
        }


        virtual public void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {

        }

        virtual public DeckPlacement DoSpecializedActionOnGainWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            return DeckPlacement.Default;
        }

        virtual public DeckPlacement DoSpecializedActionOnBuyWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            return DeckPlacement.Default;
        }

        virtual public DeckPlacement DoSpecializedActionOnGainWhileInPlay(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            return DeckPlacement.Default;
        }        

        public bool MightProvideDiscountWhileInPlay
        {
            get
            {
                return this.provideDiscountForWhileInPlay != null;
            }
        }

        public int ProvideDiscountForWhileInPlay(Card card)
        {
            return (this.MightProvideDiscountWhileInPlay) ? this.provideDiscountForWhileInPlay(card) : 0;                
        }

        virtual public int ProvideSelfDiscount(PlayerState playState)
        {
            return 0;
        }

        virtual public bool IsRestrictedFromBuy(PlayerState currentPlayer, GameState gameState)
        {
            return false;
        }

        protected void DoEmptyAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {

        }

        virtual public void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            throw new Exception("Card has overpay semantics to be implemented");
        }

        virtual public void DoSpecializedSetupIfInSupply(GameState gameState)
        {

        }        

        public static Card Type<T>()
            where T : Card, new()
        {
            return Example<T>.Card;            
        }

        private static class Example<T>            
            where T : Card, new()            
        {
            static public readonly T Card = new T();
        }
    }
}