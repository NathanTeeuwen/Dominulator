using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    public class AmbassadorAlwaysReturnBestTrash
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public AmbassadorAlwaysReturnBestTrash(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            Card cardToReturn = playerAction.trashOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));            
            return cardToReturn;
        }

        public override int GetCountToReturnToSupply(Card cardToReturn, GameState gameState)
        {            
            return 2;            
        }
    }

    public class AmbassadorReturnIfNotDisruptPurchase
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public AmbassadorReturnIfNotDisruptPurchase(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            Card cardToReturn = playerAction.trashOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
            if (cardToReturn == null)
                return null;
            
            PlayerState self = gameState.Self;
            int currentCoin = self.ExpectedCoinValueAtEndOfTurn;
            int coinCountIfReturn = currentCoin - cardToReturn.plusCoin;

            
            if (currentCoin < Dominion.Cards.Gold.DefaultCoinCost)
                return cardToReturn;
            
            Card cardWithAllCoin = playerAction.GetCardFromSupplyToBuy(gameState, card => card.CurrentCoinCost(self) <= currentCoin);
            Card cardWithReturnedCard = playerAction.GetCardFromSupplyToBuy(gameState, card => card.CurrentCoinCost(self) <= coinCountIfReturn);

            if (cardWithAllCoin != cardWithReturnedCard)
                return null;

            return cardToReturn;
        }

        public override int GetCountToReturnToSupply(Card cardToReturn, GameState gameState)
        {
            
            PlayerState self = gameState.Self;
            int currentCoin = self.ExpectedCoinValueAtEndOfTurn - cardToReturn.plusCoin;
            int coinCountIfReturn = currentCoin - cardToReturn.plusCoin;


            if (currentCoin < Dominion.Cards.Gold.DefaultCoinCost)
                return 2;
            
            Card cardWithAllCoin = playerAction.GetCardFromSupplyToBuy(gameState, card => card.CurrentCoinCost(self) <= currentCoin);
            Card cardWithReturnedCard = playerAction.GetCardFromSupplyToBuy(gameState, card => card.CurrentCoinCost(self) <= coinCountIfReturn);

            if (cardWithAllCoin != cardWithReturnedCard)
                return 1;
            
            return 2;
        }
    }

    public class AmbassadorMaxReturn
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public AmbassadorMaxReturn(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            int maxCount = 0;
            // find out which card that is wanted to be trashed you have most of in hand.
            foreach (Card card in playerAction.trashOrder.GetNeededCards())
            {
                maxCount = Math.Max(maxCount, gameState.Self.Hand.CountOf(card));
            }

            if (maxCount > 2)
                maxCount = 2;

            Card cardToReturn = playerAction.trashOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.CountOf(card) >= maxCount && acceptableCard(card));
            return cardToReturn;
        }

        public override int GetCountToReturnToSupply(Card cardToReturn, GameState gameState)
        {
            return 2;
        }
    }

}
