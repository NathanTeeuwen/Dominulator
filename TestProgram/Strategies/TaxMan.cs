using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class TaxMan
        {            
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("TaxMan",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }                
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 8),
                           CardAcceptance.For<CardTypes.Taxman>(gameState => CountAllOwned<CardTypes.Taxman>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Taxman>(gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 8 || gameState.Self.ExpectedCoinValueAtEndOfTurn > 10));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }
        }
    }
}
