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
        public static class NomadCampLaboratorySpiceMerchantWarehouse
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);                
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("NomadCampLaboratorySpiceMerchantWarehouse",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),                         
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }

                public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
                {
                    return true;
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.PlusCard;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) >=2),
                     CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                     CardAcceptance.For(CardTypes.Gold.card),
                     CardAcceptance.For(CardTypes.Laboratory.card));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.NomadCamp.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.SpiceMerchant.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Laboratory.card),
                           CardAcceptance.For(CardTypes.SpiceMerchant.card, gameState => CountInHand(CardTypes.Copper.card, gameState) > 0),
                           CardAcceptance.For(CardTypes.Warehouse.card),
                           CardAcceptance.For(CardTypes.NomadCamp.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Copper.card));
            } 
        }
    }
}
