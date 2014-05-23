using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum Expansion
    {
        Alchemy,
        Base,
        Cornucopia,
        DarkAges,
        Guilds,
        Hinterlands,        
        Intrigue,                
        Promo,
        Prosperity,
        Seaside,
        Unknown
    }

    public abstract class Card
        : IEquatable<Card>
    {
        public readonly string name;
        public readonly string pluralName;
        public readonly Expansion expansion;
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
        public readonly bool isPrize;
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
        protected GameStateCardMethod doSpecializedActionToCardWhileInPlay;  //readonly
        protected GameStateCardToPlacement doSpecializedActionOnGainWhileInPlay; //readonly
        protected GameStateCardToPlacement doSpecializedActionOnBuyWhileInHand; //readonly 
        protected GameStateCardToPlacement doSpecializedActionOnGainWhileInHand; //readonly

        private readonly int privateIndex;

        public string ProgrammaticName
        {
            get
            {
                return this.name.Replace(" ", "_");
            }
        }

        private static int lastCardIndex = 0;
        private static HashSet<Type> cardTypes = new HashSet<Type>();

        internal int Index
        {
            get
            {
                return this.privateIndex;
            }
        }        

        protected Card(
            string name,
            Expansion expansion,
            int coinCost,
            string pluralName = null,
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
            bool isPrize = false,
            bool isRuins = false,            
            bool isTreasure = false,
            bool isDuration = false,
            bool requiresRuins = false,
            bool requiresSpoils = false,
            bool isShelter = false,
            bool canOverpay = false,
            CardIntValue provideDiscountForWhileInPlay = null,
            GameStateMethod doSpecializedCleanupAtStartOfCleanup = null,
            GameStateCardMethod doSpecializedActionOnBuyWhileInPlay = null,
            GameStateCardPredicate doSpecializedActionOnTrashWhileInHand = null,
            GameStateCardToPlacement doSpecializedActionOnGainWhileInPlay = null,
            GameStateCardToPlacement doSpecializedActionOnBuyWhileInHand = null,
            GameStateCardToPlacement doSpecializedActionOnGainWhileInHand = null)
        {
            lock (Card.cardTypes)
            {
                if (Card.cardTypes.Contains(this.GetType()))
                {
                    throw new Exception("Do not create duplicate cards.");
                }
                else
                {
                    Card.cardTypes.Add(this.GetType());
                }

                this.privateIndex = Card.lastCardIndex++;
            }

            this.name = name;
            this.pluralName = pluralName != null ? pluralName : name + "s";
            this.expansion = expansion;
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
            this.isPrize = isPrize;
            this.isRuins = isRuins;
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
            this.doSpecializedActionOnGainWhileInPlay = doSpecializedActionOnGainWhileInPlay;
            this.doSpecializedActionOnBuyWhileInHand = doSpecializedActionOnBuyWhileInHand;
            this.doSpecializedActionOnGainWhileInHand = doSpecializedActionOnGainWhileInHand;
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
            return this == other;
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

        virtual public Card CardToMimick(PlayerState currentPlayer, GameState gameState)
        {
            return this;
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
        virtual public bool DoReactionToAttackWhileInHand(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;
            // by default, all cards are affected by attack
            return false;
        }

        // return true if blocks attack;
        virtual public bool DoReactionToAttackWhileInPlayAcrossTurns(PlayerState currentPlayer, GameState gameState)
        {
            // by default, all cards are affected by attack
            return false;
        }
        
        // return true if the card should end up in the trash
        virtual public bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            return true;
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
        
        public bool HasSpecializedActionOnGainWhileInHand
        {
            get
            {
                return this.doSpecializedActionOnGainWhileInHand != null;
            }
        }

        public DeckPlacement DoSpecializedActionOnGainWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (this.HasSpecializedActionOnGainWhileInHand)
            {
                return this.doSpecializedActionOnGainWhileInHand(currentPlayer, gameState, gainedCard);
            }

            return DeckPlacement.Default;
        }   

        public bool HasSpecializedActionOnBuyWhileInHand
        {
            get
            {
                return this.doSpecializedActionOnBuyWhileInHand != null;
            }
        }

        public DeckPlacement DoSpecializedActionOnBuyWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (this.HasSpecializedActionOnBuyWhileInHand)
            {
                return this.doSpecializedActionOnBuyWhileInHand(currentPlayer, gameState, gainedCard);
            }

            return DeckPlacement.Default;
        }   


        public bool HasSpecializedActionOnGainWhileInPlay
        {
            get
            {
                return this.doSpecializedActionOnGainWhileInPlay != null;
            }
        }

        public DeckPlacement DoSpecializedActionOnGainWhileInPlay(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (this.HasSpecializedActionOnGainWhileInPlay)
            {
                return this.doSpecializedActionOnGainWhileInPlay(currentPlayer, gameState, gainedCard);
            }

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

        virtual public void DoSpecializedActionOnReturnToHand(PlayerState currentPlayer, GameState gameState)
        {

        }        

        public bool IsType(Card card)
        {
            if (this == card)
                return true;

            if (this.isRuins && card == Cards.Ruins)
                return true;

            if (this.isPrize && card == Cards.Prize)
                return true;

            return false;
        }

        public static bool DoesCardCost3To6(Card card, PlayerState player)
        {            
            return card.CurrentCoinCost(player) >= 3 && card.CurrentCoinCost(player) <= 6 && card.potionCost == 0;
        }        
    }
}