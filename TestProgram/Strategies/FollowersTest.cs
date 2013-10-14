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
            
            public static PlayerAction TestPlayer(int cardCost)
            {
                return new PlayerAction(
                            "FollowersTest",                            
                            purchaseOrder: PurchaseOrder(cardCost),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(cardCost),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder(int followerCost)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(new CardTypes.TestCards.FollowersTest(followerCost), gameState => HasFollowers(gameState, followerCost)),
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(Cards.Silver));

            }

            private static bool ShouldBuyGreen(GameState gameState)
            {
                //return !HasFollowers(gameState);
                return true;
            }

            private static bool HasFollowers(GameState gameState, int followersCost)
            {
                return CountAllOwned(new CardTypes.TestCards.FollowersTest(followersCost), gameState) == 0;
            }

            private static ICardPicker ActionOrder(int followersCost)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(new CardTypes.TestCards.FollowersTest(followersCost)));
            }
        }
    }
}
