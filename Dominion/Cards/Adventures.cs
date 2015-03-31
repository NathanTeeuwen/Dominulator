using System;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Hero
        : Card
    {
        public static Hero card = new Hero();

        private Hero()
            : base("Hero", Expansion.Adventures, coinCost: 5, plusCoins: 2, isAction: true, isTraveller:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.isTreasure, "Gain a Treasure");
        }

        public override void DoSpecializedDiscardFromPlay(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class LostCity
        : Card
    {
        public static LostCity card = new LostCity();

        private LostCity()
            : base("Lost City", Expansion.Adventures, coinCost: 5, plusCards: 2, plusActions: 2, isAction: true)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            foreach (var player in gameState.players.OtherPlayers)
                player.DrawOneCardIntoHand(gameState);

            return base.DoSpecializedWhenGain(currentPlayer, gameState);
        }
    }

    public class Magpie
        : Card
    {
        public static Magpie card = new Magpie();

        private Magpie()
            : base("Magpie", Expansion.Adventures, coinCost: 4, plusCards: 1, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
            if (revealedCard.isTreasure)
            {
                currentPlayer.MoveAllRevealedCardsToHand();
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }

            if (revealedCard.isAction || revealedCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(Cards.Magpie, gameState);
            }            
        }
    }

    public class Storyteller
       : Card
    {
        public static Storyteller card = new Storyteller();

        private Storyteller()
            : base("Storyteller", Expansion.Adventures, coinCost: 5, plusActions: 1, plusCoins: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
           for (int i = 0; i < 3; ++i)
           {
               Card cardPlayed = gameState.DoPlayOneTreasure(currentPlayer);
               if (cardPlayed == null)
                   break;               
           }
           currentPlayer.DrawAdditionalCardsIntoHand(currentPlayer.AvailableCoins, gameState);
           currentPlayer.turnCounters.RemoveCoins(currentPlayer.turnCounters.AvailableCoins);
        }
    }

    
}