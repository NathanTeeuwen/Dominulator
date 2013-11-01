using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{
    public class DoubleWarehouse
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "DoubleWarehouse",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        discardOrder: DiscardOrder());
        }

        static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Gold) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Warehouse, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Warehouse) < 1),
                        CardAcceptance.For(Cards.Warehouse, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Silver) > 2 &&
                                                                            gameState.Self.AllOwnedCards.CountOf(Cards.Warehouse) < 2),
                        CardAcceptance.For(Cards.Silver));

        }

        static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Warehouse));
        }

        static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Province),
                CardAcceptance.For(Cards.Duchy),
                CardAcceptance.For(Cards.Estate),
                CardAcceptance.For(Cards.Copper),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Warehouse),
                CardAcceptance.For(Cards.Gold));
        }
    }         
}