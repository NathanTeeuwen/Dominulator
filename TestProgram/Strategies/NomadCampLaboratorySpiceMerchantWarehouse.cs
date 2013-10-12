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
                     CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >=2),
                     CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                     CardAcceptance.For(Cards.Gold),
                     CardAcceptance.For(Cards.Laboratory));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.NomadCamp),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.SpiceMerchant));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 4));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Laboratory),
                           CardAcceptance.For(Cards.SpiceMerchant, gameState => CountInHand(Cards.Copper, gameState) > 0),
                           CardAcceptance.For(Cards.Warehouse),
                           CardAcceptance.For(Cards.NomadCamp));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Copper));
            } 
        }
    }
}
