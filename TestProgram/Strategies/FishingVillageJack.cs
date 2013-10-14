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
        public static class FishingVillageJack
        {
            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "BigMoneyFishingVillageJack",                            
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.JackOfAllTrades, gameState => CountAllOwned(Cards.JackOfAllTrades, gameState) < 1),
                           CardAcceptance.For(Cards.FishingVillage));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.FishingVillage),
                           CardAcceptance.For(Cards.JackOfAllTrades));
            }
        }
    }
}
