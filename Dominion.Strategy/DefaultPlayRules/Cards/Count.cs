using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    public class Count
       : DerivedPlayerAction
    {
        public Count(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
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

        public static bool WillPlayCountCardForTrash(DefaultPlayerAction playerAction, GameState gameState)
        {
            return DoesHandHaveCombinationToTrash(playerAction, gameState) &&
                   Strategy.HasCardFromInHand(playerAction.trashOrder, gameState) &&
                   !playerAction.IsGainingCard(Dominion.Cards.Province, gameState);
        }

        private static bool PreferMoneyOverDuchy(DefaultPlayerAction playerAction, GameState gameState)
        {
            if (!gameState.GetPile(Dominion.Cards.Duchy).Any)
                return true;

            int minCoin = gameState.Self.ExpectedCoinValueAtEndOfTurn;
            int maxCoin = minCoin + 3;

            Card mostExpensiveCard = playerAction.purchaseOrder.GetPreferredCard(gameState, card => card.CurrentCoinCost(gameState.Self) > minCoin && card.CurrentCoinCost(gameState.Self) <= maxCoin);
            Card thatOrDuchy = playerAction.purchaseOrder.GetPreferredCard(gameState, card => card == Dominion.Cards.Duchy || card == mostExpensiveCard);

            if (mostExpensiveCard != null && thatOrDuchy != Dominion.Cards.Duchy)
                return true;

            return false;
        }
       
        private static bool DoesHandHaveCombinationToTrash(DefaultPlayerAction playerAction, GameState gameState)
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