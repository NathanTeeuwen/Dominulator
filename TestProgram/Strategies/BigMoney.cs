using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{
    public class BigMoney
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoney",
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}

namespace Strategies
{
    public class BigMoneyColony
        : Strategy 
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyColony",                            
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Colony, gameState => CountAllOwned(Cards.Platinum, gameState) > 2),                           
                        CardAcceptance.For(Cards.Duchy, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Estate, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Platinum),
                        CardAcceptance.For(Cards.Province, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => GainsUntilEndGame(gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static int GainsUntilEndGame(GameState gameState)
        {
            int result = Math.Min(CountOfPile(Cards.Colony, gameState), CountOfPile(Cards.Province, gameState));
            return result;
        }
    }           
}

namespace Strategies
{
    public class BigMoneySmithyEarlyProvince
        : Strategy 
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneySmithyEarlyProvince",                            
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 0),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Smithy, gameState => CountAllOwned(Cards.Smithy, gameState) < 1),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}

namespace Strategies
{
    public class BigMoneyCouncilRoomEarlyProvince
        : Strategy 
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyCouncilRoomEarlyProvince",                            
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 0),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),                           
                        CardAcceptance.For(Cards.CouncilRoom, gameState => CountAllOwned(Cards.CouncilRoom, gameState) < 2),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}
