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
        public static class RatsUpgradeBazaar
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "RatsUpgradeBazaar",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder());
            }

            static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                         CardAcceptance.For<CardTypes.Province>(),
                         CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 5),
                         CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                         CardAcceptance.For<CardTypes.Upgrade>(gameState => CountAllOwned<CardTypes.Upgrade>(gameState) < 2),
                         CardAcceptance.For<CardTypes.Gold>(),               
                         CardAcceptance.For<CardTypes.Bazaar>(),                                   
                         CardAcceptance.For<CardTypes.Rats>(gameState => CountAllOwned<CardTypes.Rats>(gameState) == 0 && CountAllOwned<CardTypes.Upgrade>(gameState) == 0),
                         CardAcceptance.For<CardTypes.Silver>(),
                         CardAcceptance.For<CardTypes.Rats>(),
                         CardAcceptance.For<CardTypes.Silver>());
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Bazaar>(),
                    CardAcceptance.For<CardTypes.Upgrade>(gameState => CountInHand<CardTypes.Estate>(gameState) > 0),
                    CardAcceptance.For<CardTypes.Rats>(HasCardToRatsInHand),
                    CardAcceptance.For<CardTypes.Upgrade>(HasCardToUpgradeInHand));                    
            }

            static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Rats>(gameState => CountAllOwned<CardTypes.Rats>(gameState) > 1 || DoNotNeedRats(gameState)),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Upgrade>(),
                    CardAcceptance.For<CardTypes.Silver>());
            }

            private static bool HasCardToUpgradeInHand(GameState gameState)
            {
                if (CountInHand<CardTypes.Copper>(gameState) != 0 ||
                       CountInHand<CardTypes.Estate>(gameState) != 0 ||
                       CountInHand<CardTypes.Upgrade>(gameState) > 1)
                    return true;

                int ratCount = CountInHand<CardTypes.Rats>(gameState);

                if (ratCount > 1 || ratCount == 1 && DoNotNeedRats(gameState))
                    return true;

                if (CountInHand<CardTypes.Silver>(gameState) > 0)
                    return true;

                return false;
            }

            private static bool HasCardToRatsInHand(GameState gameState)
            {
                if (CountInHand<CardTypes.Estate>(gameState) != 0)
                    return true;

                if (CountInHand<CardTypes.Copper>(gameState) != 0 &&
                    CountAllOwned<CardTypes.Rats>(gameState) <= 4 )
                    return true;

                return false;
            }     

            private static bool DoNotNeedRats(GameState gameState)
            {
                return CountAllOwned<CardTypes.Copper>(gameState) == 0 &&
                       CountAllOwned<CardTypes.Estate>(gameState) == 0;
            }
        }
    }
}
