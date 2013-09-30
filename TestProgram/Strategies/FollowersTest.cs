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
        public static class FollowersTest            
        {
            // big money smithy player
            public static PlayerAction TestPlayer(int playerNumber, int cardCost)
            {
                return new PlayerAction(
                            "FollowersTest",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(cardCost),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(cardCost),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder(int followerCost)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(new CardTypes.TestCards.FollowersTest(followerCost), gameState => HasFollowers(gameState)),
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static bool ShouldBuyGreen(GameState gameState)
            {
                //return !HasFollowers(gameState);
                return true;
            }

            private static bool HasFollowers(GameState gameState)
            {
                return CountAllOwned<CardTypes.TestCards.FollowersTest>(gameState) == 0;
            }

            private static ICardPicker ActionOrder(int followersCost)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(new CardTypes.TestCards.FollowersTest(followersCost)));
            }
        }
    }
}
