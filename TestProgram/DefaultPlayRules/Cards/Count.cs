using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    public class Count
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Count(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if (acceptableChoice(PlayerActionChoice.Trash))
            {
                bool wantToTrash = WillPlayCountCardForTrash(this.playerAction, gameState);
                if (wantToTrash)
                {
                    return PlayerActionChoice.Trash;
                }
                else if (PreferMoneyOverDuchy(this.playerAction, gameState))
                {
                    return PlayerActionChoice.PlusCoin;
                }
                else
                {
                    return PlayerActionChoice.GainCard;
                }
            }
            else
            {
                if (Strategy.HasExactlyOneActionInHand(gameState))
                {
                    return PlayerActionChoice.TopDeck;
                }
                else if (ShouldGainCopper(gameState))
                {
                    return PlayerActionChoice.GainCard;
                }
                else
                {
                    return PlayerActionChoice.Discard;
                }
            }
        }

        public override Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            Card result = this.playerAction.discardOrder.GetPreferredCardReverse(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
            if (result != null)
            {
                return result;
            }

            return gameState.Self.Hand.FirstOrDefault();
        }

        public static bool WillPlayCountCardForTrash(PlayerAction playerAction, GameState gameState)
        {
            return DoesHandHaveCombinationToTrash(playerAction, gameState) &&
                   Strategy.HasCardFromInHand(playerAction.trashOrder, gameState) &&
                   !playerAction.IsGainingCard(Cards.Province, gameState);
        }

        private static bool PreferMoneyOverDuchy(PlayerAction playerAction, GameState gameState)
        {
            if (!gameState.GetPile(Cards.Duchy).Any)
                return true;

            int minCoin = gameState.Self.ExpectedCoinValueAtEndOfTurn;
            int maxCoin = minCoin + 3;

            Card mostExpensiveCard = playerAction.purchaseOrder.GetPreferredCard(gameState, card => card.CurrentCoinCost(gameState.Self) > minCoin && card.CurrentCoinCost(gameState.Self) <= maxCoin);
            Card thatOrDuchy = playerAction.purchaseOrder.GetPreferredCard(gameState, card => card == Cards.Duchy || card == mostExpensiveCard);

            if (mostExpensiveCard != null && thatOrDuchy != Cards.Duchy)
                return true;

            return false;
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.DefaultGetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        private static bool DoesHandHaveCombinationToTrash(PlayerAction playerAction, GameState gameState)
        {
            int countToTrash = Strategy.CountInHandFrom(playerAction.trashOrder, gameState);
            int countInHand = gameState.Self.Hand.Count;

            return (countInHand - countToTrash <= 2);
        }

        private bool ShouldGainCopper(GameState gameState)
        {
            var self = gameState.Self;
            if (self.Hand.CountWhere(card => card.isAction) > 0)
            {
                return false;
            }

            int countToTrash = Strategy.CountInHandFrom(this.playerAction.trashOrder, gameState);
            int countInHand = self.Hand.Count;

            if (countInHand - countToTrash > 0)
            {
                return false;
            }

            return true;
        }
    }
}