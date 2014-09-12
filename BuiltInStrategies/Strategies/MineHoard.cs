using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class MineHoard
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "MineHoard",
                        purchaseOrder: PurchaseOrder(),
                        trashOrder: TrashOrder());
        }

        public static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        //CardAcceptance.For(Cards.Colony),
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Hoard, gameState) > 1 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                        CardAcceptance.For(Cards.Hoard),
                        CardAcceptance.For(Cards.Mine, 3),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.Silver));
        }

        public static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(                           
                        CardAcceptance.For(Cards.Curse),                           
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Copper));                           
        }
    }
}


