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
        public static class IronworksGreathallRemodelHuntingGrounds
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
                    : base("IronworksGreathallRemodelHuntingGrounds",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }                
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For(CardTypes.GreatHall.card, gameState => CardBeingPlayedIs(CardTypes.IronWorks.card, gameState)),
                     CardAcceptance.For(CardTypes.Province.card),
                     CardAcceptance.For(CardTypes.HuntingGrounds.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) >= 1 ),
                     CardAcceptance.For(CardTypes.Gold.card),
                     CardAcceptance.For(CardTypes.Pillage.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) == 0),                     
                     CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.IronWorks.card),
                    CardAcceptance.For(CardTypes.Silver.card));

                var lowPriority = new CardPickByPriority(                       
                       CardAcceptance.For(CardTypes.IronWorks.card, gameState => CountAllOwned(CardTypes.IronWorks.card, gameState) < 2),
                       CardAcceptance.For(CardTypes.Remodel.card),
                       CardAcceptance.For(CardTypes.Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                       
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.GreatHall.card),                           
                           CardAcceptance.For(CardTypes.IronWorks.card,
                                gameState => CountOfPile(CardTypes.GreatHall.card, gameState) > 0 ||
                                CountAllOwned(CardTypes.Remodel.card, gameState) * 6 < gameState.Self.AllOwnedCards.Count ||
                                gameState.Self.Hand.CountWhere(c => c.isAction && c != CardTypes.IronWorks.card) == 0),
                           CardAcceptance.For(CardTypes.Pillage.card),
                           CardAcceptance.For(CardTypes.Remodel.card),
                           CardAcceptance.For(CardTypes.HuntingGrounds.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.HuntingGrounds.card),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Remodel.card),
                           CardAcceptance.For(CardTypes.IronWorks.card),                           
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }            
        }
    }
}
