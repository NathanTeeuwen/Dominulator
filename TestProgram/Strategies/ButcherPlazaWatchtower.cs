using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{   
    public class ButcherPlazaWatchtower
        : Strategy
    {              
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("ButcherPlazaWatchtower",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        discardOrder: DiscardOrder(),
                        chooseDefaultActionOnNone:false)
            {
            }                
        }           

        private static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 2),                          
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Butcher, gameState => CountAllOwned(Cards.Butcher, gameState) < 2),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3));

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.Silver, gameState => CountAllOwned(Cards.Plaza, gameState) == 0),
                CardAcceptance.For(Cards.Watchtower),
                CardAcceptance.For(Cards.Plaza),
                CardAcceptance.For(Cards.Plaza),
                CardAcceptance.For(Cards.Watchtower)
                );

            var lowPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Watchtower, gameState => CountAllOwned(Cards.Butcher, gameState) + CountAllOwned(Cards.Watchtower, gameState) < CountAllOwned(Cards.Plaza, gameState)),
                        CardAcceptance.For(Cards.Plaza),
                        CardAcceptance.For(Cards.Watchtower));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                
        }            

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Necropolis),
                        CardAcceptance.For(Cards.Butcher, gameState => gameState.Self.AvailableActions > 1),
                        CardAcceptance.For(Cards.Watchtower, gameState => gameState.Self.AvailableActions > 1 && !gameState.Self.Hand.AnyWhere(c => c.isTreasure) && gameState.Self.Hand.AnyOf(Cards.Plaza)),
                        CardAcceptance.For(Cards.Plaza),
                        CardAcceptance.For(Cards.Watchtower, gameState => gameState.Self.AvailableActions == 1),                           
                        CardAcceptance.For(Cards.Butcher),
                        CardAcceptance.For(Cards.Watchtower));
        }                        

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.OvergrownEstate),
                        CardAcceptance.For(Cards.Necropolis),
                        CardAcceptance.For(Cards.Hovel),                           
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Butcher),
                        CardAcceptance.For(Cards.Watchtower),                           
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Province));
        }

        private static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(                           
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Silver));
        }        
    }
}
