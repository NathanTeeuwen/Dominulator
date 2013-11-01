using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class HorseTraderSoothsayerMinionGreatHall
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("HorseTraderSoothsayerMinionGreatHall",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentCardBeingPlayed == Cards.Minion)
                {
                    if (CountInHand(Cards.Minion, gameState) >= 2)
                        return PlayerActionChoice.PlusCoin;

                    if (HasCardInHand(Cards.Butcher, gameState) && HasCardInHand(Cards.Gold, gameState))
                        return PlayerActionChoice.PlusCoin;

                    if (gameState.Self.ExpectedCoinValueAtEndOfTurn + CountInHand(Cards.Minion, gameState) * 2 >= 6)
                        return PlayerActionChoice.PlusCoin;

                    if (HasCardInHand(Cards.Soothsayer, gameState))
                    {
                        return PlayerActionChoice.PlusCoin;
                    }

                    return PlayerActionChoice.Discard;
                }

                if (gameState.CurrentCardBeingPlayed == Cards.Pawn)
                {
                    if (acceptableChoice(PlayerActionChoice.PlusAction))
                        return PlayerActionChoice.PlusAction;
                    return PlayerActionChoice.PlusCard;
                }

                return base.ChooseBetween(gameState, acceptableChoice);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 6, CardAcceptance.OverPayMaxAmount),
                        CardAcceptance.For(Cards.Soothsayer, 1),
                        CardAcceptance.For(Cards.Butcher, 1, gameState => CountAllOwned(Cards.Minion, gameState) >= 3),
                        CardAcceptance.For(Cards.Gold, gameState => CountAllOwned(Cards.Minion, gameState) >= 3),
                        CardAcceptance.For(Cards.Minion),
                        CardAcceptance.For(Cards.HorseTraders, 1),
                        CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.GreatHall, gameState => CountOfPile(Cards.GreatHall, gameState) >= 2),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Estate, gameState => CardBeingPlayedIs(Cards.Butcher, gameState))
                        );
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.GreatHall),
                        CardAcceptance.For(Cards.Minion),
                        CardAcceptance.For(Cards.Butcher),
                        CardAcceptance.For(Cards.Soothsayer),
                        CardAcceptance.For(Cards.HorseTraders));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                    CardAcceptance.For(Cards.Gold, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                    CardAcceptance.For(Cards.Soothsayer, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                    CardAcceptance.For(Cards.HorseTraders, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                    CardAcceptance.For(Cards.Curse),
                    CardAcceptance.For(Cards.Copper, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 8),
                    CardAcceptance.For(Cards.Estate));
        }
    }
}