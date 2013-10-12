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
                            discardOrder: DiscardOrder(),
                            chooseDefaultActionOnNone:false)
                {
                }                
            }           

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                          CardAcceptance.For(Cards.Province, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                          CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 2),                          
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                          CardAcceptance.For(Cards.Butcher, gameState => CountAllOwned(Cards.Butcher, gameState) < 2),                          
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Watchtower),
                    CardAcceptance.For(Cards.Plaza),
                    CardAcceptance.For(Cards.Plaza),
                    CardAcceptance.For(Cards.Watchtower)                    
                    );

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Plaza),
                           CardAcceptance.For(Cards.Watchtower));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Plaza),
                           CardAcceptance.For(Cards.Watchtower, gameState => gameState.Self.AvailableActions == 1),
                           //CardAcceptance.For(Cards.Watchtower, gameState => CountInHand(Cards.Estate, gameState) + CountInHand(Cards.Copper, gameState) == 0 && gameState.players.CurrentPlayer.AvailableActions > 1),
                           CardAcceptance.For(Cards.Butcher),
                           CardAcceptance.For(Cards.Watchtower));
            }                        

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.OvergrownEstate),
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
}
