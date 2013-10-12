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
        public static class BigMoneyDoubleSmithy
        {

            public static PlayerAction Player(int playerNumber)
            {
                return CustomPlayer(playerNumber);
            }
            // big money smithy player
            public static PlayerAction CustomPlayer(int playerNumber, int secondSmithy = 15)
            {
                return new PlayerAction(
                            "BigMoneyDoubleSmithy",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(secondSmithy));
            }

            private static CardPickByPriority PurchaseOrder(int secondSmithy)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Smithy.card, gameState => CountAllOwned(CardTypes.Smithy.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Smithy.card, gameState => CountAllOwned(CardTypes.Smithy.card, gameState) < 2 &&
                                                                             gameState.Self.AllOwnedCards.Count >= secondSmithy),
                           CardAcceptance.For(CardTypes.Silver.card));

            }           
        }
    }
}
