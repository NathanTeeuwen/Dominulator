using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public abstract class Card
        : CardShapedObject,
          IEquatable<Card>
    {
        private readonly int coinCost;
        public readonly int potionCost;
        public readonly int debtCost;

        public readonly bool isKingdomCard;    // all cards that can be used to shuffle and make up the 10 cards of a kingdom.   For split piles, the randomizer card isKingdom and the cards that make up the piles are not
        public readonly StartingLocation startingLocation;     // cards are either in your hand, in the supply or in the non-supply
        public readonly int defaultSupplyCount;

        // types
        public readonly bool isAction;
        public readonly bool isAttack;
        public readonly bool isCastle;
        public readonly bool isCurse;
        public readonly bool isDuration;
        public readonly bool isFate;
        public readonly bool isDoom;
        public readonly bool isGathering;
        public readonly bool isHeirloom;
        public readonly bool isLooter;
        public readonly bool isNight;
        public readonly bool isPrize;
        public readonly bool isReaction;
        public readonly bool isReserve;
        public readonly bool isRuins;
        public readonly bool isShelter;
        public readonly bool isSpirit;
        public readonly bool isZombie;
        public readonly bool isTreasure;
        public readonly bool isTraveller;

        public bool isVictory
        {
            get
            {
                return this.victoryPointCounter != null;
            }
        }

        public IEnumerable<CardType> CardTypes
        {
            get
            {
                if (this.isVictory)
                    yield return CardType.Victory;
                if (this.isAction)
                    yield return CardType.Action;
                if (this.isAttack)
                    yield return CardType.Attack;
                if (this.isCastle)
                    yield return CardType.Castle;
                if (this.isCurse)
                    yield return CardType.Curse;
                if (this.isDuration)
                    yield return CardType.Duration;
                if (this.isFate)
                    yield return CardType.Fate;
                if (this.isDoom)
                    yield return CardType.Doom;
                if (this.isGathering)
                    yield return CardType.Gathering;
                if (this.isLooter)
                    yield return CardType.Looter;
                if (this.isHeirloom)
                    yield return CardType.Heirloom;
                if (this.isNight)
                    yield return CardType.Night;
                if (this.isPrize)
                    yield return CardType.Prize;
                if (this.isReaction)
                    yield return CardType.Reaction;
                if (this.isReserve)
                    yield return CardType.Reserve;
                if (this.isRuins)
                    yield return CardType.Ruins;
                if (this.isShelter)
                    yield return CardType.Shelter;
                if (this.isSpirit)
                    yield return CardType.Spirit;
                if (this.isZombie)
                    yield return CardType.Zombie;
                if (this.isTreasure)
                    yield return CardType.Treasure;
                if (this.isTraveller)
                    yield return CardType.Traveller;
            }
        }

        // part of action
        public readonly int plusAction;
        public readonly int plusBuy;
        public readonly int plusCard;
        public readonly int plusCoin;
        public readonly int plusVictoryToken;

        // affects behavior
        public readonly bool isAttackBeforeAction;
        public readonly bool canOverpay;

        // useful properites about the card
        public readonly bool canGivePlusAction;
        public readonly bool attackDependsOnPlayerChoice;
        public readonly bool requiresSpoils;

        // attached methods
        protected VictoryPointCounter victoryPointCounter;              // readonly
        protected GameStateMethod doSpecializedCleanupAtStartOfCleanup; // readonly
        protected CardIntValue provideDiscountForWhileInPlay;           // readonly
        protected GameStateCardMethod doSpecializedActionOnBuyWhileInPlay; // readonly
        protected GameStateCardPredicate doSpecializedActionOnTrashWhileInHand; //readonly
        protected GameStateCardMethod doSpecializedActionToCardWhileInPlay;  //readonly
        protected GameStateCardToPlacement doSpecializedActionOnGainWhileInPlay; //readonly
        protected GameStateCardToPlacement doSpecializedActionOnBuyWhileInHand; //readonly 
        protected GameStateCardToPlacement doSpecializedActionOnGainWhileInHand; //readonly

        // properties of cards used that don't affect behavior
        public readonly bool mightMultiplyActions;

        protected Card(
            string name,
            Expansion expansion,
            int coinCost = 0,
            int potionCost = 0,
            int debtCost = 0,
            Edition edition = Edition.First,
            bool isDeprecated = false,
            string pluralName = null,
            StartingLocation startingLocation = Dominion.StartingLocation.Supply,
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
            bool isNight = false,
            bool isPrize = false,
            bool isRuins = false,
            bool isTreasure = false,
            bool isDuration = false,
            bool isLooter = false,
            bool requiresSpoils = false,
            bool isShelter = false,
            bool isTraveller = false,
            bool isReserve = false,
            bool isGathering = false,
            bool isHeirloom = false,
            bool isFate = false,
            bool isDoom = false,
            bool isZombie = false,
            bool isSpirit = false,
            bool isCastle = false,
            bool canOverpay = false,
            bool canGivePlusAction = false,
            bool mightMultiplyActions = false,
            bool isKingdomCard = true,
            CardIntValue provideDiscountForWhileInPlay = null,
            GameStateMethod doSpecializedCleanupAtStartOfCleanup = null,
            GameStateCardMethod doSpecializedActionOnBuyWhileInPlay = null,
            GameStateCardPredicate doSpecializedActionOnTrashWhileInHand = null,
            GameStateCardToPlacement doSpecializedActionOnGainWhileInPlay = null,
            GameStateCardToPlacement doSpecializedActionOnBuyWhileInHand = null,
            GameStateCardToPlacement doSpecializedActionOnGainWhileInHand = null,
            GameStateCardMethod      doSpecializedActionToCardWhileInPlay = null)
            : base(name, expansion, edition, isDeprecated, pluralName)
        {
            this.coinCost = coinCost;
            this.potionCost = potionCost;
            this.debtCost = debtCost;
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
            this.isNight = isNight;
            this.isPrize = isPrize;
            this.isRuins = isRuins;
            this.isTreasure = isTreasure;
            this.isFate = isFate;
            this.isDoom = isDoom;
            this.isHeirloom = isHeirloom;
            this.isSpirit = isSpirit;
            this.isZombie = isZombie;
            this.isCastle = isCastle;
            this.isLooter = isLooter;
            this.defaultSupplyCount = defaultSupplyCount;
            this.isLooter = isLooter;
            this.isDuration = isDuration;
            this.isShelter = isShelter;
            this.isTraveller = isTraveller;
            this.isReserve = isReserve;
            this.isGathering = isGathering;
            this.isKingdomCard = isKingdomCard;
            this.requiresSpoils = requiresSpoils;
            this.canOverpay = canOverpay;
            this.canGivePlusAction = canGivePlusAction;
            this.mightMultiplyActions = mightMultiplyActions;
            this.provideDiscountForWhileInPlay = provideDiscountForWhileInPlay;
            this.doSpecializedCleanupAtStartOfCleanup = doSpecializedCleanupAtStartOfCleanup;
            this.doSpecializedActionOnBuyWhileInPlay = doSpecializedActionOnBuyWhileInPlay;
            this.doSpecializedActionOnTrashWhileInHand = doSpecializedActionOnTrashWhileInHand;
            this.doSpecializedActionOnGainWhileInPlay = doSpecializedActionOnGainWhileInPlay;
            this.doSpecializedActionOnBuyWhileInHand = doSpecializedActionOnBuyWhileInHand;
            this.doSpecializedActionOnGainWhileInHand = doSpecializedActionOnGainWhileInHand;
            this.doSpecializedActionToCardWhileInPlay = doSpecializedActionToCardWhileInPlay;
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

        public CostComparison GetRelativeCost(GameState gameState, Card otherCard)
        {
            return new CostComparison(
                this.CurrentCoinCost(gameState.players.CurrentPlayer) - otherCard.CurrentCoinCost(gameState.players.CurrentPlayer),
                this.potionCost - otherCard.potionCost,
                this.debtCost - otherCard.debtCost
                );
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
        
        public bool HasSpecializedActionToCardWhileInPlay
        {
            get
            {
                return this.doSpecializedActionToCardWhileInPlay != null;
            }
        }

        public void DoSpecializedActionToCardWhileInPlay(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (this.HasSpecializedActionToCardWhileInPlay)
            {
                this.doSpecializedActionToCardWhileInPlay(currentPlayer, gameState, gainedCard);
            }
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

        internal virtual void AddAdditionalCardsNeeded(GameConfig.CardGainAvailabilityBuilder builder)
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

    public class Event
      : CardShapedObject
    {
        public readonly int coinCost;
        public readonly int debtCost;
        protected Event(
            string name,
            Expansion expansion,
            int coinCost,
            int debtCost = 0,
            string pluralName = null)
            : base(name: name, expansion: expansion, pluralName: pluralName)
        {
            this.coinCost = coinCost;
            this.debtCost = debtCost;
        }

        public virtual void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Landmark
      : CardShapedObject
    {
        protected Landmark(
            string name,
            Expansion expansion,
            string pluralName = null)
            : base(name: name, expansion: expansion, pluralName: null)
        {

        }

        public virtual void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Artifact
        : CardShapedObject
    {
        protected Artifact(
           string name,
           Expansion expansion)
           : base(name: name, expansion: expansion)
        {

        }

        public virtual void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Project
        : CardShapedObject
    {
        public readonly int coinCost;

        protected Project(
           string name,
           Expansion expansion,
           int cost)
           : base(name: name, expansion: expansion)
        {
            this.coinCost = cost;
        }

        public virtual void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public struct CostComparison
    {
        int coinDifference;
        int potionDifference;
        int debtDifference;

        public CostComparison(int coinDifference, int potionDifference, int debtDifference)
        {
            this.coinDifference = coinDifference;
            this.potionDifference = potionDifference;
            this.debtDifference = debtDifference;
        }

        public bool ExactlyEquals()
        {
            return this.coinDifference == 0 && this.potionDifference == 0 && this.debtDifference == 0;
        }

        public bool CostsMoreThan()
        {
            var costsMoreOnSome = this.coinDifference > 0 || this.potionDifference > 0 || this.debtDifference > 0;
            var costsAtLeastAsMuchOnAll = this.coinDifference >= 0 && this.potionDifference >= 0 && this.debtDifference >= 0;
            return costsMoreOnSome && costsAtLeastAsMuchOnAll;
        }
    }

    public class CardShapedObject
    {
        public readonly string name;
        public readonly string backName;
        public readonly string pluralName;
        public readonly Expansion expansion;
        public readonly Edition edition;
        public readonly bool isDeprecated;

        private readonly int privateIndex;
        private static int lastCardIndex = 0;
        private static HashSet<Type> initializedCardTypes = new HashSet<Type>();

        internal int Index
        {
            get
            {
                return this.privateIndex;
            }
        }

        public CardShapedObject(string name, Expansion expansion, Edition edition = Edition.First, bool isDeprecated = false, string pluralName = null)
        {
            lock (CardShapedObject.initializedCardTypes)
            {
                if (CardShapedObject.initializedCardTypes.Contains(this.GetType()))
                {
                    throw new Exception("Do not create duplicate cards.");
                }
                else
                {
                    CardShapedObject.initializedCardTypes.Add(this.GetType());
                }

                this.privateIndex = CardShapedObject.lastCardIndex++;
            }

            this.name = name;
            this.expansion = expansion;
            this.edition = edition;
            this.pluralName = pluralName != null ? pluralName : name + "s";
            this.isDeprecated = isDeprecated;
        }

        public static string GetProgrammaticName(string str)
        {
            var stringBuilder = new System.Text.StringBuilder();
            foreach(char c in str)
            {
                if (System.Char.IsLetter(c))
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public string ProgrammaticName
        {
            get
            {
                return GetProgrammaticName(this.name);
            }
        }
    }

    public abstract class Hex
        : CardShapedObject
    {
        public Hex(string name, Expansion expansion, string pluralName = null)
            : base(name, expansion, pluralName:pluralName)
        {
        }

        public abstract void DoSpecializedHex(PlayerState currentPlayer, GameState gameState);
    }

    public class State
        : CardShapedObject
    {
        public State(string name, Expansion expansion, string pluralName = null)
            : base(name, expansion, pluralName:pluralName)
        {
        }
    }

    public abstract class Boon
        : CardShapedObject
    {
        public Boon(string name, Expansion expansion, string pluralName = null)
            : base(name, expansion, pluralName:pluralName)
        {
        }

        public abstract void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState);
    }
}