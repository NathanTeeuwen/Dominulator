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
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Smithy>(gameState => CountAllOwned<CardTypes.Smithy>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Smithy>(gameState => CountAllOwned<CardTypes.Smithy>(gameState) < 2 &&
                                                                             gameState.players.CurrentPlayer.AllOwnedCards.Count() >= secondSmithy),
                           CardAcceptance.For<CardTypes.Silver>());

            }           
        }
    }
}
