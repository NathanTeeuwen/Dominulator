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
        public static class RatsUpgrade
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "RatsUpgrade",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            chooseDefaultActionOnNone:false);
            }

            static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                         CardAcceptance.For(CardTypes.Province.card),
                         CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 6),
                         CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                         CardAcceptance.For(CardTypes.Gold.card),
                         CardAcceptance.For(CardTypes.Upgrade.card),
                         CardAcceptance.For(CardTypes.Rats.card, gameState => CountAllOwned(CardTypes.Rats.card, gameState) == 0),
                         CardAcceptance.For(CardTypes.Silver.card),
                         CardAcceptance.For(CardTypes.Rats.card));               
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Rats.card, HasCardToRatsInHand),
                    CardAcceptance.For(CardTypes.Upgrade.card, HasCardToUpgradeInHand));                    
            }

            static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Rats.card, gameState => CountAllOwned(CardTypes.Rats.card, gameState) > 1 || DoNotNeedRats(gameState)),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Upgrade.card),
                    CardAcceptance.For(CardTypes.Silver.card));
            }

            private static bool HasCardToUpgradeInHand(GameState gameState)
            {
                if (CountInHand(CardTypes.Copper.card, gameState) != 0 ||
                       CountInHand(CardTypes.Estate.card, gameState) != 0 ||
                       CountInHand(CardTypes.Upgrade.card, gameState) > 1)
                    return true;

                int ratCount = CountInHand(CardTypes.Rats.card, gameState);

                if (ratCount > 1 || ratCount == 1 && DoNotNeedRats(gameState))
                    return true;

                if (CountInHand(CardTypes.Silver.card, gameState) > 0)
                    return true;

                return false;
            }

            private static bool HasCardToRatsInHand(GameState gameState)
            {
                if (CountInHand(CardTypes.Estate.card, gameState) != 0)
                    return true;

                if (CountInHand(CardTypes.Copper.card, gameState) != 0 &&
                    CountAllOwned(CardTypes.Upgrade.card, gameState) != 0 &&
                    CountAllOwned(CardTypes.Rats.card, gameState) <= 4 )
                    return true;

                return false;
            }     

            private static bool DoNotNeedRats(GameState gameState)
            {
                return CountAllOwned(CardTypes.Copper.card, gameState) == 0 &&
                       CountAllOwned(CardTypes.Estate.card, gameState) == 0;
            }
        }
    }
}
