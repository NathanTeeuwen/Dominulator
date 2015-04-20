using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class MountebankGovernorMaurader
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
                : base("MountebankGovernorMaurader",
                    purchaseOrder: PurchaseOrder(),
                    trashOrder: TrashOrder())
            {
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)            
            {
                if (gameState.CurrentContext.CurrentCard == Cards.Governor)
                {
                    if (HasCardInHand(Cards.Gold, gameState))
                        return PlayerActionChoice.Trash;
                    else if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 6 && gameState.Self.ExpectedCoinValueAtEndOfTurn < 8)
                        return PlayerActionChoice.PlusCard;
                    else
                        return PlayerActionChoice.GainCard;
                }
                return base.ChooseBetween(gameState, acceptableChoice);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.University, gameState => CountAllOwned(Cards.University, gameState) < 1),
                        CardAcceptance.For(Cards.Mountebank, gameState => CountAllOwned(Cards.Mountebank, gameState) < 1),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Governor),
                        CardAcceptance.For(Cards.University, gameState => CountAllOwned(Cards.University, gameState) < 2),
                //CardAcceptance.For(Cards.Potion, gameState => CountAllOwned(Cards.Potion, gameState) < 1),
                        CardAcceptance.For(Cards.Marauder, gameState => CountAllOwned(Cards.Marauder, gameState) < 1),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Hovel));
        }
    }
}