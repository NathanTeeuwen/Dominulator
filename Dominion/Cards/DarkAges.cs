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

        //TODO:  Hovel reactions    
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

    // Ruins

    public class Ruin :
        Card
    {
        public Ruin()
            : base("Ruin", coinCost: 0, isAction: true, isRuin: true)
        {
        }
    }

    public class AbandonedMine :
        Card
    {
        public AbandonedMine()
            : base("Abandoned Mine", coinCost: 0, isAction: true, plusCoins: 1, isRuin: true)
        {
        }
    }

    public class RuinedLibrary :
        Card
    {
        public RuinedLibrary()
            : base("Ruined Library", coinCost: 0, isAction: true, plusCards: 1, isRuin: true)
        {
        }
    }

    public class RuinedMarket :
        Card
    {
        public RuinedMarket()
            : base("Ruined Market", coinCost: 0, isAction: true, plusBuy: 1, isRuin: true)
        {
        }
    }

    public class RuinedVillage :
        Card
    {
        public RuinedVillage()
            : base("Ruined Village", coinCost: 0, isAction: true, plusActions: 1, isRuin: true)
        {
        }
    }

    public class Survivors :
        Card
    {
        public Survivors()
            : base("Survivors", coinCost: 0, isAction: true, isRuin: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(2);
            // TODO: Require option to put ruins back.
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
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
                        currentPlayer.MoveRevealedCardsToDiscard();
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
            if (gainedCard.Is<CardTypes.Estate>())
            {
                currentPlayer.GainCardsFromSupply<CardTypes.Estate>(gameState, 2); // gain 2 more for total of 3.                
            }
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

        public override void DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruin));
            currentPlayer.GainCardFromSupply(gameState, typeof(CardTypes.Ruin));
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
            currentPlayer.MoveRevealedCardToDiscard(cardToMove => !cardToMove.Equals(foundCard));
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
}