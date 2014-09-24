using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    // Shelters
    public class Necropolis 
        : Card 
    { 
        public static Necropolis card = new Necropolis();

        private Necropolis() 
            : base("Necropolis", Expansion.DarkAges, coinCost: 1, plusActions: 2, isAction: true, isShelter: true) 
        { 
        } 
    }

    public class Hovel
        : Card
    {
        public static Hovel card = new Hovel();

        private Hovel()
            : base("Hovel", Expansion.DarkAges, coinCost: 1, isShelter: true)
        {
            this.doSpecializedActionOnBuyWhileInHand = DoSpecializedActionOnBuyWhileInHand;
        }

        private new DeckPlacement DoSpecializedActionOnBuyWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (gainedCard.isVictory)
            {
                currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => card == Hovel.card, isOptional: true);
            }

            return DeckPlacement.Default;
        }
    }

    public class OvergrownEstate
        : Card
    {
        public static OvergrownEstate card = new OvergrownEstate();

        private OvergrownEstate()
            : base("Overgrown Estate", Expansion.DarkAges, coinCost: 1, victoryPoints: PlayerState => 0, isShelter: true)
        {

        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.DrawAdditionalCardsIntoHand(1);

            return true;
        }
    }

    public class Spoils
       : Card
    {
        public static Spoils card = new Spoils();

        private Spoils()
            : base("Spoils", Expansion.DarkAges, coinCost: 0, plusCoins:3, isAction: true, isTreasure:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {     
            currentPlayer.MoveCardFromPlayToPile(gameState);            
        }
    }

    // Ruins

    public class Ruins
       : Card
    {
        public static Ruins card = new Ruins();

        private Ruins()
            : this("Ruins", Expansion.DarkAges)
        {
        }

        protected Ruins(
            string name,
            Expansion expansion,
            int plusCoins = 0,
            int plusCards = 0,
            int plusBuy = 0,
            int plusActions = 0)
            : base(name, expansion, coinCost: 0, isAction: true, isRuins: true, plusCoins: plusCoins, plusCards: plusCards, plusBuy: plusBuy, plusActions: plusActions)
        {
        }
    }

    public class AbandonedMine
        : Ruins
    {
        new public static AbandonedMine card = new AbandonedMine();

        private AbandonedMine()
            : base("Abandoned Mine", Expansion.DarkAges, plusCoins: 1)
        {            
        }
    }

    public class RuinedLibrary 
        : Ruins
    {
        new public static RuinedLibrary card = new RuinedLibrary();

        private RuinedLibrary()
            : base("Ruined Library", Expansion.DarkAges,  plusCards: 1)
        {
        }
    }

    public class RuinedMarket 
        : Ruins
    {
        new public static RuinedMarket card = new RuinedMarket();

        private RuinedMarket()
            : base("Ruined Market", Expansion.DarkAges, plusBuy: 1)
        {
        }
    }

    public class RuinedVillage 
        : Ruins
    {
        new public static RuinedVillage card = new RuinedVillage();

        private RuinedVillage()
            : base("Ruined Village", Expansion.DarkAges, plusActions: 1)
        {
        }
    }

    public class Survivors 
        : Ruins
    {
        new public static Survivors card = new Survivors();

        private Survivors()
            : base("Survivors", Expansion.DarkAges)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(2);
            // TODO: Require option to put ruins back.
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class Altar
       : Card
    {
        public static Altar card = new Altar();

        private Altar()
            : base("Altar", Expansion.DarkAges, coinCost: 6, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                gameState,
                acceptableCard => true,
                CostConstraint.UpTo,
                5,
                CardRelativeCost.AbsoluteCost);
        }
    }

    public class Armory
       : Card
    {
        public static Armory card = new Armory();

        private Armory()
            : base("Armory", Expansion.DarkAges, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) <= 4 && card.potionCost == 0,
                "Gain a card costing up to 4",
                defaultLocation:DeckPlacement.TopOfDeck);
        }
    }

    public class BandOfMisfits
       : Card
    {
        public static BandOfMisfits card = new BandOfMisfits();

        private BandOfMisfits()
            : base("Band Of Misfits", Expansion.DarkAges, coinCost: 5, isAction: true)
        {
        }

        public override Card CardToMimick(PlayerState currentPlayer, GameState gameState)
        {
            // TODO:  Lots of edge cases to cover still:
            // Make sure that when BandOfMisfits is played as cards such as hermit or scheme have an effect on cleanup, that effect still happens.
            // Also check interactions when played as a duration after having been throne room, kings court etc ...
            // Check to make sure cards like bridge that provide discount while in play work ...
            // make sure procession BandOfMisfits as fortress causes band of misfits to return to hand (because of fortress effect), but gain a card costing up to 6 (procession effect)
            // make sure can play as Sir Martin when that card is top of the knights

            // throw new NOtImplementedException
            Card cardToMimick = currentPlayer.RequestPlayerChooseCardFromSupplyToPlay(gameState, 
                card => card.CurrentCoinCost(currentPlayer) < this.CurrentCoinCost(currentPlayer) && 
                        card.potionCost == 0 && 
                        card.isAction);            
            return cardToMimick;
        }
    }

    public class BanditCamp
       : Card
    {
        public static BanditCamp card = new BanditCamp();

        private BanditCamp()
            : base("Bandit Camp", Expansion.DarkAges, coinCost: 5, isAction: true, plusCards:1, plusActions:2, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Spoils, gameState);
        }
    }

    public class Beggar
       : Card
    {
        public static Beggar card = new Beggar();

        private Beggar()
            : base("Beggar", Expansion.DarkAges, coinCost: 2, isAction: true, isReaction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardsFromSupply(gameState, Copper.card, 3, DeckPlacement.Hand);
        }

        public override bool DoReactionToAttackWhileInHand(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;

            bool wasDiscarded = currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card == Beggar.card, isOptional: true);
            if (!wasDiscarded)
            {
                return false;                
            }

            currentPlayer.GainCardsFromSupply(gameState, Cards.Silver, 1, defaultLocation: DeckPlacement.TopOfDeck);
            currentPlayer.GainCardFromSupply(Cards.Silver, gameState);
            return true;
        }
    }

    public class Catacombs
       : Card
    {
        public static Catacombs card = new Catacombs();

        private Catacombs()
            : base("Catacombs", Expansion.DarkAges, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(3);
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState, actionChoice => actionChoice == PlayerActionChoice.Discard || actionChoice == PlayerActionChoice.PutInHand);
            switch (choice)
            {
                case PlayerActionChoice.Discard:
                    {
                        currentPlayer.MoveRevealedCardsToDiscard(gameState);
                        currentPlayer.DrawAdditionalCardsIntoHand(3);
                        break;
                    }
                case PlayerActionChoice.PutInHand:
                    {
                        currentPlayer.MoveAllRevealedCardsToHand();
                        break;
                    }
                default:
                    throw new Exception();
            }
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.RequestPlayerGainCardFromSupply(gameState, 
                card => card.CurrentCoinCost(selfPlayer) < this.CurrentCoinCost(selfPlayer) &&
                        card.potionCost == 0, 
                "Must gain a card cheaper than this");

            return true;
        }
    }        

    public class Count
       : Card
    {
        public static Count card = new Count();

        private Count()
            : base("Count", Expansion.DarkAges, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard ||
                                    acceptableChoice == PlayerActionChoice.GainCard ||
                                    acceptableChoice == PlayerActionChoice.TopDeck);

            switch (choice)
            {
                case PlayerActionChoice.Discard: currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false); break;
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(Cards.Copper, gameState); break;
                case PlayerActionChoice.TopDeck: currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false); break;
            }

            PlayerActionChoice choice2 = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.PlusCoin ||
                                    acceptableChoice == PlayerActionChoice.Trash ||
                                    acceptableChoice == PlayerActionChoice.GainCard);

            switch (choice2)
            {
                case PlayerActionChoice.PlusCoin: currentPlayer.AddCoins(3); break;
                case PlayerActionChoice.Trash: currentPlayer.TrashHand(gameState); break;
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(Cards.Duchy, gameState); break;
            }
        }
    }

    public class CounterFeit
       : Card
    {
        public static CounterFeit card = new CounterFeit();

        private CounterFeit()
            : base("CounterFeit", Expansion.DarkAges, coinCost: 5, isTreasure: true, plusCoins: 1, plusBuy: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToPlay = currentPlayer.RequestPlayerChooseCardToRemoveFromHandForPlay(gameState, Delegates.IsTreasureCardPredicate, isTreasure: true, isAction: false, isOptional: true);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayTreasure(cardToPlay, gameState);
                currentPlayer.DoPlayTreasure(cardToPlay, gameState);
                currentPlayer.MoveCardFromPlayToTrash(gameState);
            }
        }
    }

    public class Cultist
       : Card
    {
        public static Cultist card = new Cultist();

        private Cultist()
            : base("Cultist", Expansion.DarkAges, coinCost: 5, isAction: true, isAttack:true, requiresRuins:true, plusCards:2, isAttackBeforeAction:true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(Cards.Ruins, gameState);            
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // may play another cultist from hand.            
            currentPlayer.RequestPlayerPlayActionFromHand(gameState, card => card == Cultist.card, isOptional: true);            
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.DrawAdditionalCardsIntoHand(3);

            return true;
        }
    }

    public class DeathCart
       : Card
    {
        public static DeathCart card = new DeathCart();

        private DeathCart()
            : base("Death Cart", Expansion.DarkAges, coinCost: 4, isAction: true, plusCoins: 5, requiresRuins: true)
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

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Ruins, gameState);
            currentPlayer.GainCardFromSupply(Cards.Ruins, gameState);
            return DeckPlacement.Default;
        }
    }    

    public class Feodum
       : Card
    {
        public static Feodum card = new Feodum();

        private Feodum()
            : base("Feodum", Expansion.DarkAges, coinCost: 4, victoryPoints: CountVictoryPoints)
        {
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.GainCardsFromSupply(gameState, Cards.Silver, 3);

            return true;
        }

        private static int CountVictoryPoints(PlayerState player)
        {
            return VictoryCountForSilver(player.AllOwnedCards.CountOf(Cards.Silver));
        }

        public static int VictoryCountForSilver(int silvercount)
        {
            return silvercount / 3;
        }
    }

    public class Forager
      : Card
    {
        public static int CurrentCoinValue(GameState gameState)
        {
            return gameState.CountOfDifferentTreasuresInTrash();
        }

        public static Forager card = new Forager();

        private Forager()
            : base("Forager", Expansion.DarkAges, coinCost: 3, isAction:true, plusBuy:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);
            currentPlayer.AddCoins(CurrentCoinValue(gameState));
        }
    }

    public class Fortress
      : Card
    {
        public static Fortress card = new Fortress();

        private Fortress()
            : base("Fortress", Expansion.DarkAges, coinCost: 4, isAction: true, plusActions: 2, plusCards:1)
        {
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            gameState.gameLog.PlayerPutCardInHand(selfPlayer, this);
            selfPlayer.hand.AddCard(this);

            return false;
        }
    }

    public class Graverobber
      : Card
    {
        public static Graverobber card = new Graverobber();

        private Graverobber()
            : base("Graverobber", Expansion.DarkAges, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState, 
                acceptableChoice => acceptableChoice == PlayerActionChoice.GainCard || acceptableChoice == PlayerActionChoice.Trash);

            if (choice == PlayerActionChoice.GainCard)
            {
                currentPlayer.RequestPlayerGainCardFromTrash(gameState,
                    acceptableCard => CardValidToGainFromTrash(acceptableCard, currentPlayer),
                    "Must gain a card costing between 3 and 6",
                    defaultLocation: DeckPlacement.TopOfDeck);
            }
            else
            {
                Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => CardValidToTrash(acceptableCard), isOptional: false);
                if (trashedCard != null)
                {
                    int maxCost = trashedCard.CurrentCoinCost(currentPlayer) + 3;
                    currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                        acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= maxCost && acceptableCard.potionCost <= trashedCard.potionCost,
                        "gain a card costing up to 3 more than the trashed card");
                }
            }
        }

        public static bool CardValidToGainFromTrash(Card card, PlayerState player)
        {
            return Card.DoesCardCost3To6(card, player);
        }

        public static bool CardValidToTrash(Card card)
        {
            return card.isAction;
        }
    }

    public class Hermit
      : Card
    {
        public static Hermit card = new Hermit();

        private Hermit()
            : base("Hermit", Expansion.DarkAges, coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandOrDiscard(gameState,
                CanTrashCard,
                isOptional: true);

            currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 3 && acceptableCard.potionCost == 0,
                "Gain a card costing up to 3");
        }

        public override void DoSpecializedDiscardFromPlay(PlayerState currentPlayer, GameState gameState)
        {            
            if (currentPlayer.turnCounters.BuysUsed == 0)
            {                
                currentPlayer.MoveCardBeingDiscardedToTrash(gameState);
                currentPlayer.GainCardFromSupply(Cards.Madman, gameState);           
            }            
        }

        public static bool CanTrashCard(Card card)
        {
            return !card.isTreasure;
        }
    }

    public class Madman
      : Card
    {
        public static Madman card = new Madman();

        private Madman()
            : base("Madman", Expansion.DarkAges, coinCost: 0, isAction: true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.MoveCardFromPlayToPile(gameState))
            {
                currentPlayer.DrawAdditionalCardsIntoHand(currentPlayer.Hand.Count);
            }
        }
    }


    public class HuntingGrounds
       : Card
    {
        public static HuntingGrounds card = new HuntingGrounds();

        private HuntingGrounds()
            : base("Hunting Grounds", Expansion.DarkAges, coinCost: 6, plusCards: 4, isAction: true)
        {
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            Card gainedCard = selfPlayer.RequestPlayerGainCardFromSupply(gameState, acceptableCard => acceptableCard == Cards.Duchy || acceptableCard == Cards.Estate, "Choose Duchy or 3 Estate");
            if (gainedCard == null)
                return true;
            
            if (gainedCard == Cards.Estate)
            {
                selfPlayer.GainCardsFromSupply(gameState, Cards.Estate, 2); // gain 2 more for total of 3.                
            }

            return true;
        }
    }

    public class IronMonger
       : Card
    {
        public static IronMonger card = new IronMonger();

        private IronMonger()
            : base("IronMonger", Expansion.DarkAges, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(1);
            Card revealedCard  = currentPlayer.CardsBeingRevealed.First();

            currentPlayer.RequestPlayerDiscardRevealedCard(gameState);
            currentPlayer.MoveRevealedCardToTopOfDeck();

            if (revealedCard.isAction)
            {
                currentPlayer.AddActions(1);
            }

            if (revealedCard.isTreasure)
            {
                currentPlayer.AddCoins(1);
            }

            if (revealedCard.isVictory)
            {
                currentPlayer.DrawOneCardIntoHand();
            }
        }
    }

    public class JunkDealer
       : Card
    {
        public static JunkDealer card = new JunkDealer();

        private JunkDealer()
            : base("Junk Dealer", Expansion.DarkAges, coinCost: 5, isAction: true, plusCards:1, plusActions:1, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
        }
    }

    public class Knights
       : Card
    {
        public static Knights card = new Knights();

        private Knights()
            : base("Knights", Expansion.DarkAges, coinCost: 5, isAction: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Marauder
       : Card
    {
        public static Marauder card = new Marauder();

        private Marauder()
            : base("Marauder", Expansion.DarkAges, coinCost: 4, isAction: true, isAttack: true, requiresRuins:true, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Spoils, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(Cards.Ruins, gameState);
        }
    }

    public class MarketSquare
       : Card
    {
        public static MarketSquare card = new MarketSquare();

        private MarketSquare()
            : base("Market Square", Expansion.DarkAges, coinCost: 3, isAction: true, plusCards:1, plusActions:1, plusBuy:1)
        {
            this.doSpecializedActionOnTrashWhileInHand = DoSpecializedActionOnTrashWhileInHand;
        }

        private new bool DoSpecializedActionOnTrashWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (currentPlayer.actions.ShouldPlayerDiscardCardFromHand(gameState, this))
            {
                currentPlayer.DiscardCardFromHand(gameState, this);
                currentPlayer.GainCardFromSupply(Gold.card, gameState);
                return true;
            }

            return false;
        }        
    }

    public class Mystic
       : Card
    {
        public static Mystic card = new Mystic();

        private Mystic()
            : base("Mystic", Expansion.DarkAges, coinCost: 5, isAction: true, plusCoins: 2, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            //currentPlayer.gameLog.LogDeck(gameState.players.CurrentPlayer);
            Card cardType = currentPlayer.GuessCardTopOfDeck(gameState);
            currentPlayer.RevealCardsFromDeck(1);
            if (currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                currentPlayer.MoveRevealedCardToHand(cardType);
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }
        }
    }

    public class Pillage
      : Card
    {
        public static Pillage card = new Pillage();

        private Pillage()
            : base("Pillage", Expansion.DarkAges, coinCost: 5, isAction: true, attackDependsOnPlayerChoice: true, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            currentPlayer.MoveCardFromPlayToTrash(gameState);

            PlayerState.AttackAction attackAction = delegate(PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
            {
                if (otherPlayer.Hand.Count >= 5)
                {                    
                    currentPlayer2.RequestPlayerDiscardCardFromOtherPlayersHand(gameState, otherPlayer);
                }
            };
            currentPlayer.AttackOtherPlayers(gameState, attackAction);

            currentPlayer.GainCardsFromSupply(gameState, Cards.Spoils, 2);
        }        
    }

    public class PoorHouse
       : Card
    {
        public static PoorHouse card = new PoorHouse();

        private PoorHouse()
            : base("Poor House", Expansion.DarkAges, coinCost: 1, isAction: true, plusCoins: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();

            currentPlayer.AddCoins(0 - currentPlayer.Hand.CountWhere(card => card.isTreasure));
        }
    }

    public class Procession
      : Card
    {
        public static Procession card = new Procession();

        private Procession()
            : base("Procession", Expansion.DarkAges, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardToPlay = currentPlayer.RequestPlayerChooseCardToRemoveFromHandForPlay(gameState, acceptableCard => acceptableCard.isAction, isTreasure: false, isAction: true, isOptional: true);
            if (cardToPlay != null)
            {                
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 2);
                currentPlayer.MoveCardFromPlayedAreaToTrash(cardToPlay, gameState);

                currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                    acceptableCard => 
                        acceptableCard.CurrentCoinCost(currentPlayer) == cardToPlay.CurrentCoinCost(currentPlayer) + 1 && 
                        acceptableCard.potionCost == cardToPlay.potionCost &&
                        acceptableCard.isAction,
                    "must gain an action card costing exactly one more than the trashed card");
            }
        }
    }

    public class Rats
       : Card
    {
        public static Rats card = new Rats();

        private Rats()
            : base("Rats", Expansion.DarkAges, coinCost: 4, isAction: true, plusCards: 1, plusActions: 1, defaultSupplyCount: 20)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Rats, gameState);
            CardPredicate cardsToTrash = card => card != Rats.card;
            if (currentPlayer.Hand.HasCard(cardsToTrash))
            {
                currentPlayer.RequestPlayerTrashCardFromHand(gameState, cardsToTrash, isOptional: false);
            }
            else
            {
                currentPlayer.RevealHand();
            }
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.DrawAdditionalCardsIntoHand(1);

            return true;
        }
    }

    public class Rebuild
       : Card
    {
        public static Rebuild card = new Rebuild();

        private Rebuild()
            : base("Rebuild", Expansion.DarkAges, coinCost: 5, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardType = currentPlayer.RequestPlayerNameACard(gameState);

            Card foundCard = null;
            gameState.gameLog.PushScope();
            while (true)
            {
                foundCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (foundCard == null)
                    break;

                if (foundCard.isVictory && foundCard != cardType)
                {
                    break;
                }
            }
            currentPlayer.MoveRevealedCardsToDiscard(cardToMove => !cardToMove.Equals(foundCard), gameState);
            gameState.gameLog.PopScope();

            if (foundCard != null)
            {
                int cardCost = foundCard.CurrentCoinCost(currentPlayer);
                currentPlayer.MoveRevealedCardToTrash(foundCard, gameState);
                currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                    acceptableCard => acceptableCard.isVictory && acceptableCard.CurrentCoinCost(currentPlayer) <= cardCost + 3 && acceptableCard.potionCost <= foundCard.potionCost,
                    "Gain a victory card costing up to 3 more than the trashed card.");
            }
        }
    }

    public class Rogue
      : Card
    {
        public static Rogue card = new Rogue();

        private Rogue()
            : base("Rogue", Expansion.DarkAges, coinCost: 5, isAction: true, plusCoins:2, isAttack:true, attackDependsOnPlayerChoice:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerState.AttackAction attackAction = this.DoEmptyAttack;
            
            CardPredicate acceptableCard = card => Card.DoesCardCost3To6(card, currentPlayer);
            if (gameState.trash.AnyWhere(acceptableCard))
            {
                currentPlayer.RequestPlayerGainCardFromTrash(gameState, acceptableCard, "Card from 3 to 6");
            }
            else
            {
                attackAction = Rogue.AttackAction;
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);
        }

        private static void AttackAction(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(2);
            otherPlayer.RequestPlayerTrashRevealedCard(gameState, card => Card.DoesCardCost3To6(card, otherPlayer));
            otherPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Sage
      : Card
    {
        public static Sage card = new Sage();

        private Sage()
            : base("Sage", Expansion.DarkAges, coinCost: 3, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (true)
            {
                Card card = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (card == null)
                    break;
                if (card.CurrentCoinCost(currentPlayer) >= 3 && card.potionCost == 0)
                {
                    currentPlayer.MoveRevealedCardToHand(card);
                    break;
                }
            }
            currentPlayer.MoveRevealedCardsToDiscard(gameState);
        }
    }

    public class Scavenger
      : Card
    {
        public static Scavenger card = new Scavenger();

        private Scavenger()
            : base("Scavenger", Expansion.DarkAges, coinCost: 4, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.actions.ShouldPutDeckInDiscard(gameState);
            currentPlayer.RequestPlayerTopDeckCardFromDiscard(gameState, isOptional: true);
        }
    }

    public class Squire
      : Card
    {
        public static Squire card = new Squire();

        private Squire()
            : base("Squire", Expansion.DarkAges, coinCost: 2, isAction: true, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice =>
                    acceptableChoice == PlayerActionChoice.PlusAction ||
                    acceptableChoice == PlayerActionChoice.PlusBuy ||
                    acceptableChoice == PlayerActionChoice.GainCard);

            switch (choice)
            {
                case PlayerActionChoice.PlusAction: currentPlayer.AddActions(2); break;
                case PlayerActionChoice.PlusBuy: currentPlayer.AddBuys(2); break;
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(Cards.Silver, gameState); break;
                default: throw new Exception();
            }
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.isAttack, "Must gain an attack card", isOptional: false);

            return true;
        }
    }

    public class Storeroom
      : Card
    {
        public static Storeroom card = new Storeroom();

        private Storeroom()
            : base("Storeroom", Expansion.DarkAges, coinCost: 3, isAction: true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            int discardedCount = currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, currentPlayer.Hand.Count, isOptional:true);
            currentPlayer.DrawAdditionalCardsIntoHand(discardedCount);
            discardedCount = currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, currentPlayer.Hand.Count, isOptional: true);
            currentPlayer.AddCoins(discardedCount);

            // TODO:  How does the player know they are discarding for coins or for card?
            // throw new NotImplementedException();
        }
    }

    public class Urchin
      : Card
    {
        public static Urchin card = new Urchin();

        private Urchin()
            : base("Urchin", Expansion.DarkAges, coinCost: 3, isAction: true, isAttack:true, plusCards:1, plusActions:1)
        {
            this.doSpecializedActionToCardWhileInPlay = DoSpecializedActionToCardWhileInPlay;
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 4);
        }

        private void DoSpecializedActionToCardWhileInPlay(PlayerState currentPlayer, GameState gameState, Card card)
        {
            if (card.isAttack)
            {
                if (currentPlayer.actions.ShouldTrashCard(gameState, this))
                {
                    throw new NotImplementedException();
                    // something like this:
                    //currentPlayer.MoveCardFromPlayedCardToNativeVillageMatt(this);
                    //currentPlayer.GainCardFromSupply(Cards.Mercenary, gameState);
                }
            }
        }
    }

    public class Mercenary
      : Card
    {
        public static Mercenary card = new Mercenary();

        private Mercenary()
            : base("Mercenary", Expansion.DarkAges, coinCost: 0, isAction: true, isAttack: true, attackDependsOnPlayerChoice:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            bool[] otherPlayersAffectedByAttacks = new bool[gameState.players.PlayerCount-1];

            // from rule book
            // "Players responding to this attack must choose to do so before you decide whether or not to trash 2 cards"
            int otherIndex = 0;
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayersAffectedByAttacks[otherIndex++] = otherPlayer.IsAffectedByAttacks(gameState);                
            }

            if (currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 2, isOptional: true, allOrNone:true).Count == 2)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2);
                currentPlayer.AddCoins(2);

                otherIndex = 0;
                foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                {
                    if (otherPlayersAffectedByAttacks[otherIndex++])
                    {
                        otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);                        
                    }
                }
            }           
        }
    }

    public class Vagrant
      : Card
    {
        public static Vagrant card = new Vagrant();

        private Vagrant()
            : base("Vagrant", Expansion.DarkAges, coinCost: 2, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(1);
            Card revealedCard = currentPlayer.cardsBeingRevealed.FirstOrDefault();
            if (revealedCard == null)
            {
                return;
            }

            if (revealedCard.isCurse || revealedCard.isRuins || revealedCard.isShelter || revealedCard.isVictory)
            {
                currentPlayer.MoveRevealedCardToHand(revealedCard);
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }
        }
    }

    public class WanderingMinstrel
      : Card
    {
        public static WanderingMinstrel card = new WanderingMinstrel();

        private WanderingMinstrel()
            : base("Wandering Minstrel", Expansion.DarkAges, coinCost: 4, isAction: true, plusCards: 1, plusActions: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(3);
            currentPlayer.MoveRevealedCardsToDiscard(card => !card.isAction, gameState);
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }
}