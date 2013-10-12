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
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(cardCost),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder(int followerCost)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(new CardTypes.TestCards.FollowersTest(followerCost), gameState => HasFollowers(gameState, followerCost)),
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2 && ShouldBuyGreen(gameState)),
                           CardAcceptance.For(CardTypes.Silver.card));

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
