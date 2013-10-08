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
        public static class ButcherPlazaWatchtower
        {              
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("ButcherPlazaWatchtower",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder())
                {
                }

                override public Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
                {
                    return Card.Type<CardTypes.Watchtower>();
                }                
            }           

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                         CardAcceptance.For<CardTypes.Province>(),
                          CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                          CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                          CardAcceptance.For<CardTypes.Butcher>(gameState => CountAllOwned<CardTypes.Butcher>(gameState) < 2),
                          CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 3));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Watchtower>(),
                    CardAcceptance.For<CardTypes.Plaza>(),
                    CardAcceptance.For<CardTypes.Plaza>(),
                    CardAcceptance.For<CardTypes.Watchtower>()                    
                    );

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Plaza>(),
                           CardAcceptance.For<CardTypes.Watchtower>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Plaza>(),
                           CardAcceptance.For<CardTypes.Watchtower>(gameState=> gameState.players.CurrentPlayer.AvailableActions == 1),
                           //CardAcceptance.For<CardTypes.Watchtower>(gameState => CountInHand<CardTypes.Estate>(gameState) + CountInHand<CardTypes.Copper>(gameState) == 0 && gameState.players.CurrentPlayer.AvailableActions > 1),
                           CardAcceptance.For<CardTypes.Butcher>(),
                           CardAcceptance.For<CardTypes.Watchtower>());
            }                        

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),
                           CardAcceptance.For<CardTypes.Hovel>(),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Butcher>(),
                           CardAcceptance.For<CardTypes.Watchtower>(),
                           CardAcceptance.For<CardTypes.Duchy>(),
                           CardAcceptance.For<CardTypes.Province>());
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }        
        }
    }
}
