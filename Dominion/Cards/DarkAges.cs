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
    public class Necropolis : Card { public Necropolis() : base("Necropolis", coinCost: 1, plusActions: 2, isAction: true, isShelter: true) { } }

    public class Hovel
        : Card
    {
        public Hovel()
            : base("Hovel", coinCost: 1, isShelter: true)
        {
        }

        public override DeckPlacement DoSpecializedActionOnBuyWhileInHand(PlayerState currentPlayer, GameState gameState, Card gainedCard)
        {
            if (gainedCard.isVictory)
            {
                currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => card.Is<Hovel>(), isOptional: true);
            }

            return DeckPlacement.Default;
        }
    }

    public class OvergrownEstate
        : Card
    {
        public OvergrownEstate()
            : base("Overgrown Estate", coinCost: 1, victoryPoints: PlayerState => 0, isShelter: true)
        {

        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(1);
        }
    }

    public class Spoils :
        Card
    {
        public Spoils()
            : base("Spoils", coinCost: 0, plusCoins:3, isAction: true, isTreasure:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {     
            currentPlayer.MoveCardFromPlayToPile(gameState);            
        }
    }

    // Ruins

    public class Ruins :
        Card
    {
        public Ruins()
            : this("Ruins")
        {
        }

        public Ruins(
            string name,
            int plusCoins = 0,
            int plusCards = 0,
            int plusBuy = 0,
            int plusActions = 0)
            : base(name, coinCost: 0, isAction: true, isRuin: true, plusCoins:plusCoins, plusCards: plusCards, plusBuy: plusBuy, plusActions:plusActions)
        {
        }
    }

    public class AbandonedMine :
        Ruins
    {
        public AbandonedMine()
            : base("Abandoned Mine", plusCoins: 1)
        {            
        }
    }

    public class RuinedLibrary :
        Ruins
    {
        public RuinedLibrary()
            : base("Ruined Library",  plusCards: 1)
        {
        }
    }

    public class RuinedMarket :
        Ruins
    {
        public RuinedMarket()
            : base("Ruined Market", plusBuy: 1)
        {
        }
    }

    public class RuinedVillage :
        Ruins
    {
        public RuinedVillage()
            : base("Ruined Village", plusActions: 1)
        {
        }
    }

    public class Survivors :
        Ruins
    {
        public Survivors()
            : base("Survivors")
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(2);
            // TODO: Require option to put ruins back.
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }
    }

    public class Altar :
        Card
    {
        public Altar()
            : base("Altar", coinCost: 6, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) <= 5,
                "Gain a card costing up to 5");
        }
    }

    public class Armory :
        Card
    {
        public Armory()
            : base("Armory", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) <= 4,
                "Gain a card costing up to 4",
                defaultLocation:DeckPlacement.TopOfDeck);
        }
    }

    public class BandOfMisfits :
        Card
    {
        public BandOfMisfits()
            : base("BandOfMisfits", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class BanditCamp :
        Card
    {
        public BanditCamp()
            : base("BanditCamp", coinCost: 5, isAction: true, plusCards:1, plusActions:2, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<CardTypes.Spoils>(gameState);
        }
    }

    public class Beggar :
        Card
    {
        public Beggar()
            : base("Beggar", coinCost: 2, isAction: true, isReaction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardsFromSupply<Copper>(gameState, 3, DeckPlacement.Hand);
        }

        public override bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState, out bool cancelsAttack)
        {
            cancelsAttack = false;

            bool wasDiscarded = currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.Is<Beggar>(), isOptional: true);
            if (!wasDiscarded)
            {
                return false;                
            }

            currentPlayer.GainCardsFromSupply<CardTypes.Silver>(gameState, 1, defaultLocation: DeckPlacement.TopOfDeck);
            currentPlayer.GainCardFromSupply<CardTypes.Silver>(gameState);
            return true;
        }
    }

    public class Catacombs :
        Card
    {
        public Catacombs()
            : base("Catacombs", coinCost: 5, isAction: true)
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
                        currentPlayer.MoveRevealedCardsToHand(acceptableCard => true);
                        break;
                    }
                default:
                    throw new Exception();
            }
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) < this.CurrentCoinCost(currentPlayer), "Must gain a card cheaper than this");
        }
    }        

    public class Count :
        Card
    {
        public Count()
            : base("Count", coinCost: 5, isAction: true)
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
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Copper)); break;
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
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Duchy)); break;
            }
        }
    }

    public class CounterFeit :
        Card
    {
        public CounterFeit()
            : base("CounterFeit", coinCost: 5, isTreasure: true, plusCoins: 1, plusBuy: 1)
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

    public class Cultist :
        Card
    {
        public Cultist()
            : base("Cultist", coinCost: 5, isAction: true, isAttack:true, requiresRuins:true, plusCards:2, isAttackBeforeAction:true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruins));            
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // may play another cultist from hand.            
            currentPlayer.RequestPlayerPlayActionFromHand(gameState, card => card.Is<Cultist>(), isOptional: true);            
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(3);
        }
    }

    public class DeathCart :
        Card
    {
        public DeathCart()
            : base("Death Cart", coinCost: 4, isAction: true, plusCoins: 5, requiresRuins: true)
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
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruins));
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruins));
            return DeckPlacement.Default;
        }
    }    

    public class Feodum :
        Card
    {
        public Feodum()
            : base("Feodum", coinCost: 4, victoryPoints: CountVictoryPoints)
        {
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardsFromSupply(gameState, typeof(CardTypes.Silver), 3);
        }

        private static int CountVictoryPoints(PlayerState player)
        {
            return VictoryCountForSilver(player.AllOwnedCards.Where(card => card.Is<CardTypes.Silver>()).Count());
        }

        public static int VictoryCountForSilver(int silvercount)
        {
            return silvercount / 3;
        }
    }

    public class Forager :
       Card
    {
        public static int CurrentCoinValue(GameState gameState)
        {
            return gameState.CountOfDifferentTreasuresInTrash();
        }

        public Forager()
            : base("Forager", coinCost: 3, isAction:true, plusBuy:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);
            currentPlayer.AddCoins(CurrentCoinValue(gameState));
        }
    }

    public class Fortress :
       Card
    {
        public Fortress()
            : base("Fortress", coinCost: 4, isAction: true, plusActions: 2, plusCards:1)
        {
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Graverobber :
       Card
    {
        public Graverobber()
            : base("Graverobber", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Hermit :
       Card
    {
        public Hermit()
            : base("Hermit", coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Madman :
       Card
    {
        public Madman()
            : base("Madman", coinCost: 0, isAction: true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }


    public class HuntingGrounds :
        Card
    {
        public HuntingGrounds()
            : base("Hunting Grounds", coinCost: 6, plusCards: 4, isAction: true)
        {
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, acceptableCard => acceptableCard.Is<CardTypes.Duchy>() || acceptableCard.Is<CardTypes.Estate>(), "Choose Duchy or 3 Estate");
            if (gainedCard == null)
                return;
            
            if (gainedCard.Is<CardTypes.Estate>())
            {
                currentPlayer.GainCardsFromSupply<CardTypes.Estate>(gameState, 2); // gain 2 more for total of 3.                
            }
        }
    }

    public class IronMonger :
        Card
    {
        public IronMonger()
            : base("IronMongerGrounds", coinCost: 4, isAction: true)
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

    public class JunkDealer :
        Card
    {
        public JunkDealer()
            : base("JunkDealer", coinCost: 5, isAction: true, plusCards:1, plusActions:1, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
        }
    }

    public class Knights :
        Card
    {
        public Knights()
            : base("Knights", coinCost: 0, isAction: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Marauder :
        Card
    {
        public Marauder()
            : base("Marauder", coinCost: 4, isAction: true, isAttack: true, requiresRuins:true, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<CardTypes.Spoils>(gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruins));
        }
    }

    public class MarketSquare :
        Card
    {
        public MarketSquare()
            : base("MarketSquare", coinCost: 3, isAction: true, plusCards:1, plusActions:1, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Mystic :
        Card
    {
        public Mystic()
            : base("Mystic", coinCost: 5, isAction: true, plusCoins: 2, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            //currentPlayer.gameLog.LogDeck(gameState.players.CurrentPlayer);
            Type cardType = currentPlayer.GuessCardTopOfDeck(gameState);
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

    public class Pillage :
       Card
    {
        public Pillage()
            : base("Pillage", coinCost: 5, isAction: true, attackDependsOnPlayerChoice: true, requiresSpoils:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
            currentPlayer.MoveCardFromPlayToTrash(gameState);

            PlayerState.AttackAction attackAction = delegate(PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
            {
                if (otherPlayer.Hand.Count() >= 5)
                {
                    // TODO: make other player discard a good card
                    otherPlayer.RequestPlayerDiscardCardFromOtherPlayersHand(gameState, otherPlayer);
                }
            };
            currentPlayer.AttackOtherPlayers(gameState, attackAction);

            currentPlayer.GainCardsFromSupply<CardTypes.Spoils>(gameState, 2);
        }        
    }

    public class PoorHouse :
        Card
    {
        public PoorHouse()
            : base("Poor House", coinCost: 1, isAction: true, plusCoins: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();

            currentPlayer.AddCoins(0 - currentPlayer.Hand.Where(card => card.isTreasure).Count());
        }
    }

    public class Procession :
       Card
    {
        public Procession()
            : base("Procession", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Rats :
        Card
    {
        public Rats()
            : base("Rats", coinCost: 4, isAction: true, plusCards: 1, plusActions: 1, defaultSupplyCount: 20)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<CardTypes.Rats>(gameState);
            CardPredicate cardsToTrash = card => !card.Is<Rats>();
            if (currentPlayer.Hand.HasCard(cardsToTrash))
            {
                currentPlayer.RequestPlayerTrashCardFromHand(gameState, cardsToTrash, isOptional: false);
            }
            else
            {
                currentPlayer.RevealHand();
            }
        }

        public override void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(1);
        }
    }

    public class Rebuild :
        Card
    {
        public Rebuild()
            : base("Rebuild", coinCost: 5, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type cardType = currentPlayer.RequestPlayerNameACard(gameState);

            Card foundCard = null;
            gameState.gameLog.PushScope();
            while (true)
            {
                foundCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (foundCard == null)
                    break;

                if (foundCard.isVictory && !foundCard.Is(cardType))
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
                    acceptableCard => acceptableCard.isVictory && acceptableCard.CurrentCoinCost(currentPlayer) <= cardCost + 3,
                    "Gain a victory card costing up to 3 more than the trashed card.");
            }
        }
    }

    public class Rogue :
       Card
    {
        public Rogue()
            : base("Rogue", coinCost: 5, isAction: true, isAttack:true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Sage :
       Card
    {
        public Sage()
            : base("Sage", coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Scavenger :
       Card
    {
        public Scavenger()
            : base("Scavenger", coinCost: 4, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Squire :
       Card
    {
        public Squire()
            : base("Squire", coinCost: 2, isAction: true, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Storeroom :
       Card
    {
        public Storeroom()
            : base("Storeroom", coinCost: 3, isAction: true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Urchin :
       Card
    {
        public Urchin()
            : base("Urchin", coinCost: 3, isAction: true, isAttack:true, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Mercenary :
       Card
    {
        public Mercenary()
            : base("Mercenary", coinCost: 0, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Vagrant :
       Card
    {
        public Vagrant()
            : base("Vagrant", coinCost: 2, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class WanderingMinstrell :
       Card
    {
        public WanderingMinstrell()
            : base("WanderingMinstrell", coinCost: 4, isAction: true, plusCards: 1, plusActions: 2)
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