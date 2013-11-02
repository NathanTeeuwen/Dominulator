using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class FamiliarPotionSilverOpenning
        : Strategy
    {

        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "FamiliarPotionSilverOpenning",
                        purchaseOrder: PurchaseOrder(),
                        trashOrder: TrashOrder());
        }

        public static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Familiar, 3),
                        CardAcceptance.For(Cards.Potion, 1),
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