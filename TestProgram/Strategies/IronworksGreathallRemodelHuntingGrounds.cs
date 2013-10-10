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
                     CardAcceptance.For<CardTypes.GreatHall>(gameState => CardBeingPlayedIs<CardTypes.IronWorks>(gameState)),
                     CardAcceptance.For<CardTypes.Province>(),
                     CardAcceptance.For<CardTypes.HuntingGrounds>(gameState => CountAllOwned<CardTypes.Gold>(gameState) >= 1 ),
                     CardAcceptance.For<CardTypes.Gold>(),
                     CardAcceptance.For<CardTypes.Pillage>(gameState => CountAllOwned<CardTypes.Gold>(gameState) == 0),                     
                     CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For<CardTypes.IronWorks>(),
                    CardAcceptance.For<CardTypes.Silver>());

                var lowPriority = new CardPickByPriority(                       
                       CardAcceptance.For<CardTypes.IronWorks>(gameState => CountAllOwned<CardTypes.IronWorks>(gameState) < 2),
                       CardAcceptance.For<CardTypes.Remodel>(),
                       CardAcceptance.For<CardTypes.Silver>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                       
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.GreatHall>(),                           
                           CardAcceptance.For<CardTypes.IronWorks>(
                                gameState => CountOfPile<CardTypes.GreatHall>(gameState) > 0 || 
                                CountAllOwned<CardTypes.Remodel>(gameState) * 6 < gameState.players.CurrentPlayer.AllOwnedCards.Count || 
                                gameState.players.CurrentPlayer.Hand.CountWhere( c => c.isAction && !c.Is<CardTypes.IronWorks>()) == 0),
                           CardAcceptance.For<CardTypes.Pillage>(),
                           CardAcceptance.For<CardTypes.Remodel>(),
                           CardAcceptance.For<CardTypes.HuntingGrounds>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.HuntingGrounds>(),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Remodel>(),
                           CardAcceptance.For<CardTypes.IronWorks>(),                           
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }            
        }
    }
}
