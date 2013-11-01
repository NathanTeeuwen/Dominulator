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
    public class RatsUpgrade
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "RatsUpgrade",                            
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        chooseDefaultActionOnNone:false);
        }

        static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 6),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Upgrade),
                        CardAcceptance.For(Cards.Rats, gameState => CountAllOwned(Cards.Rats, gameState) == 0),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Rats));               
        }

        static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Rats, HasCardToRatsInHand),
                CardAcceptance.For(Cards.Upgrade, HasCardToUpgradeInHand));                    
        }

        static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Rats, gameState => CountAllOwned(Cards.Rats, gameState) > 1 || DoNotNeedRats(gameState)),
                CardAcceptance.For(Cards.Estate),
                CardAcceptance.For(Cards.Copper),
                CardAcceptance.For(Cards.Upgrade),
                CardAcceptance.For(Cards.Silver));
        }

        private static bool HasCardToUpgradeInHand(GameState gameState)
        {
            if (CountInHand(Cards.Copper, gameState) != 0 ||
                    CountInHand(Cards.Estate, gameState) != 0 ||
                    CountInHand(Cards.Upgrade, gameState) > 1)
                return true;

            int ratCount = CountInHand(Cards.Rats, gameState);

            if (ratCount > 1 || ratCount == 1 && DoNotNeedRats(gameState))
                return true;

            if (CountInHand(Cards.Silver, gameState) > 0)
                return true;

            return false;
        }

        private static bool HasCardToRatsInHand(GameState gameState)
        {
            if (CountInHand(Cards.Estate, gameState) != 0)
                return true;

            if (CountInHand(Cards.Copper, gameState) != 0 &&
                CountAllOwned(Cards.Upgrade, gameState) != 0 &&
                CountAllOwned(Cards.Rats, gameState) <= 4 )
                return true;

            return false;
        }     

        private static bool DoNotNeedRats(GameState gameState)
        {
            return CountAllOwned(Cards.Copper, gameState) == 0 &&
                    CountAllOwned(Cards.Estate, gameState) == 0;
        }
    }
}
